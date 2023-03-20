namespace Sulimov.MyChat.Server.Models.Responses
{
    /// <summary>
    /// Chat user item.
    /// </summary>
    public class ChatUserDto
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Chat user.
        /// </summary>
        public UserDto User { get; set; } = new UserDto();

        /// <summary>
        /// User role.
        /// </summary>
        public ChatRoleDto Role { get; set; } = new ChatRoleDto();
    }
}
