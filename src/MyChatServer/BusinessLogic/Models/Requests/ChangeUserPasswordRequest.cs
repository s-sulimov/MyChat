using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Request for user password changing.
    /// </summary>
    public class ChangeUserPasswordRequest
    {
        /// <summary>
        /// Current user password.
        /// </summary>
        [Required]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// New user password.
        /// </summary>
        [Required] 
        public string NewPassword { get; set; } = string.Empty;
    }
}
