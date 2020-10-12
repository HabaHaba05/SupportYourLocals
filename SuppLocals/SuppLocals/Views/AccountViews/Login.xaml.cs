using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            //CheckifTextboxisEmpty();
            this.DataContext = new ValidateUsername();
        }

        public void LogInBtnClick(object sender, RoutedEventArgs e)
        {
            using UsersDbTable db = new UsersDbTable();
            var username = Username.Text;
            string password = PasswordBox.Password.ToString();

            var usersList = db.Users.ToList();
            var user = usersList.SingleOrDefault(x => (x.Username == username & BC.Verify(password, x.HashedPsw)));
            if (user == null)
            {
                MessageBox.Show("Invalid credentials");
                return;
            }

            MainWindow map = new MainWindow(user);
            map.Show();
            this.Close();
        }

        private void SignUpBtnClick(object sender, RoutedEventArgs e)
        {
            SignUp signUpForm = new SignUp();
            signUpForm.Show();
            this.Close();
        }
      
        private void GoToMapClick(object sender, RoutedEventArgs e)
        {
            User user = new User
            {
                Username = "Anonimas",
                VendorsCount = 0,
                HashedPsw = ""
            };

            MainWindow map = new MainWindow(user);
            map.Show();
            this.Close();
        }
        
        // Method which allow user drag window aroud their screen
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void UsernameTextChangedEventHandler(object sender, TextChangedEventArgs args)
        {
            if (string.IsNullOrWhiteSpace(Username.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password.ToString()))
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
            if (string.IsNullOrWhiteSpace(PasswordBox.Password.ToString()) || string.IsNullOrWhiteSpace(Username.Text))
            {
                loginBtn.IsEnabled = false;
            }
            else
            {
                loginBtn.IsEnabled = true;
            }
        }



        private void Hyperlinky_Click(object sender, RoutedEventArgs e)
        {
            ForgotPassword forgotPassword = new ForgotPassword();
            forgotPassword.Show();
            Close();
        }

    }
}
