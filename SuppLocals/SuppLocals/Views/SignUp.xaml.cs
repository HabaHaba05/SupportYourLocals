
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using BC = BCrypt.Net.BCrypt;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
     
        public SignUp()
        {
            InitializeComponent();
            var userList = getList();
            /*UsernameTextBox.TextChanged += delegate (object sender, TextChangedEventArgs args)
            {
                usernameUsingCheck(sender, args, userList);
            };*/

        }

        private void BackButtonClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void SignUp_ButtonClick(object sender, RoutedEventArgs e)
        {

            var username = UsernameTextBox.Text;
            var password = PasswordBox1.Password;
            var repeatPassword = ConfirmPasswordBox1.Password;

            if(password != repeatPassword)
            {
                MessageBox.Show("Passwords do not match");
                return;
            }

            using (AppDbContext db = new AppDbContext())
            {
                var usersList = db.Users.ToList();
                if (usersList.FirstOrDefault(x => x.Username == username) != null || username == "Anonimas")
                {
                    MessageBox.Show("Sorry, but username is already taken");
                    return;
                }

                User newUser = new User()
                {
                    Username = username,
                    HashedPsw = BC.HashPassword(password),
                    VendorsCount = 0
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                MainWindow mainWindow = new MainWindow(newUser);
                mainWindow.Show();
                this.Close();
            }
        }

        // Method which allow user drag window aroud their screen
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void usernameUsingCheck(object sender, TextChangedEventArgs args, List<User> userList)
        {
            var username = UsernameTextBox.Text;
            if (userList.FirstOrDefault(x => x.Username == username) != null || username == "Anonimas")
            {
                usernameUsingLabel.Content = "Username is already taken";
            }
            else
            {
                usernameUsingLabel.Content = "";
            }
        }

        private List<User> getList()
        {
            var userList = new List<User>();
            using (AppDbContext db = new AppDbContext())
            {
                userList = db.Users.ToList();
            }
            return userList;
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox1.Password.ToString()) || string.IsNullOrWhiteSpace(ConfirmPasswordBox1.Password.ToString())
                || UsernameTextBox.Text.Length < 5 || UsernameTextBox.Text.Length > 12)
            {
                applyBtn.IsEnabled = false;
            }
            else
            {
                applyBtn.IsEnabled = true;
            }
        }

        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) || string.IsNullOrWhiteSpace(PasswordBox1.Password.ToString()) || string.IsNullOrWhiteSpace(ConfirmPasswordBox1.Password.ToString())
                ||UsernameTextBox.Text.Length < 5 || UsernameTextBox.Text.Length > 12 )
            {
                applyBtn.IsEnabled = false;
            }
            else
            {
                applyBtn.IsEnabled = true;
            }
        }
    }

} 
