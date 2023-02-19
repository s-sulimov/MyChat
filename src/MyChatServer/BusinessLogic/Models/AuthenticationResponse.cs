namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Response for authentification request.
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// JWT token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration date and time.
        /// </summary>
        public DateTime Expiration { get; set; }
    }
}
