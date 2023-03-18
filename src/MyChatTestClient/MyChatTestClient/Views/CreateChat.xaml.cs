using Sulimov.MyChat.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Net.Http.Json;
using Sulimov.MyChat.Client.ViewModels;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for CreateChat.xaml
    /// </summary>
    public partial class CreateChat : Window
    {
        private Credentials credentials;
        private bool createNew = true;
        private readonly ChatViewModel chatViewModel;
        private readonly UserViewModel userViewModel;

        public Chat Chat;

        public CreateChat(Credentials credentials, ChatViewModel chatViewModel, UserViewModel userViewModel, Chat actualChat = null)
        {
            InitializeComponent();

            this.credentials = credentials;
            this.chatViewModel = chatViewModel;
            this.userViewModel = userViewModel;

            if (credentials == null)
            {
                MessageBox.Show("Please Sign In...");
                return;
            }

            if (actualChat == null)
            {
                Chat = new Chat();
                Chat.Users = new List<ChatUser>();
                FillUsers();
            }
            else
            {
                Chat = actualChat;
                createNew = false;
                this.ChatTitleTxtBox.Text = Chat.Title;
                this.CreateChatBtn.Content = "Save";
            }
        }

        private async void AddUserBtn_Click(object sender, RoutedEventArgs e)
        {
            if (this.credentials == null)
            {
                MessageBox.Show("Please Sign In...");
                return;
            }
            
            string userLogin = this.LoginTxtBox.Text.Trim();

            if (string.IsNullOrEmpty(userLogin))
            {
                MessageBox.Show("Please type user login.");
                return;
            }

            if (Chat.Users.Any(a => a.User.Name == userLogin))
            {
                MessageBox.Show("Already added.");
                return;
            }

            var user = await userViewModel.GetUserInfo(userLogin);

            Chat.Users.Add(new ChatUser { User = user });
            FillUsers();
        }

        private void RemoveUserBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (User)this.UsersListBox.SelectedItem;

            if (selectedUser == null)
            {
                MessageBox.Show("Please select user.");
                return;
            }

            var user = Chat.Users.FirstOrDefault(f => f.User.Id == selectedUser.Id);
            Chat.Users.Remove(user);
            FillUsers();
        }

        private async void CreateChatBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.ChatTitleTxtBox.Text?.Trim()))
            {
                MessageBox.Show("Please type chat title...");
                return;
            }

            if (Chat.Users.Count == 0)
            {
                MessageBox.Show("Please add users...");
                return;
            }
            
            Chat.Title = this.ChatTitleTxtBox.Text.Trim();
            
            if (this.createNew)
            {
                var result = await chatViewModel.CreateChat(Chat.Title, Chat.Users.Select(s => s.User.Id).ToArray());
                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Message);
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillUsers()
        {
            this.UsersListBox.Items.Clear();
            foreach (var user in Chat.Users)
            {
                this.UsersListBox.Items.Add(user);
            }
        }
    }
}
