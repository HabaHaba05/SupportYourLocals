using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BC = BCrypt.Net.BCrypt;
using System;

namespace SuppLocals.Views
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            
        }
    }
}
        /*
        public void LogInBtnClick(object sender, RoutedEventArgs e)
        {
            using var db = new AppDbContext();
            var username = Username.Text;
            var password = PasswordBox.Password;

            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => (x.Username == username) & BC.Verify(password, x.HashedPsw));
            if (user == null)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            var map = new MainWindow(user);
            map.Show();
            Close();
        }

        private void SignUpBtnClick(object sender, RoutedEventArgs e)
        {
            var signUpForm = new SignUp();
            signUpForm.Show();
            Close();
        }

        private void GoToMapClick(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                Username = "Anonimas",
                VendorsCount = 0,
                HashedPsw = ""
            };

            var map = new MainWindow(user);
            map.Show();
            Close();
        }

        // Method which allow user drag window around their screen
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


        private void UsernameTextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                PasswordBox.Password.Length < 8)
            {
                loginBtn.IsEnabled = false;
            }
            else
            {
                loginBtn.IsEnabled = true;
            }
        }

        private void PasswordTextChangedEventHandler(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(PasswordBox.Password) || string.IsNullOrWhiteSpace(Username.Text) ||
                PasswordBox.Password.Length < 8)
            {
                loginBtn.IsEnabled = false;
            }
            else
            {
                loginBtn.IsEnabled = true;
            }
        }


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var forgotPassword = new ForgotPassword();
            forgotPassword.Show();
            this.Close();
        }
        */
