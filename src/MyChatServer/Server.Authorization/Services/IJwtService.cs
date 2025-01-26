using Sulimov.MyChat.Server.Core.Models.Responses;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Authorization.Services;

public interface IJwtService
{
    /// <summary>
    /// Create new JWT token.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="userName">User name.</param>
    /// <param name="userEmail">User email.</param>
    /// <param name="roleClaims">Claims for user roles.</param>
    /// <returns>Response that contains JWT token.</returns>
    AuthenticationResponse CreateToken(string userId, string userName, string userEmail, IReadOnlyCollection<Claim> roleClaims);
}
