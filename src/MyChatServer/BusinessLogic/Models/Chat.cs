using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <inheritdoc/>
    public class Chat : IChat
    {
        /// <summary>
        /// Default instance of <see cref="Chat"/>.
        /// </summary>
        public static Chat Instance { get; set; } = new Chat();

        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public string Title { get; set; }

        /// <inheritdoc/>
        public IEnumerable<IChatUser> Users { get; set; }

        public Chat()
        {
            Title = string.Empty;
            Users = new List<IChatUser>();
        }

        public Chat(DbChat dbChat)
        {
            Id = dbChat.Id;
            Title = dbChat.Title;
            Users = dbChat.Users.Select(s => new ChatUser(s)).ToArray();
        }
    }
}
