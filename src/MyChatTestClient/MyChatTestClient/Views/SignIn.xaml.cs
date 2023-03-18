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
using Sulimov.MyChat.Client.ViewModels;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        private UserViewModel userViewModel;

        public SignIn(UserViewModel userViewModel)
        {
            InitializeComponent();
            this.userViewModel = userViewModel;
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

            var result = await userViewModel.Login(this.LoginTxtBox.Text, this.PasswordTxtBox.Text);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message);
            }

            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
