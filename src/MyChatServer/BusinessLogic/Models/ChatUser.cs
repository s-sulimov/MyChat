using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <inheritdoc/>
    public class ChatUser : IChatUser
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public IUser User { get; set; }

        /// <inheritdoc/>
        public IChatRole Role { get; set; }

        public ChatUser(int id, User user, ChatRole role)
        {
            Id = id;
            User = user;
            Role = role;
        }

        public ChatUser(DbChatUser dbChatUser)
        {
            Id = dbChatUser.Id;
            User = new User(dbChatUser.User);
            Role = new ChatRole(dbChatUser.Role);
        }
    }
}
