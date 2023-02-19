namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for user entity.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// User name (login).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// User e-mail.
        /// </summary>
        public string Email { get; set; }
    }
}
