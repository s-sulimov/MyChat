using Sulimov.MyChat.Server.BL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <summary>
    /// Service for work with messages.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Save message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>Saved message.</returns>
        public Task<Message> SaveMessage(Message message);
 
        /// <summary>
        /// Get all message from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <returns>List of messages.</returns>
        public Task<Message[]> GetAllChatMessages(int chatId);
    }
}
