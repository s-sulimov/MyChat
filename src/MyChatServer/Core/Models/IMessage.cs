namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat message.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Mesaage date and time.
        /// </summary>
        DateTime DateTime { get; set; }

        /// <summary>
        /// Chat ID.
        /// </summary>
        int ChatId { get; set; }

        /// <summary>
        /// Sender.
        /// </summary>
        IUser Sender { get; set; }

        /// <summary>
        /// Message text.
        /// </summary>
        string Text { get; set; }
    }
}
