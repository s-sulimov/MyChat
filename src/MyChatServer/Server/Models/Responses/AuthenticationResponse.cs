namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// Response for authentification request.
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// Default instance of the <see cref="AuthenticationResponse"/>
        /// </summary>
        public static AuthenticationResponse Instance { get; } = new AuthenticationResponse(string.Empty, default);
        
        /// <summary>
        /// JWT token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Token expiration date and time.
        /// </summary>
        public DateTime Expiration { get; set; }

        public AuthenticationResponse(string token, DateTime expirationDate)
        {
            Token = token;
            Expiration = expirationDate;
        }
    }
}
