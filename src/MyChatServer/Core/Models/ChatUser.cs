namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat user item.
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
        public User User { get; set; } = new User();

        /// <summary>
        /// User role.
        /// </summary>
        public ChatRole Role { get; set; } = new ChatRole();
    }
}
