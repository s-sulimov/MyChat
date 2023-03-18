using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;
using System.Globalization;
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
        public async Task<IResult<IEnumerable<IChat>>> GetUserChats(string userId)
        {
            var result = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Where(w => w.Users.Any(a => a.User.Id == userId))
                .Select(s => new Chat(s))
                .ToArrayAsync();


            return new Result<IEnumerable<IChat>>(ResultStatus.Success, result);
        }

        /// <inheritdoc/>
        public async Task<IResult<IChat>> CreateChat(string title, IEnumerable<string> userIds, string ownerId)
        {
            var dbUsers = await dbContext.Users
                .Where(w => w.Id == ownerId || userIds.Contains(w.Id))
                .ToArrayAsync();

            var sb = new StringBuilder();

            foreach (string userId in userIds)
            {
                if (!dbUsers.Any(a => a.Id == userId))
                {
                    sb.AppendLine(new CultureInfo("en-US"), $"User {userId} not found.");
                }
            }

            var dbOwner = dbUsers.FirstOrDefault(f => f.Id == ownerId);
            if (dbOwner == null)
            {
                sb.AppendLine(new CultureInfo("en-US"), $"User {ownerId} not found.");
            }

            if (sb.Length > 0)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, sb.ToString());
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
                return new Result<IChat>(ResultStatus.UnhandledError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            dbChat.Users.Add(new DbChatUser
            {
#pragma warning disable CS8601 // JUSTIFICATION: Checked above.
                User = dbOwner,
#pragma warning restore CS8601
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
                    return new Result<IChat>(ResultStatus.UnhandledError, Chat.Instance, Constants.UnknownErrorMessage);
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

            return new Result<IChat>(ResultStatus.Success, new Chat(dbChat));
        }

        /// <inheritdoc/>
        public async Task<IResult<IChat>> AddUserToChat(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<IChat>(ResultStatus.InconsistentData, Chat.Instance, "You can't add yourself");
            }
            
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user == null)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, $"User {userId} not found.");
            }

            var chatUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (chatUser == null || chatUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"Current user doesn't have permission.");
            }

            var role = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (role == null)
            {
                return new Result<IChat>(ResultStatus.UnhandledError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            chat.Users.Add(new DbChatUser
            {
                User = user,
                Role = role,
            });

            await dbContext.SaveChangesAsync();

            return new Result<IChat>(ResultStatus.Success, new Chat(chat));
        }

        /// <inheritdoc/>
        public async Task<IResult<IChat>> RemoveUserFromChat(int chatId, string currentUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null || user.Role.Name == Constants.ChatOwnerRoleName)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name == Constants.ChatUserRoleName)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"Current user doesn't have permission.");
            }

            chat.Users.Remove(user);
            await dbContext.SaveChangesAsync();

            return new Result<IChat>(ResultStatus.Success, new Chat(chat));
        }

        /// <inheritdoc/>
        public async Task<IResult<IChat>> SetChatAdmin(int chatId, string currentUserId, string userId)
        {
            if (currentUserId == userId)
            {
                return new Result<IChat>(ResultStatus.InconsistentData, Chat.Instance, "You can't add yourself");
            }

            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatAdminRoleName);
            if (newRole == null)
            {
                return new Result<IChat>(ResultStatus.UnhandledError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<IChat>(ResultStatus.Success, new Chat(chat));
        }

        /// <inheritdoc/>
        public async Task<IResult<IChat>> RemoveChatAdmin(int chatId, string currentUserId, string userId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.Role)
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IChat>(ResultStatus.ObjectNotFound, Chat.Instance, $"Chat {chatId} not found.");
            }

            var user = chat.Users.FirstOrDefault(f => f.User.Id == userId);
            if (user == null)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"User {userId} doesn't include in the chat {chatId}.");
            }

            var currentUser = chat.Users.FirstOrDefault(f => f.User.Id == currentUserId);
            if (currentUser == null || currentUser.Role.Name != Constants.ChatOwnerRoleName)
            {
                return new Result<IChat>(ResultStatus.AccessDenied, Chat.Instance, $"Current user doesn't have permission.");
            }

            var newRole = await dbContext.ChatRoles.FirstOrDefaultAsync(f => f.Name == Constants.ChatUserRoleName);
            if (newRole == null)
            {
                return new Result<IChat>(ResultStatus.UnhandledError, Chat.Instance, Constants.UnknownErrorMessage);
            }

            user.Role = newRole;
            await dbContext.SaveChangesAsync();

            return new Result<IChat>(ResultStatus.Success, new Chat(chat));
        }

        public async Task<IEnumerable<string>> GetChatUsers(int chatId)
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
                .ToArray();
        }
    }
}
