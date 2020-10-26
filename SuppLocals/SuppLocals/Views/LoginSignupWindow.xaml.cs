using SuppLocals.Utilities.Helpers;
using SuppLocals.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SuppLocals.Views
{
    public partial class LoginSignupWindow : Window
    {
        public LoginSignupWindow()
        {
            InitializeComponent();
            DataContext = new LoginVM();
            CloseWindow.WinObject = (Window)this;

        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            DataContext = new LoginVM();
            Labeliukas.Visibility = Visibility.Visible;
        }

        private void SignupClick(object sender, RoutedEventArgs e)
        {
            DataContext = new SignupVM();
            Labeliukas.Visibility = Visibility.Hidden;
            
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            var start = new StartWindow();
            start.Show();
            this.Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var forgotPassword = new ForgotPassword();
            forgotPassword.Show();
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
