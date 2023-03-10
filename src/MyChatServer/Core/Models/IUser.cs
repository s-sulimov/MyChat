namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// User.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// User ID.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// User name (login).
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// User e-mail.
        /// </summary>
        string Email { get; set; }
    }
}
