using Microsoft.AspNetCore.Identity;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL.Models;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Services
{
    /// <inheritdoc/>
    public class UserService : IUserService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;
        private readonly IJwtService jwtService;

        public UserService(UserManager<DbUser> userManager,
            IJwtService jwtService,
            SignInManager<DbUser> signInManager)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.signInManager = signInManager;
        }

        /// <inheritdoc/>
        public async Task<Result<User>> ChangeEmail(string userId, string password, string email)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result<User>(ResultStatus.NotFound, User.Instance, $"User {userId} not found.");
            }

            var checkPasswordResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!checkPasswordResult.Succeeded)
            {
                return new Result<User>(ResultStatus.BadData, User.Instance, "Bad credentials");
            }

            var token = await userManager.GenerateChangeEmailTokenAsync(user, email);
            if (token == null)
            {
                return new Result<User>(ResultStatus.BadData, User.Instance, "Bad email");
            }

            var result = await userManager.ChangeEmailAsync(user, email, token);
            if (!result.Succeeded)
            {
                return new Result<User>(ResultStatus.BadData, User.Instance, Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUserDto(user), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<User>> ChangePassword(string userId, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new Result<User>(ResultStatus.NotFound, User.Instance, $"User {userId} not found.");
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                return new Result<User>(ResultStatus.BadData, User.Instance, Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUserDto(user), string.Empty);
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
                return new Result<User>(ResultStatus.BadData, User.Instance, "User with this credentials has already exists");
            }

            var dbUser = await userManager.FindByNameAsync(userName);
            result = await userManager.AddToRoleAsync(dbUser, Constants.IdentityUserRoleName);
            if (!result.Succeeded)
            {
                return new Result<User>(ResultStatus.BadData, User.Instance, Constants.UnknownErrorMessage);
            }

            return new Result<User>(ResultStatus.Success, CreateUserDto(dbUser), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<User>> GetUser(string userName)
        {
            DbUser dbUser = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (dbUser == null)
            {
                return new Result<User>(ResultStatus.NotFound, User.Instance, $"User with login or email {userName} not found.");
            }

            return new Result<User>(ResultStatus.Success, CreateUserDto(dbUser), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<AuthenticationResponse>> Login(string userName, string password)
        {
            DbUser user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return new Result<AuthenticationResponse>(ResultStatus.NotFound, AuthenticationResponse.Instance, $"User {userName} not found.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return new Result<AuthenticationResponse>(ResultStatus.BadData, AuthenticationResponse.Instance, "Bad credentials");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var token = jwtService.CreateToken(user, userRoles.Select(s => new Claim(ClaimTypes.Role, s)));

            return new Result<AuthenticationResponse>(ResultStatus.Success, token, string.Empty);
        }

        private User CreateUserDto(DbUser dbUser)
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
