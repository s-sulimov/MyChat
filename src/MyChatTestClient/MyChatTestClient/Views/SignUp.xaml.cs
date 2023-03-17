using Sulimov.MyChat.Client.ViewModels;
using System.Windows;

namespace Sulimov.MyChat.Client
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        private UserViewModel userViewModel;

        public SignUp(UserViewModel userViewModel)
        {
            InitializeComponent();
            this.userViewModel = userViewModel;
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

            var result = await userViewModel.Register(this.LoginTxtBox.Text, this.PasswordTxtBox.Text, this.EmailTxtBox.Text);
            if (!result.IsSuccess)
            {
                MessageBox.Show(result.Message);
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
