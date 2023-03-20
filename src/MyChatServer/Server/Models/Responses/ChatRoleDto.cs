namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// Chat role.
    /// </summary>
    public class ChatRoleDto
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
