using System.ComponentModel.DataAnnotations;

namespace Sulimov.MyChat.Server.BL.Models.Requests
{
    /// <summary>
    /// Request for chat creation.
    /// </summary>
    public class CreateChatRequest
    {
        /// <summary>
        /// Chat title.
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Chat user (exclude owner).
        /// </summary>
        [Required]
        public IEnumerable<string> ChatUserIds { get; set; } = new List<string>();
    }
}
