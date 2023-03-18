using Sulimov.MyChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Sulimov.MyChat.Client.ViewModels
{
    public class MessageViewModel
    {
        private readonly HttpClient httpClient;
        private List<Message> messages;
        private int currentChatId;

        public Action<IEnumerable<Message>> onMessageListUpdated;

        public List<Message> Messages => messages;

        public MessageViewModel(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            messages = new List<Message>();
        }

        public async Task<Result> GetAllMessages(int chatId)
        {
            currentChatId = chatId;
            var response = await httpClient.GetAsync($"{Constants.ApiUrl}messages/all?chatId={chatId}");

            if (!response.IsSuccessStatusCode)
            {
                return new Result { IsSuccess = false, Message = "Problem with network" };
            }

            messages.Clear();

            var chatMessages = await response.Content.ReadFromJsonAsync<IEnumerable<Message>>();
            foreach (Message message in chatMessages)
            {
                messages.Add(message);
            }

            onMessageListUpdated?.Invoke(messages);

            return new Result { IsSuccess = true };
        }

        public async Task<Result> SendMessage(int chatId, string text)
        {
            var messageData = new
            {
                ChatId = chatId,
                Message = text,
            };

            var response = await httpClient.PostAsJsonAsync($"{Constants.ApiUrl}messages", messageData);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return new Result { IsSuccess = false, Message = "Problem with network." };
            }

            var responseData = await response.Content.ReadFromJsonAsync<Message>();

            ReceieveMessage(responseData);

            return new Result { IsSuccess = true };
        }

        public void ReceieveMessage(Message message)
        {
            if (message.ChatId != currentChatId)
            {
                return;
            }

            var currentMessage = messages.FirstOrDefault(f => f.Id == message.Id);
            if (currentMessage != null)
            {
                messages.Remove(currentMessage);
            }

            messages.Add(message);

            onMessageListUpdated?.Invoke(messages);
        }
    }
}
