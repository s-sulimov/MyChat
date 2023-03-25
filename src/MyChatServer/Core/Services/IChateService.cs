using Sulimov.MyChat.Server.Core.Models;

namespace Sulimov.MyChat.Server.Core.Services
{
    /// <summary>
    /// Service for work with chats.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        /// Return all users's chat.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the list of available chats.</returns>
        Task<Result<IReadOnlyCollection<Chat>>> GetUserChats(string userId);

        /// <summary>
        /// Create new chat.
        /// </summary>
        /// <param name="title">New chat title.</param>
        /// <param name="title">Chat users IDs.</param>
        /// <param name="ownerId">Chat owner ID.</param>
        /// <returns>Result that contains new chat..</returns>
        Task<Result<Chat>> CreateChat(string title, IReadOnlyCollection<string> userIds, string ownerId);

        /// <summary>
        /// Add user to chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the chat.</returns>
        Task<Result<Chat>> AddUserToChat(int chatId, string currentUserId, string userId);

        /// <summary>
        /// Remove user from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the chat.</returns>
        Task<Result<Chat>> RemoveUserFromChat(int chatId, string currentUserId, string userId);

        /// <summary>
        /// Set admin role for chat user.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the chat.</returns>
        Task<Result<Chat>> SetChatAdmin(int chatId, string currentUserId, string userId);

        /// <summary>
        /// Remove admin role for chat user.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the chat.</returns>
        Task<Result<Chat>> RemoveChatAdmin(int chatId, string currentUserId, string userId);

        /// <summary>
        /// Get all chat's users.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <returns>All chat's users.</returns>
        Task<IReadOnlyCollection<string>> GetChatUsers(int chatId);
    }
}
