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

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private Credentials credentials;
        private int actualChatId;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUp();
            signUpWindow.Show();
        }

        private async void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var signInWindow = new SignIn();
            signInWindow.ShowDialog();

            if (signInWindow.Credentials == null)
            {
                return;
            }

            this.credentials = signInWindow.Credentials;
            this.UserLabel.Content = this.credentials.Login;

            await GetChats();
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
            var createChatWindow = new CreateChat(credentials);
            createChatWindow.ShowDialog();

            if (createChatWindow.Chat != null && createChatWindow.Chat.Id != 0)
            {
                this.ChatList.Items.Add(createChatWindow.Chat);
            }
        }

        private async Task GetChats()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.credentials.Token);
            var response = await client.GetAsync($"{Constants.ApiUrl}chats");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Problem with network.");
                return;
            }

            this.ChatList.Items.Clear();

            var chats = await response.Content.ReadFromJsonAsync<IEnumerable<Chat>>();
            foreach (Chat chat in chats)
            {
                if (string.IsNullOrEmpty(chat.Title))
                {
                    chat.Title = chat.Users.FirstOrDefault(f => f.Name != this.credentials.Login).Name;
                }

                this.ChatList.Items.Add(chat);
            }
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
    }
}
