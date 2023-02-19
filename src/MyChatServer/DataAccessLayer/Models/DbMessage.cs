namespace Sulimov.MyChat.Server.DAL.Models
{
    public class DbMessage
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Message date and time.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Sender.
        /// </summary>
        public string SenderId { get; set; }
        public DbUser Sender { get; set; }

        /// <summary>
        /// Chat.
        /// </summary>
        public int ChatId { get; set; }
        public DbChat Chat { get; set; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; set; }
    }
}
