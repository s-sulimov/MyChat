namespace Sulimov.MyChat.Server.BL.Models
{
    /// <summary>
    /// Access role for chat.
    /// </summary>
    public class ChatRole
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}
