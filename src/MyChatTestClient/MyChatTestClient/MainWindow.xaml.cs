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

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Credentials credentials;

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

        private async Task GetChats()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.credentials.Token);
            var response = await client.GetAsync(Constants.ApiUrl + "chats");

            if (!response.IsSuccessStatusCode)
            {
                MessageBox.Show("Problem with network.");
                return;
            }

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
    }
}
