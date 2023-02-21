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
                    Date = s.Date,
                    Text = s.Text,
                    Sender = new User
                    {
                        Name = s.Sender.UserName,
                        Email = s.Sender.Email,
                    },
                })
                .ToArrayAsync();
        }

        /// <inheritdoc/>
        public async Task<Message> SaveMessage(Message message, string senderId)
        {
            DbChat chat = null;
            if (message.ChatId != 0)
            {
                chat = await dbContext.Chats.FirstOrDefaultAsync(f => f.Id == message.ChatId);
            }
            else
            {
                chat = await GetPrivateChat(senderId, message.RecepientId, null);
            }

            if (chat == null)
            {
                return null;
            }
            
            var dbModel = new DbMessage
            {
                SenderId = senderId,
                ChatId = chat.Id,
                Date = DateTime.UtcNow,
                Text = message.Text,
            };

            dbContext.Messages.Add(dbModel);
            await dbContext.SaveChangesAsync();

            dbModel = await dbContext.Messages.Include(i => i.Sender).FirstOrDefaultAsync(f => f.Id == dbModel.Id);

            return new Message
            {
                Id = dbModel.Id,
                ChatId = dbModel.Id,
                Date = dbModel.Date,
                Sender = new User
                {
                    Id = dbModel.Sender.Id,
                    Name = dbModel.Sender.UserName,
                    Email = dbModel.Sender.Email,
                },
                Text = dbModel.Text,
            };
        }

        /// <summary>
        /// Create or get chat.
        /// </summary>
        /// <param name="senderId">Sender ID.</param>
        /// <param name="recepientId">Recepient ID.</param>
        /// <param name="title">Chat title.</param>
        /// <returns></returns>
        private async Task<DbChat> GetPrivateChat(string senderId, string recepientId, string title)
        {
            var chat = await dbContext.Chats
                .Where(w => w.Users.Count() == 2
                    && w.Users.Any(a => a.Id == senderId)
                    && w.Users.Any(a => a.Id == recepientId))
                .FirstOrDefaultAsync();

            if (chat != null)
            {
                return chat;
            }

            var sender = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == senderId);
            var recepient = await dbContext.Users.FirstOrDefaultAsync(f => f.Id == recepientId);

            if (sender == null || recepient == null)
            {
                return null;
            }

            var newChat = new DbChat
            {
                Title = title,
                Users = new List<DbUser> 
                {
                    sender,
                    recepient
                },
            };

            dbContext.Chats.Add(newChat);
            await dbContext.SaveChangesAsync();

            return newChat;
        }
    }
}
