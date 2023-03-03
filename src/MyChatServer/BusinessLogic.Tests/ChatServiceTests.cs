using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.FunctionalTests
{
    [TestClass]
    public class ChatServiceTests
    {
        DbContextOptions<DataContext> options;
        IChateService chateService;

        public ChatServiceTests()
        {
            FillDataBase();
            chateService = new ChatService(new DataContext(options));
        }

        [TestMethod]
        [DataRow(1, "1", "2", true)]
        [DataRow(1, "2", "3", false)]
        public async Task RemoveUserFromChat(int chatId, string actualUserId, string userId, bool expected)
        {
            // Act.
            var chat = await chateService.RemoveUserFromChat(chatId, actualUserId, userId);

            // Assert.
            Assert.AreEqual(expected, chat != null);
            Assert.AreEqual(expected, !chat?.Users.Any(a => a.User.Id == userId) ?? true);

            bool userRemoved = false;
            using (var context = new DataContext(options))
            {
                userRemoved = !context.Chats.FirstOrDefault(f => f.Id == chatId)?.Users.Any(a => a.User.Id == userId) ?? true;
            }

            Assert.AreEqual(expected, userRemoved);
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

                context.SaveChanges();

                context.Chats.Add(new DbChat
                {
                    Id = 1,
                    Title = "Chat1",
                    Users = new List<DbChatUser>
                    {
                        new DbChatUser
                        {
                            Id = 1,
                            User = context.Users.FirstOrDefault(f => f.Id == "1"),
                            Role = context.ChatRoles.FirstOrDefault(f => f.Name == Constants.ChatOwnerRoleName),
                        },
                        new DbChatUser
                        {
                            Id = 2,
                            User = context.Users.FirstOrDefault(f => f.Id == "2"),
                            Role = context.ChatRoles.FirstOrDefault(f => f.Name == Constants.ChatUserRoleName),
                        },
                        new DbChatUser
                        {
                            Id = 3,
                            User = context.Users.FirstOrDefault(f => f.Id == "3"),
                            Role = context.ChatRoles.FirstOrDefault(f => f.Name == Constants.ChatUserRoleName),
                        },
                    },
                });

                context.SaveChanges();
            }
        }
    }
}