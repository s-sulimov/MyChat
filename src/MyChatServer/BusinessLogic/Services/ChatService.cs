using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <inheritdoc/>
    public class ChatService : IChatService
    {
        private readonly DataContext dbContext;

        public ChatService(DataContext dataContext)
        {
            dbContext = dataContext;
        }

        /// <inheritdoc/>
        public async Task<Result<IReadOnlyCollection<Chat>>> GetUserChats(string userId)
        {
            var result = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Where(w => w.Users.Any(a => a.User.Id == userId))
                .Select(s => CreateChat(s))
                .ToListAsync();


            return new Result<IReadOnlyCollection<Chat>>(ResultStatus.Success, result);
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> CreateChat(string title, IReadOnlyCollection<string> userIds, string ownerId)
        {
            if (userIds.Count == 0)
            {
                return new Result<Chat>(ResultStatus.InconsistentData, "Chat hasn't any user.");
            }

            var dbUsers = await dbContext.Users
                .Where(w => w.Id == ownerId || userIds.Contains(w.Id))
                .ToDictionaryAsync(k => k.Id, v => v);

            var sb = new StringBuilder();

            var unknownUsers = userIds.Where(w => !dbUsers.ContainsKey(w));
            foreach (string unknownUser in unknownUsers)
            {
                sb.AppendLine(new CultureInfo("en-US"), $"User {unknownUser} not found.");
            }

            DbUser? dbOwner;
            if (!dbUsers.TryGetValue(ownerId, out dbOwner))
            {
                sb.AppendLine(new CultureInfo("en-US"), $"User {ownerId} not found.");
            }

            if (sb.Length > 0)
            {
                return new Result<Chat>(ResultStatus.ObjectNotFound, sb.ToString());
            }

            var roleOwner = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatOwnerRoleName);
            var roleUser = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (roleOwner == null || roleUser == null)
            {
                return new Result<Chat>(ResultStatus.UnhandledError, Constants.UnknownErrorMessage);
            }

            if (dbOwner == null)
            {
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"User {ownerId} not found.");
            }

            var dbChat = new DbChat
            {
                Title = title,
                Users = new List<DbChatUser>(),
            };

            dbChat.Users = dbUsers
                .Where(w => w.Key != dbOwner.Id)
                .Select(s => new DbChatUser
                {
                    User = s.Value,
                    Role = roleUser,
                })
                .ToList();

            dbChat.Users.Add(new DbChatUser
            {
                User = dbOwner,
                Role = roleOwner,
            });

            dbContext.Add(dbChat);
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChat(dbChat));
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> AddUserToChat(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<Chat>(ResultStatus.InconsistentData, "You can't add yourself");
            }
            
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"Chat {chatId} not found.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"User {userId} not found.");
            }

            var chatUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (chatUser == null || chatUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"Current user doesn't have permission.");
            }

            var role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (role == null)
            {
                return new Result<Chat>(ResultStatus.UnhandledError, Constants.UnknownErrorMessage);
            }

            chat.Users.Add(new DbChatUser
            {
                User = user,
                Role = role,
            });

            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChat(chat));
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
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null || user.Role.Name == Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"Current user doesn't have permission.");
            }

            chat.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChat(chat));
        }

        /// <inheritdoc/>
        public async Task<Result<Chat>> SetChatAdmin(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<Chat>(ResultStatus.InconsistentData, "You can't add yourself");
            }

            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatAdminRoleName);
            if (newRole == null)
            {
                return new Result<Chat>(ResultStatus.UnhandledError, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChat(chat));
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
                return new Result<Chat>(ResultStatus.ObjectNotFound, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<Chat>(ResultStatus.AccessDenied, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (newRole == null)
            {
                return new Result<Chat>(ResultStatus.UnhandledError, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<Chat>(ResultStatus.Success, CreateChat(chat));
        }

        public async Task<IReadOnlyCollection<string>> GetChatUsers(int chatId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return Array.Empty<string>();
            }
            
            return chat.Users
                .Select(s => s.User.Id)
                .ToList();
        }

        public static Chat CreateChat(DbChat chat)
        {
            return new Chat(
                id: chat.Id,
                title: chat.Title,
                users: chat.Users
                    .Select(s => new ChatUser(
                        id: s.Id,
                        user: new User(s.User.Id, s.User.UserName!, s.User.Email!),
                        role: new ChatRole(s.Role.Id, s.Role.Name)))
                    .ToList());
        }
    }
}
