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
        public async Task<Result<IReadOnlyCollection<Message>>> GetAllChatMessages(int chatId, string currentUserId)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId)
                .Select(s => CreateMessage(s))
                .ToListAsync();
            
            return new Result<IReadOnlyCollection<Message>>(ResultStatus.Success, messages);
        }

        /// <inheritdoc/>
        public async Task<Result<IReadOnlyCollection<Message>>> GetLastChatMessages(int chatId, string currentUserId, DateTimeOffset fromDateTime)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId && w.DateTime >= fromDateTime)
                .Select(s => CreateMessage(s))
                .ToListAsync();

            return new Result<IReadOnlyCollection<Message>>(ResultStatus.Success, messages);
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
                return new Result<Message>(ResultStatus.ObjectNotFound, "Chat not found.");
            }

            var sender = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == senderId);
            if (sender == null)
            {
                return new Result<Message>(ResultStatus.ObjectNotFound, $"Sender {senderId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == senderId))
            {
                return new Result<Message>(ResultStatus.ObjectNotFound, $"Sender {senderId} isn't included to chat {chatId}.");
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

        private async Task<Result<IReadOnlyCollection<Message>>> CheckChatExistsAndCurrentUserHasPermissions(int chatId, string currentUserId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IReadOnlyCollection<Message>>(ResultStatus.ObjectNotFound, $"Chat {chatId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == currentUserId))
            {
                return new Result<IReadOnlyCollection<Message>>(ResultStatus.AccessDenied, $"User {currentUserId} doesn't have access to chat {chatId}");
            }

            return new Result<IReadOnlyCollection<Message>>(ResultStatus.Success, Array.Empty<Message>());
        }

        private static Message CreateMessage(DbMessage dbMessage)
        {
            return new Message(
                id: dbMessage.Id,
                dateTime: dbMessage.DateTime,
                chatId: dbMessage.ChatId,
                text: dbMessage.Text,
                sender: new User(id: dbMessage.Sender.Id, name: dbMessage.Sender.UserName, email: dbMessage.Sender.Email));
        }
    }
}
