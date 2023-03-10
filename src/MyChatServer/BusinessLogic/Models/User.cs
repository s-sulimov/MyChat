using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <inheritdoc/>
    public class User : IUser
    {
        /// <summary>
        /// Default instance of <see cref="User"/>
        /// </summary>
        public static User Instance { get; } = new User();

        /// <inheritdoc/>
        public string Id { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Name { get; set; } = string.Empty;

        /// <inheritdoc/>
        public string Email { get; set; } = string.Empty;

        public User() { }

        public User(DbUser dbUser)
        {
            Id = dbUser.Id;
            Name = dbUser.UserName;
            Email = dbUser.Email;
        }
    }
}
