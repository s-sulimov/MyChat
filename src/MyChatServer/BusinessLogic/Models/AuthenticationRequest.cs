using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for authentification request.
    /// </summary>
    public class AuthenticationRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
