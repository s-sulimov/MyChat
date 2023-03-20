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
        public int Id { get; set; }

        /// <summary>
        /// Mesaage date and time.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Chat ID.
        /// </summary>
        public int ChatId { get; set; }

        /// <summary>
        /// Sender.
        /// </summary>
        public UserDto Sender { get; set; } = new UserDto();

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; set; } = string.Empty;
    }
}
