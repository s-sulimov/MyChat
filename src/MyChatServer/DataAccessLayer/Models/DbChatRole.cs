namespace Sulimov.MyChat.Server.DAL.Models
{
    /// <summary>
    /// Access role for chat.
    /// </summary>
    public class DbChatRole
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; set; }
    }
}
