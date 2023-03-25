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
        public int Id { get; }

        /// <summary>
        /// Chat user.
        /// </summary>
        public User User { get; }

        /// <summary>
        /// User role.
        /// </summary>
        public ChatRole Role { get; }

        public ChatUser(int id, User user, ChatRole role)
        {
            Id = id;
            User = user;
            Role = role;
        }
    }
}
