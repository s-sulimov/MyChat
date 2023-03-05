using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.BL.Models.Requests;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <summary>
    /// Service for work with chats.
    /// </summary>
    public interface IChateService
    {
        /// <summary>
        /// Return all users's chat.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>Result that contains the list of available chats.</returns>
        Task<Result<IEnumerable<Chat>>> GetUserChats(string userId);

        /// <summary>
        /// Create new chat.
        /// </summary>
        /// <param name="chat">New chat.</param>
        /// <param name="ownerId">Chat owner ID.</param>
        /// <returns>Result that contains new chat..</returns>
        Task<Result<Chat>> CreateChat(CreateChatRequest chat, string ownerId);

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
        /// <param name="excludeUserId">Excluded user ID.</param>
        /// <returns>All chat's users.</returns>
        Task<IEnumerable<string>> GetChatUsers(int chatId, string excludeUserId);
    }
}
