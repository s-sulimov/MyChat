namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat role.
    /// </summary>
    public interface IChatRole
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Role name.
        /// </summary>
        string Name { get; set; }
    }
}
