namespace Sulimov.MyChat.Server.Authorization.Services;

using Microsoft.AspNetCore.Identity;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL.Models;

/// <inheritdoc/>
public class UserService : IUserService
{
    private readonly UserManager<DbUser> userManager;
    private readonly SignInManager<DbUser> signInManager;
    private readonly ICacheService cacheService;

    private readonly TimeSpan cacheExpirationTime = TimeSpan.FromMinutes(30);

    public UserService(UserManager<DbUser> userManager, SignInManager<DbUser> signInManager, ICacheService cacheService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.cacheService = cacheService;
    }

    /// <inheritdoc/>
    public async Task<Result<User>> ChangeEmail(string userId, string password, string email)
    {
        var dbUser = await userManager.FindByIdAsync(userId);
        if (dbUser == null)
        {
            return new Result<User>(ResultStatus.ObjectNotFound, $"User {userId} not found.");
        }

        var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(dbUser, password, false);
        if (!checkPasswordResult.Succeeded)
        {
            return new Result<User>(ResultStatus.InconsistentData, "Bad credentials");
        }

        var token = await userManager.GenerateChangeEmailTokenAsync(dbUser, email);
        if (token == null)
        {
            return new Result<User>(ResultStatus.InconsistentData, "Bad email");
        }

        var result = await userManager.ChangeEmailAsync(dbUser, email, token);
        if (!result.Succeeded)
        {
            return new Result<User>(ResultStatus.InconsistentData, Constants.UnknownErrorMessage);
        }

        var user = CreateUser(dbUser);
        await this.cacheService.RemoveAsync(CachedDataType.User, user.Name);
        await this.cacheService.SetAsync(CachedDataType.User, user.Name, user, this.cacheExpirationTime);

        return new Result<User>(ResultStatus.Success, user);
    }

    /// <inheritdoc/>
    public async Task<Result<User>> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        var dbUser = await userManager.FindByIdAsync(userId);
        if (dbUser == null)
        {
            return new Result<User>(ResultStatus.ObjectNotFound, $"User {userId} not found.");
        }

        var result = await userManager.ChangePasswordAsync(dbUser, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return new Result<User>(ResultStatus.InconsistentData, Constants.UnknownErrorMessage);
        }

        var user = CreateUser(dbUser);
        await this.cacheService.RemoveAsync(CachedDataType.User, user.Name);
        await this.cacheService.SetAsync(CachedDataType.User, user.Name, user, this.cacheExpirationTime);

        return new Result<User>(ResultStatus.Success, user);
    }

    /// <inheritdoc/>
    public async Task<Result<User>> CreateUser(string userName, string email, string password)
    {
        var result = await userManager.CreateAsync(
            new DbUser()
            {
                UserName = userName,
                Email = email,
            },
            password
        );

        if (!result.Succeeded)
        {
            return new Result<User>(ResultStatus.InconsistentData, "User with this credentials has already exists");
        }

        var dbUser = await userManager.FindByNameAsync(userName);
        if (dbUser == null)
        {
            return new Result<User>(ResultStatus.ObjectNotFound, "User with this credentials wasn't created");
        }

        var addRoleResult = await userManager.AddToRoleAsync(dbUser, Constants.IdentityUserRoleName);
        if (!addRoleResult.Succeeded)
        {
            return new Result<User>(ResultStatus.InconsistentData, Constants.UnknownErrorMessage);
        }

        var user = CreateUser(dbUser);
        await this.cacheService.SetAsync(CachedDataType.User, user.Name, user, this.cacheExpirationTime);

        return new Result<User>(ResultStatus.Success, user);
    }

    /// <inheritdoc/>
    public async Task<Result<User>> GetUser(string userName)
    {
        DbUser? dbUser = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
        if (dbUser == null)
        {
            return new Result<User>(ResultStatus.ObjectNotFound, $"User with login or email {userName} not found.");
        }

        var user = CreateUser(dbUser);
        await this.cacheService.SetAsync(CachedDataType.User, user.Name, user, this.cacheExpirationTime);

        return new Result<User>(ResultStatus.Success, user);
    }

    private static User CreateUser(DbUser dbUser)
    {
        return new User(id: dbUser.Id, name: dbUser.UserName!, email: dbUser.Email!);
    }
}
