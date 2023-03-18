using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
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
        public async Task<IResult<IEnumerable<IMessage>>> GetAllChatMessages(int chatId, string currentUserId)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId)
                .Select(s => new Message(s))
                .ToArrayAsync();
            
            return new Result<IEnumerable<IMessage>>(ResultStatus.Success, messages);
        }

        /// <inheritdoc/>
        public async Task<IResult<IEnumerable<IMessage>>> GetLastChatMessages(int chatId, string currentUserId, DateTime fromDateTime)
        {
            var result = await CheckChatExistsAndCurrentUserHasPermissions(chatId, currentUserId);
            if (!result.IsSuccess)
            {
                return result;
            }

            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId && w.Date >= fromDateTime.ToUniversalTime())
                .Select(s => new Message(s))
                .ToArrayAsync();

            return new Result<IEnumerable<IMessage>>(ResultStatus.Success, messages);
        }

        /// <inheritdoc/>
        public async Task<IResult<IMessage>> SaveMessage(string senderId, int chatId, string message)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IMessage>(ResultStatus.ObjectNotFound, Message.Instance, "Chat not found.");
            }

            var sender = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == senderId);
            if (sender == null)
            {
                return new Result<IMessage>(ResultStatus.ObjectNotFound, Message.Instance, $"Sender {senderId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == senderId))
            {
                return new Result<IMessage>(ResultStatus.ObjectNotFound, Message.Instance, $"Sender {senderId} isn't included to chat {chatId}.");
            }
            
            var dbModel = new DbMessage
            {
                SenderId = senderId,
                ChatId = chat.Id,
                Date = DateTime.UtcNow,
                Text = message,
            };

            dbContext.Messages.Add(dbModel);
            await dbContext.SaveChangesAsync();

            return new Result<IMessage>(ResultStatus.Success, new Message(dbModel));
        }

        private async Task<IResult<IEnumerable<IMessage>>> CheckChatExistsAndCurrentUserHasPermissions(int chatId, string currentUserId)
        {
            var chat = await dbContext.Chats
                .Include(i => i.Users)
                    .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(f => f.Id == chatId);

            if (chat == null)
            {
                return new Result<IEnumerable<IMessage>>(ResultStatus.ObjectNotFound, new List<Message>(), $"Chat {chatId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == currentUserId))
            {
                return new Result<IEnumerable<IMessage>>(ResultStatus.AccessDenied, new List<Message>(), $"User {currentUserId} doesn't have access to chat {chatId}");
            }

            return new Result<IEnumerable<IMessage>>(ResultStatus.Success, new List<Message>());
        }
    }
}
