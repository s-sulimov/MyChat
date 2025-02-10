using Microsoft.AspNetCore.Identity;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Responses;
using Sulimov.MyChat.Server.DAL.Models;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Authorization.Services;

/// <inheritdoc />
public class AuthentificationService : IAuthentificationService
{
    private readonly UserManager<DbUser> userManager;
    private readonly SignInManager<DbUser> signInManager;
    private readonly IJwtService jwtService;

    public AuthentificationService(
        UserManager<DbUser> userManager,
        SignInManager<DbUser> signInManager,
        IJwtService jwtService)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
    }

    public async Task<Result<AuthenticationResponse>> Login(string userName, string password)
    {
        string badCredentialsMessage = "Bad credentials.";

        DbUser? user = await userManager.FindByNameAsync(userName) ?? await userManager.FindByEmailAsync(userName);
        if (user == null)
        {
            return new Result<AuthenticationResponse>(ResultStatus.AccessDenied, badCredentialsMessage);
        }

        var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            return new Result<AuthenticationResponse>(ResultStatus.AccessDenied, badCredentialsMessage);
        }

        var userRoles = await userManager.GetRolesAsync(user);

        var jwtResult = this.jwtService.CreateToken(
            userId: user.Id,
            userName:
            user.UserName!,
            userEmail: user.Email!,
            userRoles.Select(s => new Claim(ClaimTypes.Role, s)).ToArray());

        return new Result<AuthenticationResponse>(ResultStatus.Success, jwtResult!);
    }
}
