using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for authentification request.
    /// </summary>
    public class AuthenticationRequest
    {
        /// <summary>
        /// User login.
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
