namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// User.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// User name (login).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// User e-mail.
        /// </summary>
        public string Email { get; }

        public User(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }
    }
}
