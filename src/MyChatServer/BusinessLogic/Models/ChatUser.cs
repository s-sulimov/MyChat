namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Collection item for chat user.
    /// </summary>
    public class ChatUser
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chat user.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// User role.
        /// </summary>
        public ChatRole Role { get; set; }
    }
}
