using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Models.Responses;

namespace Sulimov.MyChat.Server.Services;

/// <summary>
/// Client for interaction with user service.
/// </summary>
public interface IUserClient
{
    /// <summary>
    /// Get user info.
    /// </summary>
    /// <param name="userName">Login or email.</param>
    /// <param name="token">Access token.</param>
    /// <returns>Result that contains user info.</returns>
    public Task<Result<UserDto>> GetUser(string userName, string? token);

    /// <summary>
    /// Create new user.
    /// </summary>
    /// <param name="name">Login.</param>
    /// <param name="email">User email.</param>
    /// <param name="password">User password.</param>
    /// <returns>Result that contains new user info.</returns>
    public Task<Result<UserDto>> CreateUser(string name, string email, string password);

    /// <summary>
    /// Change user's email.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="password">Actual password.</param>
    /// <param name="email">New email.</param>
    /// <param name="token">Access token.</param>
    /// <returns>Result that contains user info.</returns>
    public Task<Result<UserDto>> ChangeEmail(string userId, string password, string email, string? token);

    /// <summary>
    /// Change user's password.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="currentPassword">Current user password.</param>
    /// <param name="newPassword">New user password.</param>
    /// <param name="token">Access token.</param>
    /// <returns>Result that contains user info.</returns>
    public Task<Result<UserDto>> ChangePassword(string userId, string currentPassword, string newPassword, string? token);
}
