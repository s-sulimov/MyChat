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
        public int Id { get; set; }

        /// <summary>
        /// Chat title.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Chat users.
        /// </summary>
        public IEnumerable<ChatUserDto> Users { get; set; } = Array.Empty<ChatUserDto>();
    }
}
