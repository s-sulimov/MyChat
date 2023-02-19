namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for chat.
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
        public string Title { get; set; }
        
        /// <summary>
        /// Chat users.
        /// </summary>
        public IEnumerable<User> Users { get; set; }
    }
}
