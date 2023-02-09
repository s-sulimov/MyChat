using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// DTO for user entity.
    /// </summary>
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }
    }
}
