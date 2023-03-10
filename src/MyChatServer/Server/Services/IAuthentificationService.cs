using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Models.Responses;

namespace Sulimov.MyChat.Server.Services
{
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
}
