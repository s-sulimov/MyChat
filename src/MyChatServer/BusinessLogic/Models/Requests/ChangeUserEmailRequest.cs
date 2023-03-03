using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Request for user email changing.
    /// </summary>
    public class ChangeUserEmailRequest
    {
        /// <summary>
        /// User password.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// New email.
        /// </summary>
        [Required] 
        public string Email { get; set; } = string.Empty;
    }
}
