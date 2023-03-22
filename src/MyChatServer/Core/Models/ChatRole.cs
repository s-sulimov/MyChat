namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat role.
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
