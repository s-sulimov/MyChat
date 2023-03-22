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
        public int Id { get; set; }

        /// <summary>
        /// Mesaage date and time.
        /// </summary>
        public DateTimeOffset DateTime { get; set; }

        /// <summary>
        /// Chat ID.
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Sender.
        /// </summary>
        public User Sender { get; set; } = new User();

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
