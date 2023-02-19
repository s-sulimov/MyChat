using Sulimov.MyChat.Server.BL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <summary>
    /// Service for work with chats.
    /// </summary>
    public interface IChateService
    {
        /// <summary>
        /// Retur all users's chat.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Chat>> GetUserChats(string userId);
    }
}
