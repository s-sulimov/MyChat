using System;

namespace Sulimov.MyChat.Client.Models
{
    public class Message
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int ChatId { get; set; }

        public string RecepientId { get; set; }

        public User Sender { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"[{DateTime.ToLocalTime()}] {Sender.Name}: {Text}";
        }
    }
}
