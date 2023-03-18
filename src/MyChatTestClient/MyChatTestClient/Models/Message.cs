using System;
namespace Sulimov.MyChat.Client.Models
{
    public class Message
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int ChatId { get; set; }

        public string RecepientId { get; set; }

        public User Sender { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{Date.ToLocalTime()}] {Sender.Name}: {Text}";
        }
    }
}
