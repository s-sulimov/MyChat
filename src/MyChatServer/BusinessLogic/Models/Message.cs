using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <inheritdoc/>
    public class Message : IMessage
    {
        /// <summary>
        /// Default instance of <see cref="Message"/>
        /// </summary>
        public static Message Instance { get; set; } = new Message();

        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public DateTime Date { get; set; }

        /// <inheritdoc/>
        public int ChatId { get; set; }

        /// <inheritdoc/>
        public IUser Sender { get; set; } = new User();

        /// <inheritdoc/>
        public string Text { get; set; } = string.Empty;

        public Message() { }

        public Message(DbMessage dbMessage)
        {
            Id = dbMessage.Id;
            ChatId = dbMessage.Id;
            Date = dbMessage.Date;
            Sender = new User
            {
                Id = dbMessage.Sender.Id,
                Name = dbMessage.Sender.UserName,
                Email = dbMessage.Sender.Email,
            };
            Text = dbMessage.Text;
        }
    }
}
