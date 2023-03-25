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
        public int Id { get; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; }

        public ChatRoleDto(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
