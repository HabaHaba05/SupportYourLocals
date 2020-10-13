using System;
using System.Linq;
using System.Printing;
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
            this.DataContext = new ValidateUsername();
        }

        public void Send_Button_Click(object sender, RoutedEventArgs e)
        {
            var email = emailForForgotPassword.Text;
            var code = GenerateRandomString();

            using UsersDbTable db = new UsersDbTable();
            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => (x.Email == email));

            if (user == null)
            {
                MessageBox.Show("Invalid Email");
                return;
            }
            else
            {
                var emailSender = new EmailSender();
                emailSender.SendEmail(email, code);
            }


            CodeInput codeInput = new CodeInput(user, email, code);
            codeInput.Show();

            Close();

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
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
                DragMove();
        }

        public string GenerateRandomString()
        {

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            var finalString = new String(stringChars);
            return finalString;
        }


    }
}
