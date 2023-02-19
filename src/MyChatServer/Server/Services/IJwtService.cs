using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.DAL.Models;
using System.Security.Claims;

namespace Sulimov.MyChat.Server.Services
{
    /// <summary>
    /// Service for generation JWT tokens.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Create new JWT token.
        /// </summary>
        /// <param name="user">User.</param>
        /// <param name="roleClaims">Claims for user roles.</param>
        /// <returns>Response that contains JWT token.</returns>
        AuthenticationResponse CreateToken(DbUser user, IEnumerable<Claim> roleClaims);
    }
}
