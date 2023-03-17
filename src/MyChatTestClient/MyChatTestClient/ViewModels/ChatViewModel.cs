using Microsoft.AspNetCore.SignalR.Client;
using Sulimov.MyChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sulimov.MyChat.Client.ViewModels
{
    public class ChatViewModel
    {
        private readonly HttpClient client;
        private readonly Credentials currentCredentials;
        private HubConnection hubConnection;
        private List<Chat> chatList = new List<Chat>();

        public Action<List<Chat>> onChatListUpdated;

        public List<Chat> Chats { get; }

        public ChatViewModel(HttpClient client, Credentials currentCredentials, HubConnection hubConnection)
        {
            this.client = client;
            this.currentCredentials = currentCredentials;
            this.hubConnection = hubConnection;
            CreateChatHubHandler();
        }

        public async Task<Result> GetAllUserChats()
        {
            var response = await client.GetAsync($"{Constants.ApiUrl}chats");

            if (!response.IsSuccessStatusCode)
            {
                return new Result { IsSuccess = false, Message = "Problem with network." };
            }

            chatList.Clear();

            var chats = await response.Content.ReadFromJsonAsync<IEnumerable<Chat>>();
            foreach (Chat chat in chats)
            {
                if (string.IsNullOrEmpty(chat.Title))
                {
                    chat.Title = chat.Users.FirstOrDefault(f => f.User.Name != this.currentCredentials.Login).User.Name;
                }

                chatList.Add(chat);
            }

            onChatListUpdated?.Invoke(chatList);

            return new Result { IsSuccess = true, };
        }

        public async Task<Result> CreateChat(string title, IEnumerable<string> userIds)
        {
            var data = new
            {
                Title = title,
                ChatUserIds = userIds,
            };

            var response = await client.PostAsJsonAsync($"{Constants.ApiUrl}chats", data);
            if (!response.IsSuccessStatusCode)
            {
                return new Result { IsSuccess = false, Message = "Problem with network." };
            }

            var newChat = await response.Content.ReadFromJsonAsync<Chat>();

            chatList.Add(newChat);
            onChatListUpdated?.Invoke(chatList);

            return new Result { IsSuccess = true, };
        }

        private void CreateChatHubHandler()
        {
            hubConnection.On<string, Chat>("chat", (user, chat) =>
            {
                int a = 1;
            });
        }
    }
}
