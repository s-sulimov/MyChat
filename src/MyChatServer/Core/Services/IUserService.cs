using Sulimov.MyChat.Server.Core.Models;

namespace Sulimov.MyChat.Server.Core.Services
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
        Task<IResult<IUser>> GetUser(string userName);

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="userName">Login.</param>
        /// <param name="email">Email.</param>
        /// <param name="password">Password.</param>
        /// <returns>Result that contains new user info.</returns>
        Task<IResult<IUser>> CreateUser(string userName, string email, string password);

        /// <summary>
        /// Change user's email.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="password">User password.</param>
        /// <param name="email">New e-mail.</param>
        /// <returns>Result that contains user info.</returns>
        Task<IResult<IUser>> ChangeEmail(string userId, string password, string email);

        /// <summary>
        /// Change user's password.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="currentPassword">Current password.</param>
        /// <param name="newPassword">New password.</param>
        /// <returns>Result that contains user info.</returns>
        Task<IResult<IUser>> ChangePassword(string userId, string currentPassword, string newPassword);
    }
}
