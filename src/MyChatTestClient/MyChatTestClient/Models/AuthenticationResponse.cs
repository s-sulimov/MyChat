using System;

namespace Sulimov.MyChat.Client.Models
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }

        public DateTime Expiration { get; set; }
    }
}
