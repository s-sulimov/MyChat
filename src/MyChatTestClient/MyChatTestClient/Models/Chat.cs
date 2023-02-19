using System.Collections.Generic;

namespace Sulimov.MyChat.Client.Models
{
    internal class Chat
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public IEnumerable<User> Users { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
