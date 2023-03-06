using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;
using System.Text;

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
        public async Task<Result<IEnumerable<Chat>>> GetUserChats(string userId)
        {
            var result = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Where(w => w.Users.Any(a => a.User.Id == userId))
                .Select(s => CreateChatModel(s))
                .ToArrayAsync();


            return new Result<IEnumerable<Chat>>(ResultStatus.Success, result, string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> CreateChat(string title, IEnumerable<string> userIds, string ownerId)
        {
            var dbUsers = await dbContext.Users
                .Where(w => w.Id == ownerId || userIds.Contains(w.Id))
                .ToArrayAsync();

            var sb = new StringBuilder();

            foreach (string userId in userIds)
            {
                if (!dbUsers.Any(a => a.Id == userId))
                {
                    sb.AppendLine($"User {userId} not found.");
                }
            }

            var dbOwner = dbUsers.FirstOrDefault(f => f.Id == ownerId);
            if (dbOwner == null)
            {
                sb.AppendLine($"User {ownerId} not found.");
            }

            if (sb.Length > 0)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, sb.ToString());
            }

            var dbChat = new DbChat
            {
                Title = title,
                Users = new List<DbChatUser>(),
            };

            var roleOwner = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatOwnerRoleName);
            var roleUser = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (roleOwner == null || roleUser == null)
            {
                return new Result<Chat>(ResultStatus.InternalError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            dbChat.Users.Add(new DbChatUser
            {
                User = dbOwner,
                Role = roleOwner,
            });

            foreach (string userId in userIds)
            {
                if (userId == ownerId)
                {
                    continue;
                }
                
                var user = dbUsers.FirstOrDefault(f => f.Id == userId);
                if (user == null)
                {
                    return new Result<Chat>(ResultStatus.InternalError, Chat.Instance, Constants.UnknownErrorMessage);
                }

                var dbChatUser = new DbChatUser
                {
                    User = user,
                    Role = roleUser,
                };

                dbChat.Users.Add(dbChatUser);
            }

            dbContext.Add(dbChat);
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChatModel(dbChat), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> AddUserToChat(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<Chat>(ResultStatus.BadData, Chat.Instance, "You can't add yourself");
            }
            
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, $"User {userId} not found.");
            }

            var chatUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (chatUser == null || chatUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"Current user doesn't have permission.");
            }

            var role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (role == null)
            {
                return new Result<Chat>(ResultStatus.InternalError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            chat.Users.Add(new DbChatUser
            {
                User = user,
                Role = role,
            });

            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChatModel(chat), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> RemoveUserFromChat(int chatId, string currentUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null || user.Role.Name == Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"Current user doesn't have permission.");
            }

            chat.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChatModel(chat), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> SetChatAdmin(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<Chat>(ResultStatus.BadData, Chat.Instance, "You can't add yourself");
            }

            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatAdminRoleName);
            if (newRole == null)
            {
                return new Result<Chat>(ResultStatus.InternalError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChatModel(chat), string.Empty);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> RemoveChatAdmin(int chatId, string currentUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.NotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.Forbidden, Chat.Instance, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (newRole == null)
            {
                return new Result<Chat>(ResultStatus.InternalError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChatModel(chat), string.Empty);
        }

        public async Task<IEnumerable<string>> GetChatUsers(int chatId, string excludeUserId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new string[0];
            }
            
            return chat.Users
                .Where(w => w.User.Id != excludeUserId)
                .Select(s => s.User.Id)
                .ToArray();
        }

        private static Chat CreateChatModel(DbChat dbChat)
        {
            var chat = new Chat
            {
                Id = dbChat.Id,
                Title = dbChat.Title,
            };

            foreach (DbChatUser dbChatUser in dbChat.Users)
            {
                var user = new User
                {
                    Id = dbChatUser.User.Id,
                    Name = dbChatUser.User.UserName,
                    Email = dbChatUser.User.Email,
                };

                var role = new ChatRole
                {
                    Id = dbChatUser.Role.Id,
                    Name = dbChatUser.Role.Name,
                };

                chat.Users.Add(new ChatUser(dbChatUser.Id, user, role));
            }
            
            return chat;
        }
    }
}
