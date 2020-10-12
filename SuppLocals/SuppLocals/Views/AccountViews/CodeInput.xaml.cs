﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuppLocals.Views
{
    public partial class CodeInput : Window
    {
        public User ActiveUser;


        public CodeInput(User user)
        {
            ActiveUser = user;
            InitializeComponent();
        }

        public void Apply_Button_Click(object sender, RoutedEventArgs e)
        {
            EmailSender emailSender = new EmailSender();


            if ("" == CodeTextBox.Text)
            {

                MainWindow map = new MainWindow(ActiveUser);
                map.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("");
            }

        }

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

    }
}
