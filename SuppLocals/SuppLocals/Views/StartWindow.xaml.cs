using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {

        public StartWindow()
        {
            InitializeComponent();   
        }

        private void BuyClicked(object sender, RoutedEventArgs e)
        {
            StartWindow sWindow = this;
            MainWindow mWindow = new MainWindow();
            mWindow.Show();
            sWindow.Close();
        }

        private void JoinClicked(object sender, RoutedEventArgs e)
        {
            StartWindow sWindow = this;
            Login lWindow = new Login();
            lWindow.Show();
            sWindow.Close();
        }
    }
}
