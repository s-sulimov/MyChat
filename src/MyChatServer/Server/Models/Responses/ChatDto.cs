namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// Chat.
    /// </summary>
    public class ChatDto
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
        public IReadOnlyCollection<ChatUserDto> Users { get; }

        public ChatDto(int id, string title, IReadOnlyCollection<ChatUserDto> users)
        {
            Id = id;
            Title = title;
            Users = users;
        }
    }
}
