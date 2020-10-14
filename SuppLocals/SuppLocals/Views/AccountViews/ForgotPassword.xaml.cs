using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuppLocals.Views
{
    public partial class ForgotPassword : Window
    {
        public ForgotPassword()
        {
            InitializeComponent();
            DataContext = new ValidateUsername();
        }

        public void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            var email = emailForForgotPassword.Text;
            var code = GenerateRandomString();

            using var db = new UsersDbTable();
            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => x.Email == email);

            if (user == null)
            {
                MessageBox.Show("Invalid Email");
                return;
            }

            var emailSender = new EmailSender();
            emailSender.SendEmail(email, code);


            var codeInput = new CodeInput(user, email, code);
            codeInput.Show();

            Close();
        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            var login = new Login();
            login.Show();
            Close();
        }


        private void EmailTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(emailForForgotPassword.Text))
            {
                sendBtn.IsEnabled = false;
            }
            else
            {
                sendBtn.IsEnabled = true;
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        public string GenerateRandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
    }
}