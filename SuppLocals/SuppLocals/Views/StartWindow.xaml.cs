using System.Windows;

namespace SuppLocals.Views
{
    /// <summary>
    ///     Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void BuyClicked(object sender, RoutedEventArgs e)
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

        private void JoinClicked(object sender, RoutedEventArgs e)
        {
            var lWindow = new Login();
            lWindow.Show();
            Close();
        }
    }
}