using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    public class ChatService : IChateService
    {
        private readonly DataContext dbContext;

        public ChatService(DataContext dataContext)
        {
            dbContext = dataContext;
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

        public async Task<Chat> CreateChat(Chat chat)
        {
            var ids = chat.Users.Select(s => s.Id).ToArray();
            var dbUsers = await dbContext.Users.Where(w => ids.Contains(w.Id)).ToListAsync();
            if (dbUsers.Count < chat.Users.Count())
            {
                return null;
            }

            var dbChat = new DbChat
            {
                Title = chat.Title,
                Users = dbUsers,
            };

            dbContext.Add(dbChat);
            await dbContext.SaveChangesAsync();

            return new Chat
            {
                Id = dbChat.Id,
                Title = dbChat.Title,
                Users = dbChat.Users.Select(s => new User
                {
                    Id = s.Id,
                    Name = s.UserName,
                    Email = s.Email,
                }),
            };
        }
    }
}
