using System.Collections.Generic;

namespace Sulimov.MyChat.Client.Models
{
    public class Chat
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public List<User> Users { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
