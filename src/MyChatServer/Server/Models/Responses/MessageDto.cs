namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// Chat message.
    /// </summary>
    public class MessageDto
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Mesaage date and time.
        /// </summary>
        public DateTimeOffset DateTime { get; }

        /// <summary>
        /// Chat ID.
        /// </summary>
        public int ChatId { get; }

        /// <summary>
        /// Sender.
        /// </summary>
        public UserDto Sender { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; }

        public MessageDto(int id, DateTimeOffset dateTime, int chatId, UserDto sender, string text)
        {
            Id = id;
            DateTime = dateTime;
            ChatId = chatId;
            Sender = sender;
            Text = text;
        }
    }
}
