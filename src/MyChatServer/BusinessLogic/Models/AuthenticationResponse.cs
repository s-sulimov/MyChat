namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Response for authentification request.
    /// </summary>
    public class AuthenticationResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
