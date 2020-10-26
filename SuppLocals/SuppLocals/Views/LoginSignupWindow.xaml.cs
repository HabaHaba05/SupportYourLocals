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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataContext = new LoginVM();
        }
    }
}
