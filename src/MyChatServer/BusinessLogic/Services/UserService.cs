using Microsoft.AspNetCore.Identity;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;

        public UserService(UserManager<DbUser> userManager, SignInManager<DbUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        /// <inheritdoc/>
        public async Task<Result<User>> ChangeEmail(string userId, string password, string email)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result<User>(ResultStatus.ObjectNotFound, new User(), $"User {userId} not found.");
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!checkPasswordResult.Succeeded)
            {
                return new Result<User>(ResultStatus.InconsistentData, new User(), "Bad credentials");
            }

            var token = await userManager.GenerateChangeEmailTokenAsync(user, email);
            if (token == null)
            {
                return new Result<User>(ResultStatus.InconsistentData, new User(), "Bad email");
            }

            var result = await userManager.ChangeEmailAsync(user, email, token);
            if (!result.Succeeded)
            {
                return new Result<User>(ResultStatus.InconsistentData, new User(), Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUser(user), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<User>> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result<User>(ResultStatus.ObjectNotFound, new User(), $"User {userId} not found.");
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return new Result<User>(ResultStatus.InconsistentData, new User(), Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUser(user), string.Empty);
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
                return new Result<User>(ResultStatus.InconsistentData, new User(), "User with this credentials has already exists");
            }

            var dbUser = await userManager.FindByNameAsync(userName);
            var addRoleResult = await userManager.AddToRoleAsync(dbUser, Constants.IdentityUserRoleName);
            if (!addRoleResult.Succeeded)
            {
                return new Result<User>(ResultStatus.InconsistentData,new User(), Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUser(dbUser), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<User>> GetUser(string userName)
        {
            DbUser dbUser = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (dbUser == null)
            {
                return new Result<User>(ResultStatus.ObjectNotFound, new User(), $"User with login or email {userName} not found.");
            }

            return new Result<User>(ResultStatus.Success, CreateUser(dbUser), string.Empty);
        }

        private static User CreateUser(DbUser dbUser)
        {
            return new User
            {
                Id = dbUser.Id,
                Name = dbUser.UserName,
                Email = dbUser.Email,
            };
        }
    }
}
