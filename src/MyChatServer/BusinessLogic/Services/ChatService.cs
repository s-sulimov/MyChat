using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <inheritdoc/>
    public class ChatService : IChateService
    {
        private readonly DataContext dbContext;

        public ChatService(DataContext dataContext)
        {
            dbContext = dataContext;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Chat>> GetUserChats(string userId)
        {
            return await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Where(w => w.Users.Any(a => a.User.Id == userId))
                .Select(s => CreateChatModel(s))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Chat> CreateChat(Chat chat, string ownerId)
        {
            var ids = chat.Users.Select(s => s.User.Id).ToArray();
            var dbRoles = await dbContext.ChatRoles.ToArrayAsync();
            var dbUsers = await dbContext.Users.Where(w => ids.Contains(w.Id)).ToListAsync();

            if (dbUsers.Count < chat.Users.Count())
            {
                return null;
            }

            var dbChat = new DbChat
            {
                Title = chat.Title,
                Users = new List<DbChatUser>(),
            };

            foreach (ChatUser chatUser in chat.Users)
            {
                var role = chatUser.User.Id != ownerId
                    ? dbRoles.Where(w => w.Name == Constants.ChatUserRoleName).FirstOrDefault()
                    : dbRoles.Where(w => w.Name == Constants.ChatOwnerRoleName).FirstOrDefault();

                var user = dbUsers.Where(w => w.Id == chatUser.User.Id).FirstOrDefault();

                if (role == null || user == null)
                {
                    return null;
                }

                var dbChatUser = new DbChatUser
                {
                    User = user,
                    Role = role,
                };

                dbChat.Users.Add(dbChatUser);
            }

            dbContext.Add(dbChat);
            await dbContext.SaveChangesAsync();

            return CreateChatModel(dbChat);
        }

        /// <inheritdoc/>
        public async Task<Chat> AddUserToChat(int chatId, string actualUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == userId);

            if (chat == null 
                || user == null
                || chat.Users.FirstOrDefault(f => f.User.Id == userId) != null
                || chat.Users.FirstOrDefault(f => f.User.Id == actualUserId)?.Role?.Name != Constants.ChatAdminRoleName)
            {
                return null;
            }

            chat.Users.Add(new DbChatUser
            {
                User = user,
                Role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName),
            });

            await dbContext.SaveChangesAsync();

            return CreateChatModel(chat);
        }

        /// <inheritdoc/>
        public async Task<Chat> RemoveUserFromChat(int chatId, string actualUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            var chatUser = chat?.Users?.FirstOrDefault(f => f.User.Id == userId);

            if (chat == null
                || chatUser == null
                || (chat.Users.FirstOrDefault(f => f.User.Id == actualUserId)?.Role?.Name != Constants.ChatAdminRoleName || actualUserId != userId))
            {
                return null;
            }

            chat.Users.Remove(chatUser);

            await dbContext.SaveChangesAsync();

            return CreateChatModel(chat);
        }

        /// <inheritdoc/>
        public async Task<Chat> SetChatAdmin(int chatId, string actualUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            var chatUser = chat?.Users?.FirstOrDefault(f => f.User.Id == userId);

            if (chat == null
                || chatUser == null
                || chatUser.Role.Name == Constants.ChatOwnerRoleName
                || chat.Users.FirstOrDefault(f => f.User.Id == actualUserId)?.Role?.Name != Constants.ChatOwnerRoleName)
            {
                return null;
            }

            chatUser.Role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatAdminRoleName);

            await dbContext.SaveChangesAsync();

            return CreateChatModel(chat);
        }

        /// <inheritdoc/>
        public async Task<Chat> RemoveChatAdmin(int chatId, string actualUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            var chatUser = chat?.Users?.FirstOrDefault(f => f.User.Id == userId);

            if (chat == null
                || chatUser == null
                || chatUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return null;
            }

            chatUser.Role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);

            await dbContext.SaveChangesAsync();

            return CreateChatModel(chat);
        }

        private Chat CreateChatModel(DbChat dbChat)
        {
            return new Chat
            {
                Id = dbChat.Id,
                Title = dbChat.Title,
                Users = dbChat.Users.Select(u => new ChatUser
                {
                    Id = u.Id,
                    User = new User
                    {
                        Id = u.User.Id,
                        Email = u.User.Email,
                        Name = u.User.UserName,
                    },
                    Role = new ChatRole
                    {
                        Id = u.Role.Id,
                        Name = u.Role.Name,
                    },
                })
            };
        }
    }
}
