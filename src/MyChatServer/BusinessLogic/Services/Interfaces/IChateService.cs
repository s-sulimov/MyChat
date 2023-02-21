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
        /// <param name="chat">New chat</param>
        /// <returns>New chat.</returns>
        Task<Chat> CreateChat(Chat chat);
    }
}
