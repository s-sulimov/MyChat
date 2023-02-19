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
        public string Title { get; set; }
        
        /// <summary>
        /// Chat users.
        /// </summary>
        public List<DbUser> Users { get; set; }
    }
}
