using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Requests;
using Sulimov.MyChat.Server.Core.Models.Responses;

namespace Sulimov.MyChat.Server.Services;

/// <summary>
/// Client for authorization service
/// </summary>
public interface IAuthorizationClient
{
    /// <summary>
    /// Sign in.
    /// </summary>
    /// <param name="request">Credentials.</param>
    /// <returns>Result that contains JWT token.</returns>
    Task<Result<AuthenticationResponse>> Login(AuthenticationRequest request);
}
