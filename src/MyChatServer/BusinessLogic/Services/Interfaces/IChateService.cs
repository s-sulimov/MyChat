using Sulimov.MyChat.Server.BL.Models;

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
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Chat>> GetUserChats(string userId);

        /// <summary>
        /// Create new chat.
        /// </summary>
        /// <param name="chat">New chat.</param>
        /// <param name="ownerId">Chat owner ID.</param>
        /// <returns>New chat.</returns>
        Task<Chat> CreateChat(Chat chat, string ownerId);

        /// <summary>
        /// Add user to chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="actualUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Chat.</returns>
        Task<Chat> AddUserToChat(int chatId, string actualUserId, string userId);

        /// <summary>
        /// Remove user from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="actualUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Chat.</returns>
        Task<Chat> RemoveUserFromChat(int chatId, string actualUserId, string userId);

        /// <summary>
        /// Set admin role for chat user.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="actualUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Chat.</returns>
        Task<Chat> SetChatAdmin(int chatId, string actualUserId, string userId);

        /// <summary>
        /// Remove admin role for chat user.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="actualUserId">Actual user ID.</param>
        /// <param name="userId">User ID.</param>
        /// <returns>Chat.</returns>
        Task<Chat> RemoveChatAdmin(int chatId, string actualUserId, string userId);
    }
}
