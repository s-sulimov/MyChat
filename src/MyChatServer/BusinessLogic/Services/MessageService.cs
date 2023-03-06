using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Models;
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
        public async Task<Result<IEnumerable<Message>>> GetAllChatMessages(int chatId)
        {
            var messages = await dbContext.Messages
                .Where(w => w.ChatId == chatId)
                .Select(s => CreateMessageDto(s))
                .ToArrayAsync();
            
            return new Result<IEnumerable<Message>>(ResultStatus.Success, messages, string.Empty);
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
                return new Result<Message>(ResultStatus.NotFound, Message.Instance, "Chat not found.");
            }

            var sender = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == senderId);
            if (sender == null)
            {
                return new Result<Message>(ResultStatus.NotFound, Message.Instance, $"Sender {senderId} not found.");
            }

            if (!chat.Users.Any(a => a.User.Id == senderId))
            {
                return new Result<Message>(ResultStatus.NotFound, Message.Instance, $"Sender {senderId} isn't included to chat {chatId}.");
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

            return new Result<Message>(ResultStatus.Success, CreateMessageDto(dbModel), string.Empty);
        }

        private static Message CreateMessageDto(DbMessage dbMessage)
        {
            return new Message
            {
                Id = dbMessage.Id,
                ChatId = dbMessage.Id,
                Date = dbMessage.Date,
                Sender = new User
                {
                    Id = dbMessage.Sender.Id,
                    Name = dbMessage.Sender.UserName,
                    Email = dbMessage.Sender.Email,
                },
                Text = dbMessage.Text,
            };
        }
    }
}
