namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Base DTO for user entity.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Default instance of <see cref="User"/>
        /// </summary>
        public static User Instance { get; } = new User();

        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User name (login).
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// User e-mail.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
