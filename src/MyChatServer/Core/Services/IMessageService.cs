using Sulimov.MyChat.Server.Core.Models;

namespace Sulimov.MyChat.Server.Core.Services
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
        public Task<IResult<IMessage>> SaveMessage(string senderId, int chatId, string message);

        /// <summary>
        /// Get all message from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Current user ID ID.</param>
        /// <returns>Result that contains list of messages.</returns>
        public Task<IResult<IEnumerable<IMessage>>> GetAllChatMessages(int chatId, string currentUserId);

        /// <summary>
        /// Get last message from chat.
        /// </summary>
        /// <param name="chatId">Chat ID.</param>
        /// <param name="currentUserId">Current user ID ID.</param>
        /// <param name="fromDateTime">Current user ID ID.</param>
        /// <returns>Result that contains list of messages.</returns>
        public Task<IResult<IEnumerable<IMessage>>> GetLastChatMessages(int chatId, string currentUserId, DateTime fromDateTime);
    }
}
