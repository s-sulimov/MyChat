using Microsoft.EntityFrameworkCore;
using Sulimov.MyChat.Server.BL.Services;
using Sulimov.MyChat.Server.Core;
using Sulimov.MyChat.Server.DAL;
using Sulimov.MyChat.Server.DAL.Models;

namespace Sulimov.MyChat.Server.BL.FunctionalTests
{
    [TestClass]
    public class ChatServiceFunctionalTests
    {
        DbContextOptions<DataContext> options;
        IChateService chateService;

        public ChatServiceFunctionalTests()
        {
            options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "myChatDb")
                .Options;

            FillDataBase();

            chateService = new ChatService(new DataContext(options));
        }

        [TestMethod]
        [DataRow("4", 0)]
        [DataRow("1", 1)]
        public async Task GetUserChats(string userId, int expectedChatsCount)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.GetUserChats(userId);

            // Assert.
            Assert.AreEqual(expectedChatsCount, result.Data.Count());
        }

        [TestMethod]
        [DataRow("New chat", new string[] { "1", "2", "8" }, "3", false)]
        [DataRow("New chat", new string[] { "1", "2" }, "8", false)]
        [DataRow("New chat", new string[] { }, "3", false)]
        [DataRow("New chat", new string[] { "1", "2" }, "3", true)]
        public async Task CreateChat(string title, IEnumerable<string> userIds, string ownerId, bool expected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.CreateChat(title, userIds, ownerId);

            // Assert.
            Assert.AreEqual(expected, result.IsSuccess);
        }

        [TestMethod]
        [DataRow(2, "1", "1", false)]
        [DataRow(1, "5", "1", false)]
        [DataRow(1, "1", "5", false)]
        [DataRow(1, "1", "1", false)]
        [DataRow(1, "2", "3", false)]
        [DataRow(1, "1", "2", true)]
        public async Task RemoveUserFromChat(int chatId, string actualUserId, string userId, bool expected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.RemoveUserFromChat(chatId, actualUserId, userId);

            // Assert.
            Assert.AreEqual(expected, result.IsSuccess);
        }

        [TestMethod]
        [DataRow(2, "1", "2", false)]
        [DataRow(1, "5", "2", false)]
        [DataRow(1, "1", "5", false)]
        [DataRow(1, "2", "3", false)]
        [DataRow(1, "1", "1", false)]
        [DataRow(1, "1", "4", true)]
        public async Task AddUserToChat(int chatId, string actualUserId, string userId, bool expected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.AddUserToChat(chatId, actualUserId, userId);

            // Assert.
            Assert.AreEqual(expected, result.IsSuccess);
        }

        [TestMethod]
        [DataRow(2, "1", "2", false)]
        [DataRow(1, "5", "2", false)]
        [DataRow(1, "1", "5", false)]
        [DataRow(1, "2", "3", false)]
        [DataRow(1, "1", "1", false)]
        [DataRow(1, "1", "2", true)]
        public async Task SetChatAdmin(int chatId, string currentUserId, string userId, bool expected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.SetChatAdmin(chatId, currentUserId, userId);

            // Assert.
            Assert.AreEqual(expected, result.IsSuccess);
        }

        [TestMethod]
        [DataRow(2, "1", "2", false)]
        [DataRow(1, "5", "2", false)]
        [DataRow(1, "1", "5", false)]
        [DataRow(1, "2", "3", false)]
        [DataRow(1, "1", "1", true)]
        [DataRow(1, "1", "2", true)]
        public async Task RemoveChatAdmin(int chatId, string currentUserId, string userId, bool expected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.RemoveChatAdmin(chatId, currentUserId, userId);

            // Assert.
            Assert.AreEqual(expected, result.IsSuccess);
        }

        [TestMethod]
        [DataRow(2, "2", 0)]
        [DataRow(2, "8", 0)]
        [DataRow(1, "0", 3)]
        [DataRow(1, "1", 2)]
        public async Task GetChatUsers(int chatId, string excludeUserId, int usersCountExpected)
        {
            // Arrange.
            await ResetDataBase();

            // Act.
            var result = await chateService.GetChatUsers(chatId, excludeUserId);

            // Assert.
            Assert.AreEqual(usersCountExpected, result.Count());
        }

        private async Task ResetDataBase()
        {
            using (var context = new DataContext(options))
            {
                await context.Database.EnsureDeletedAsync();
            }

            FillDataBase();
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

                context.Chats.Add(new DbChat
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
                });

                context.SaveChanges();
            }
        }
    }
}