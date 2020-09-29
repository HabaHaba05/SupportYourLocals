
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

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
            var path = @"..\LoginInfo.txt";
            var inUse = false;

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            else if (File.Exists(path))
            {
                if (username == "" || password == "")
                {
                    MessageBox.Show("Please enter your username or password");
                }
                if (password != repeatPassword)
                {
                    MessageBox.Show("Your password and confirmation password do not match");
                }
                if (username != "" && password != "" && repeatPassword != "" && password == repeatPassword)
                {
                    string[] lines = File.ReadAllLines(path);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        string[] line = lines[i].Split('`');
                        if (line[0] == username)
                        {
                            inUse = true;
                            MessageBox.Show("This username is already in use");
                            UsernameTextBox.Text = "";
                        }
                    }
                    if (inUse == false)
                    {
                        using (StreamWriter writeText = new StreamWriter(path, true))
                        {
                            writeText.WriteLine(username + "`" + password + "`noPhoto");
                        }
                        MessageBox.Show("User was registered");

                        Login login = new Login();
                        login.Show();
                        this.Close();
                    }
                }
            }
        }

        // Method which allow user drag window aroud their screen
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void UsernameTextBox_Error(object sender, System.Windows.Controls.ValidationErrorEventArgs e)
        {

        }
    }

} 
