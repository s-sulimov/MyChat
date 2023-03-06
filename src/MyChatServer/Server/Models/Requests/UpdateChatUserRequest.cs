using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.Models
{
    /// <summary>
    /// Request for update user in chat.
    /// </summary>
    public class UpdateChatUserRequest
    {
        /// <summary>
        /// Chat ID.
        /// </summary>
        [Required]
        public int ChatId { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}
