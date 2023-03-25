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
        public int Id { get; }

        /// <summary>
        /// Chat title.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Chat users.
        /// </summary>
        public IReadOnlyCollection<ChatUser> Users { get; }

        public Chat(int id, string title, IReadOnlyCollection<ChatUser> users)
        {
            Id = id;
            Title = title;
            Users = users;
        }
    }
}
