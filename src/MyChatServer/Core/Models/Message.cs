namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat message.
    /// </summary>
    public class Message
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
        public User Sender { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; }

        public Message(int id, DateTimeOffset dateTime, int chatId, User sender, string text)
        {
            Id = id;
            DateTime = dateTime;
            ChatId = chatId;
            Sender = sender;
            Text = text;
        }
    }
}
