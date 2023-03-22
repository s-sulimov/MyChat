using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.Core.Enums;
using Sulimov.MyChat.Server.Core.Models;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.Services
{
    /// <inheritdoc/>
    public class MessageService : IMessageService
    {
        private readonly DataContext dbContext;

        public MessageService(DataContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc/>
        public async Task<Result<IEnumerable<Message>>> GetAllChatMessages(int chatId, string currentUserId)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId)
                .Select(s => CreateMessage(s))
                .ToArrayAsync();
            
            return new Result<IEnumerable<Message>>(ResultStatus.Success, messages);
        }

        /// <inheritdoc/>
        public async Task<Result<IEnumerable<Message>>> GetLastChatMessages(int chatId, string currentUserId, DateTimeOffset fromDateTime)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId && w.DateTime >= fromDateTime)
                .Select(s => CreateMessage(s))
                .ToArrayAsync();

            return new Result<IEnumerable<Message>>(ResultStatus.Success, messages);
        }

        /// <inheritdoc/>
        public async Task<Result<Message>> SaveMessage(string senderId, int chatId, string message)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<Message>(ResultStatus.ObjectNotFound, new Message(), "Chat not found.");
            }

            var sender = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == senderId);
            if (sender == null)
            {
                return new Result<Message>(ResultStatus.ObjectNotFound, new Message(), $"Sender {senderId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == senderId))
            {
                return new Result<Message>(ResultStatus.ObjectNotFound, new Message(), $"Sender {senderId} isn't included to chat {chatId}.");
            }
            
            var dbModel = new DbMessage
            {
                Sender = sender,
                Chat = chat,
                DateTime = DateTimeOffset.UtcNow,
                Text = message,
            };

            dbContext.Messages.Add(dbModel);
            await dbContext.SaveChangesAsync();

            return new Result<Message>(ResultStatus.Success, CreateMessage(dbModel));
        }

        private async Task<Result<IEnumerable<Message>>> CheckChatExistsAndCurrentUserHasPermissions(int chatId, string currentUserId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IEnumerable<Message>>(ResultStatus.ObjectNotFound, Array.Empty<Message>(), $"Chat {chatId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == currentUserId))
            {
                return new Result<IEnumerable<Message>>(ResultStatus.AccessDenied, Array.Empty<Message>(), $"User {currentUserId} doesn't have access to chat {chatId}");
            }

            return new Result<IEnumerable<Message>>(ResultStatus.Success, Array.Empty<Message>());
        }

        private static Message CreateMessage(DbMessage dbMessage)
        {
            return new Message
            {
                Id = dbMessage.Id,
                ChatId = dbMessage.ChatId,
                DateTime = dbMessage.DateTime,
                Text = dbMessage.Text,
                Sender = new User
                {
                    Id = dbMessage.Sender.Id,
                    Name = dbMessage.Sender.UserName,
                    Email = dbMessage.Sender.Email,
                }
            };
        }
    }
}
