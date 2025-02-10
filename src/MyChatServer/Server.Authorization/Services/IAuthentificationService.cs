using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Responses;

namespace Sulimov.MyChat.Server.Authorization.Services;

/// <summary>
/// Authentification service.
/// </summary>
public interface IAuthentificationService
{
    /// <summary>
    /// Sign in.
    /// </summary>
    /// <param name="userName">Login or email.</param>
    /// <param name="password">Password.</param>
    /// <returns>Result that contains JWT token.</returns>
    Task<Result<AuthenticationResponse>> Login(string userName, string password);
}
