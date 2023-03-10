using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
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
using Sulimov.MyChat.Client.Models;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        internal Credentials Credentials { get; set; }

        public SignIn()
        {
            InitializeComponent();
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.LoginTxtBox.Text))
            {
                MessageBox.Show("Please type e-mail...");
                return;
            }

            if (string.IsNullOrEmpty(this.PasswordTxtBox.Text))
            {
                MessageBox.Show("Please type password...");
                return;
            }

            var data = new
            {
                Login = this.LoginTxtBox.Text,
                Password = this.PasswordTxtBox.Text,
            };

            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync($"{Constants.ApiUrl}users/login", data);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Bad credentials");
                return;
            }

            var responseData = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            this.Credentials = new Credentials
            {
                Login = this.LoginTxtBox.Text,
                Password = this.PasswordTxtBox.Text,
                Token = responseData.Token,
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.AuthMethod, this.Credentials.Token);
            var userResponse = await client.GetAsync($"{Constants.ApiUrl}users?login={data.Login}");

            if (!userResponse.IsSuccessStatusCode)
            {
                MessageBox.Show("Problem with network...");
                return;
            }

            var user = await userResponse.Content.ReadFromJsonAsync<User>();
            this.Credentials.Id = user.Id;
            this.Credentials.Email = user.Email;

            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        class AuthenticationResponse
        {
            public string Token { get; set; }
            public DateTime Expiration { get; set; }
        }
    }
}
