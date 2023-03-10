using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Newtonsoft.Json;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private async void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.LoginTxtBox.Text))
            {
                MessageBox.Show("Please type login...");
                return;
            }

            if (string.IsNullOrEmpty(this.EmailTxtBox.Text))
            {
                MessageBox.Show("Please type e-mail...");
                return;
            }

            if (string.IsNullOrEmpty(this.PasswordTxtBox.Text))
            {
                MessageBox.Show("Please type password...");
                return;
            }

            if (this.PasswordTxtBox.Text != this.PasswordConfirmTxtBox.Text)
            {
                MessageBox.Show("Please confirm the password...");
                return;
            }

            var data = new
            {
                Name = this.LoginTxtBox.Text,
                Email = this.EmailTxtBox.Text,
                Password = this.PasswordTxtBox.Text,
            };

            using var client = new HttpClient();

            var response = await client.PostAsJsonAsync(Constants.ApiUrl + "users", data);

            if (response.StatusCode != HttpStatusCode.Created)
            {
                MessageBox.Show($"User {this.LoginTxtBox.Text} already exists.");
                return;
            }

            MessageBox.Show($"User {this.LoginTxtBox.Text} successfull registered.");
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
