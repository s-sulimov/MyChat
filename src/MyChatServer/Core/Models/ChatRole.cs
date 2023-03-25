namespace Sulimov.MyChat.Server.Core.Models
{
    /// <summary>
    /// Chat role.
    /// </summary>
    public class ChatRole
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Role name.
        /// </summary>
        public string Name { get; }

        public ChatRole(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
