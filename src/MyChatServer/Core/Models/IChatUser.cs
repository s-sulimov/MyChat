namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat user item.
    /// </summary>
    public interface IChatUser
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Chat user.
        /// </summary>
        IUser User { get; set; }

        /// <summary>
        /// User role.
        /// </summary>
        IChatRole Role { get; set; }
    }
}
