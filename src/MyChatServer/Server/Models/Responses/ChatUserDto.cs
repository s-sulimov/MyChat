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
        public int Id { get; }

        /// <summary>
        /// Chat user.
        /// </summary>
        public UserDto User { get; }

        /// <summary>
        /// User role.
        /// </summary>
        public ChatRoleDto Role { get; }

        public ChatUserDto(int id, UserDto user, ChatRoleDto role)
        {
            Id = id;
            User = user;
            Role = role;
        }
    }
}
