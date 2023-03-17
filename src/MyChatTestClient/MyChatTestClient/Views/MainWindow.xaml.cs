using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using Sulimov.MyChat.Client.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Reflection;
using System.Net;
using System.Windows.Markup;
using Sulimov.MyChat.Client.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private Credentials credentials;
        private int actualChatId;
        private UserViewModel userViewModel;
        private HttpClient httpClient;
        private HubConnection hubConnection;
        private ChatViewModel chatViewModel;

        public MainWindow()
        {
            InitializeComponent();

            httpClient = new HttpClient();
            userViewModel = new UserViewModel(httpClient);
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUp(userViewModel);
            signUpWindow.Show();
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var signInWindow = new SignIn(userViewModel);
            signInWindow.ShowDialog();

            if (this.userViewModel.CurrentCredentials == null)
            {
                return;
            }

            this.credentials = this.userViewModel.CurrentCredentials;
            this.UserLabel.Content = this.credentials.Login;

            hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost/chat-hub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(userViewModel.CurrentCredentials.Token);
                })
                .Build();

            hubConnection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await hubConnection.StartAsync();
            };

            await hubConnection.StartAsync();

            chatViewModel = new ChatViewModel(httpClient, userViewModel.CurrentCredentials, hubConnection);

            chatViewModel.onChatListUpdated += UpdateChats;

            var chatResult = await chatViewModel.GetAllUserChats();
            if (!chatResult.IsSuccess)
            {
                MessageBox.Show(chatResult.Message);
            }
        }

        private async void ChatList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var actuaLChat = (Chat)((System.Windows.Controls.Primitives.Selector)sender).SelectedValue;

            if (actuaLChat == null)
            {
                return;
            }

            this.actualChatId = actuaLChat.Id;
            await GetMessages();
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private void CreateChatBtn_Click(object sender, RoutedEventArgs e)
        {
            var createChatWindow = new CreateChat(this.userViewModel.CurrentCredentials, chatViewModel, userViewModel);
            createChatWindow.ShowDialog();
        }

        private async Task GetMessages()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.credentials.Token);

            var response = await client.GetAsync($"{Constants.ApiUrl}messages?chatId={this.actualChatId}");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Problem with network.");
                return;
            }

            this.MessagesList.Items.Clear();

            var messages = await response.Content.ReadFromJsonAsync<IEnumerable<Message>>();
            foreach (Message message in messages)
            {
                this.MessagesList.Items.Add(message);
            }
        }

        private async Task SendMessage()
        {
            string message = this.MessageTxtBox.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var messageData = new Message
            {
                ChatId = this.actualChatId,
                Text= message,
            };

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.credentials.Token);

            var response = await client.PostAsJsonAsync($"{Constants.ApiUrl}messages", messageData);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Problem with network.");
                return;
            }

            var responseData = await response.Content.ReadFromJsonAsync<Message>();

            this.MessagesList.Items.Add(responseData);
            this.MessageTxtBox.Text = string.Empty;
        }

        private void UpdateChats(List<Chat> chats)
        {
            this.ChatList.Items.Clear();
            foreach (Chat chat in chats)
            {
                this.ChatList.Items.Add(chat);
            }
        }
    }
}
