using Sulimov.MyChat.Server.BL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <summary>
    /// Service for work with messages.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Save new message.
        /// </summary>
        /// <param name="senderId">Sender ID.</param>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="message">Message text.</param>
        /// <returns>Result that contains saved message.</returns>
        public Task<Result<Message>> SaveMessage(string senderId, int chatId, string message);
 
        /// <summary>
        /// Get all message from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <returns>Result that contains list of messages.</returns>
        public Task<Result<IEnumerable<Message>>> GetAllChatMessages(int chatId);
    }
}
