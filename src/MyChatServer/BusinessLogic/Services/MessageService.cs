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
            var chat = await dbContext.Chats.FirstOrDefaultAsync(f => f.Id == chatId);
            if (chat == null)
            {
                return new Result<Message>(ResultStatus.NotFound, Message.Instance, "Chat not found.");
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

        private Message CreateMessageDto(DbMessage dbMessage)
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
