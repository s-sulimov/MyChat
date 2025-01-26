using Microsoft.AspNetCore.Identity;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Responses;
using Sulimov.MyChat.Server.DAL.Models;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Services
{
    public class AuthentificationService : IAuthentificationService
    {
        private readonly UserManager<DbUser> userManager;
        private readonly SignInManager<DbUser> signInManager;
        private readonly IJwtService jwtService;

        public AuthentificationService(UserManager<DbUser> userManager,
            IJwtService jwtService,
            SignInManager<DbUser> signInManager)
        {
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.signInManager = signInManager;
        }

        public async Task<Result<AuthenticationResponse>> Login(string userName, string password)
        {
            DbUser? user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                return new Result<AuthenticationResponse>(ResultStatus.ObjectNotFound, $"User {userName} not found.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return new Result<AuthenticationResponse>(ResultStatus.InconsistentData, "Bad credentials");
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var token = jwtService.CreateToken(user, userRoles.Select(s => new Claim(ClaimTypes.Role, s)).ToList());

            return new Result<AuthenticationResponse>(ResultStatus.Success, token);
        }
    }
}
