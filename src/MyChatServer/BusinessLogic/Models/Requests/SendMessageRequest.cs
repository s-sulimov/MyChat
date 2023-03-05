using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Send message request.
    /// </summary>
    public class SendMessageRequest
    {
        /// <summary>
        /// Chat ID>
        /// </summary>
        [Required]
        public int ChatId { get; set; }

        /// <summary>
        /// Message text.
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;
    }
}
