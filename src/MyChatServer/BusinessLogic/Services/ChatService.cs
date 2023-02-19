using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.DAL;

namespace Sulimov.MyChat.Server.BL.Services
{
    public class ChatService : IChateService
    {
        private readonly DataContext dbContext;

        public ChatService(DataContext dataContext)
        {
            this.dbContext = dataContext;
        }

        public async Task<IEnumerable<Chat>> GetUserChats(string userId)
        {
            return await dbContext.Chats
                .Include(i => i.Users)
                .Where(w => w.Users.Any(a => a.Id == userId))
                .Select(s => new Chat
                {
                    Id= s.Id,
                    Title = s.Title,
                    Users = s.Users.Select(u => new User
                    {
                        Id = u.Id,
                        Email= u.Email,
                        Name = u.UserName,
                    })
                })
                .ToListAsync();
        }
    }
}
