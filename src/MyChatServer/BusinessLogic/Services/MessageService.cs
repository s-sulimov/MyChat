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
        public Task<Message[]> GetAllChatMessages(int chatId)
        {
            return dbContext.Messages
                .Where(w => w.ChatId == chatId)
                .Select(s => new Message
                {
                    Id = s.Id,
                    ChatId = chatId,
                    SenderId = s.SenderId,
                    Text = s.Text,
                })
                .ToArrayAsync();
        }

        /// <inheritdoc/>
        public async Task<Message> SaveMessage(Message message)
        {
            var dbModel = new DbMessage
            {
                ChatId = message.ChatId,
                SenderId = message.SenderId,
                Text = message.Text,
            };

            dbContext.Messages.Add(dbModel);
            await dbContext.SaveChangesAsync();

            return new Message
            {
                Id = dbModel.Id,
                ChatId = dbModel.ChatId,
                SenderId = dbModel.SenderId,
                Text = dbModel.Text,
            };
        }
    }
}
