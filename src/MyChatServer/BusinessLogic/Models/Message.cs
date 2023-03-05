namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Default instance of <see cref="Message"/>
        /// </summary>
        public static Message Instance { get; set; } = new Message();

        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Mesaage date and time.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Chat ID.
        /// </summary>
        public int ChatId { get; set; }
        
        /// <summary>
        /// Recepient ID.
        /// </summary>
        public string RecepientId { get; set; } = string.Empty;
        
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
