namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Chat ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chat title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Chat users.
        /// </summary>
        public IEnumerable<ChatUser> Users { get; set; } = Array.Empty<ChatUser>();
    }
}
