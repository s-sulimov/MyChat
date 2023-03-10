using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <inheritdoc/>
    public class ChatRole : IChatRole
    {
        /// <inheritdoc/>
        public int Id { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;

        public ChatRole() { }

        public ChatRole(DbChatRole dbChatRole)
        {
            Id = dbChatRole.Id;
            Name = dbChatRole.Name;
        }
    }
}
