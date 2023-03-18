using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Sulimov.MyChat.Client.Models;
using Sulimov.MyChat.Client.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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
        private MessageViewModel messageViewModel;

        public MainWindow()
        {
            InitializeComponent();

            httpClient = new HttpClient();
            userViewModel = new UserViewModel(httpClient);
            messageViewModel = new MessageViewModel(httpClient);

            this.MessagesList.Items.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
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

            if (!this.userViewModel.IsLoggedIn)
            {
                return;
            }

            this.credentials = this.userViewModel.CurrentCredentials;
            this.UserLabel.Content = this.credentials.Login;

            chatViewModel = new ChatViewModel(httpClient, userViewModel.CurrentCredentials);

            chatViewModel.onChatListUpdated += UpdateChats;
            messageViewModel.onMessageListUpdated += UpdateMesages;

            await CreateHub();

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
            await messageViewModel.GetAllMessages(this.actualChatId);
        }

        private async void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage();
        }

        private void CreateChatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!userViewModel.IsLoggedIn)
            {
                MessageBox.Show("Please Sign In...");
                return;
            }
            
            var createChatWindow = new CreateChat(this.userViewModel.CurrentCredentials, chatViewModel, userViewModel);
            createChatWindow.ShowDialog();
        }

        private async Task SendMessage()
        {
            string message = this.MessageTxtBox.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var result = await messageViewModel.SendMessage(this.actualChatId, message);

            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message);
                return;
            }

            this.MessageTxtBox.Text = string.Empty;
        }

        private void UpdateChats(IEnumerable<Chat> chats)
        {
            this.ChatList.Items.Clear();
            foreach (Chat chat in chats)
            {
                this.ChatList.Items.Add(chat);
            }
        }

        private void UpdateMesages(IEnumerable<Message> messages)
        {
            this.MessagesList.Items.Clear();
            foreach (Message message in messages)
            {
                this.MessagesList.Items.Add(message);
            }
        }

        private async Task CreateHub()
        {
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

            hubConnection.On<Message>("message", message => messageViewModel.ReceieveMessage(message));
            hubConnection.On<Chat>("chat", chat => chatViewModel.ReceieveChat(chat));
            hubConnection.On<int>("remove-user-from-chat", chatId => chatViewModel.RemoveChat(chatId));
        }
    }
}
