namespace Sulimov.MyChat.Client.Models
{
    /// <summary>
    /// Class for store credentials.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        /// User ID.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// User login.
        /// </summary>
        public string Login { get; set; } = string.Empty;

        /// <summary>
        /// User password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Email.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Current token.
        /// </summary>
        public string Token { get; set; } = string.Empty;
    }
}
