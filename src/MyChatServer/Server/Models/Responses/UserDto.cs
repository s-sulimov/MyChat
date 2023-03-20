namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// User.
    /// </summary>
    public class UserDto
    {
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
