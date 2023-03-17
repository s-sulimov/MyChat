namespace Sulimov.MyChat.Client.Models
{
    public class ChatUser
    {
        public int Id { get; set; }
        public User User { get; set; }
        public ChatRole Role { get; set; }

        public override string ToString()
        {
            return User?.Name;
        }
    }
}
