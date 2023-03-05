namespace Sulimov.MyChat.Server.DAL.Models
{
    /// <summary>
    /// Chat data model.
    /// </summary>
    public class DbChat
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Chat title.
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// Chat users and roles.
        /// </summary>
        public List<DbChatUser> Users { get; set; } = new List<DbChatUser>();
    }
}
