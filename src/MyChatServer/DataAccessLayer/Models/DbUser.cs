using Microsoft.AspNetCore.Identity;

namespace Sulimov.MyChat.Server.DAL.Models
{
    /// <summary>
    /// User data model.
    /// </summary>
    public class DbUser : IdentityUser
    {
        /// <summary>
        /// User chats.
        /// </summary>
        public List<DbChat> Chats { get; set; } = new List<DbChat>();
    }
}
