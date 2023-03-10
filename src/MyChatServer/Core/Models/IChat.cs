namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat.
    /// </summary>
    public interface IChat
    {
        /// <summary>
        /// Chat ID.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Chat title.
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Chat users.
        /// </summary>
        IEnumerable<IChatUser> Users { get; set; }
    }
}
