﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private void SignUp_ButtonClick(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordBox.Password.ToString();
            var repeatPassword = ConfirmPasswordBox.Password.ToString();
            var path = @"..\LoginInfo.txt";
            var inUse = false;

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
            }
            else if (File.Exists(path))
            {
                if (username == "")
                {
                    MessageBox.Show("Please enter your username");
                }
                if (password == "")
                {
                    MessageBox.Show("Please enter your password");
                }
                if (repeatPassword == "")
                {
                    MessageBox.Show("Please enter your confirm password");
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
                            writeText.Write(username + "`" + password + Environment.NewLine);
                        }
                        MessageBox.Show("User was registered");
                        this.Close();
                    }
                }
            }

        }
    }
}
