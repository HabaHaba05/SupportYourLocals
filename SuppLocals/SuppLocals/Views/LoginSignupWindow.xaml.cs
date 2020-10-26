using SuppLocals.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SuppLocals.Views
{
    public partial class LoginSignupWindow : Window
    {
        public LoginSignupWindow()
        {
            InitializeComponent();
            DataContext = new LoginVM();

        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            DataContext = new LoginVM();
        }

        private void SignupClick(object sender, RoutedEventArgs e)
        {
            DataContext = new SignupVM();
            
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            var start = new StartWindow();
            start.Show();
            this.Close();
        }
    }
}
