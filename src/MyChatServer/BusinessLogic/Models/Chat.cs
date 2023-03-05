namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for chat.
    /// </summary>
    public class Chat
    {
        /// <summary>
        /// Default instance of <see cref="Chat"/>.
        /// </summary>
        public static Chat Instance { get; set; } = new Chat();

        /// <summary>
        /// Chat ID.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Chat title.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Chat users.
        /// </summary>
        public List<ChatUser> Users { get; set; }

        public Chat()
        {
            Title = string.Empty;
            Users = new List<ChatUser>();
        }
    }
}
