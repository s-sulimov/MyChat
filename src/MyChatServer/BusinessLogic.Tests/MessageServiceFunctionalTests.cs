using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.Core.Services;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.FunctionalTests
{
    [TestClass]
    public class MessageServiceFunctionalTests
    {        
        DbContextOptions<DataContext> options;
        IMessageService messageService;

        public MessageServiceFunctionalTests()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "myChatDb")
                .Options;

            FillDataBase();

            messageService = new MessageService(new DataContext(options));
        }

        private async Task ResetDataBase()
        {
            using (var context = new DataContext(options))
            {
                await context.Database.EnsureDeletedAsync();
            }

            FillDataBase();
        }

        [TestMethod]
        [DataRow(2, "1", 0)]
        [DataRow(1, "4", 0)]
        [DataRow(1, "1", 2)]
        public async Task GetAllChatMessages(int chatId, string currentUserId, int messageCountExpected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await messageService.GetAllChatMessages(chatId, currentUserId);

            // Assert.
            Assert.AreEqual(messageCountExpected, result.Data.Count());
        }

        [TestMethod]
        [DataRow(2, "1", 0)]
        [DataRow(1, "4", 0)]
        [DataRow(1, "1", 2)]
        public async Task GetLastChatMessages(int chatId, string currentUserId, int messageCountExpected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result1 = await messageService.GetLastChatMessages(chatId, currentUserId, DateTime.Now.AddHours(1).ToUniversalTime());
            var result2 = await messageService.GetLastChatMessages(chatId, currentUserId, DateTime.MinValue.ToUniversalTime());

            // Assert.
            Assert.AreEqual(0, result1.Data.Count());
            Assert.AreEqual(messageCountExpected, result2.Data.Count());
        }

        [TestMethod]
        [DataRow("1", 2, "New message", false)]
        [DataRow("8", 1, "New message", false)]
        [DataRow("4", 1, "New message", false)]
        [DataRow("2", 1, "New message", true)]
        public async Task SaveMessage(string senderId, int chatId, string message, bool expexted)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await messageService.SaveMessage(senderId, chatId, message);

            // Assert.
            Assert.AreEqual(expexted, result.IsSuccess);
        }

        private void FillDataBase()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "myChatDb")
                .Options;

            using (var context = new DataContext(options))
            {
                // If the database has already filled.
                if (context.Users.Count() > 0)
                {
                    return;
                }

                context.Users.Add(new DbUser
                {
                    Id = "1",
                    UserName = "User1",
                    NormalizedUserName = "User1",
                });

                context.Users.Add(new DbUser
                {
                    Id = "2",
                    UserName = "User2",
                    NormalizedUserName = "User2",
                });

                context.Users.Add(new DbUser
                {
                    Id = "3",
                    UserName = "User3",
                    NormalizedUserName = "User3",
                });

                context.Users.Add(new DbUser
                {
                    Id = "4",
                    UserName = "User4",
                    NormalizedUserName = "User4",
                });

                context.ChatRoles.Add(new DbChatRole
                {
                    Name = Constants.ChatOwnerRoleName,
                });

                context.ChatRoles.Add(new DbChatRole
                {
                    Name = Constants.ChatAdminRoleName,
                });

                context.ChatRoles.Add(new DbChatRole
                {
                    Name = Constants.ChatUserRoleName,
                });

                context.SaveChanges();

                var newChat = new DbChat
                {
                    Id = 1,
                    Title = "Chat1",
                    Users = new List<DbChatUser>
                    {
                        new DbChatUser
                        {
                            User = context.Users.First(f => f.Id == "1"),
                            Role = context.ChatRoles.First(f => f.Name == Constants.ChatOwnerRoleName),
                        },
                        new DbChatUser
                        {
                            User = context.Users.First(f => f.Id == "2"),
                            Role = context.ChatRoles.First(f => f.Name == Constants.ChatUserRoleName),
                        },
                        new DbChatUser
                        {
                            User = context.Users.First(f => f.Id == "3"),
                            Role = context.ChatRoles.First(f => f.Name == Constants.ChatUserRoleName),
                        },
                    },
                };

                context.Chats.Add(newChat);

                context.SaveChanges();

                context.Messages.Add(new DbMessage
                {
                    Chat = newChat,
                    Date = DateTime.MinValue.ToUniversalTime(),
                    SenderId = "1",
                    Text = "Hey",
                });

                context.Messages.Add(new DbMessage
                {
                    Chat = newChat,
                    Date = DateTime.Now.ToUniversalTime(),
                    SenderId = "2",
                    Text = "Hey guys",
                });

                context.SaveChanges();
            }
        }
    }
}
