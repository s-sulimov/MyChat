namespace Sulimov.MyChat.Server.DAL.Models
{
    /// <summary>
    /// Collection item for chat user.
    /// </summary>
    public class DbChatUser
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Chat user.
        /// </summary>
        public DbUser User { get; set; } = new DbUser();
        
        /// <summary>
        /// User role.
        /// </summary>
        public DbChatRole Role { get; set; } = new DbChatRole();
    }
}
