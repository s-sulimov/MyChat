using Sulimov.MyChat.Server.BL.Models;

namespace Sulimov.MyChat.Server.Services
{
    /// <summary>
    /// Service for user managment.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Get user info.
        /// </summary>
        /// <param name="userName">Login or email.</param>
        /// <returns>Result that contains user info.</returns>
        Task<Result<User>> GetUser(string userName);

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="userName">Login.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>Result that contains new user info.</returns>
        Task<Result<User>> CreateUser(string userName, string email, string password);

        /// <summary>
        /// Change user's email.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="password">User password.</param>
        /// <param name="email">New e-mail.</param>
        /// <returns>Result that contains user info.</returns>
        Task<Result<User>> ChangeEmail(string userId, string password, string email);

        /// <summary>
        /// Change user's password.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="currentPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>Result that contains user info.</returns>
        Task<Result<User>> ChangePassword(string userId, string currentPassword, string newPassword);

        /// <summary>
        /// Sign in.
        /// </summary>
        /// <param name="userName">Login or email.</param>
        /// <param name="password">Password.</param>
        /// <returns>Result that contains JWT token.</returns>
        Task<Result<AuthenticationResponse>> Login(string userName, string password);
    }
}
