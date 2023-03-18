using Sulimov.MyChat.Client.ViewModels;
using System.Windows;

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
