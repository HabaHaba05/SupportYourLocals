using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SuppLocals
{
    public partial class MainWindow
    {
        // Methods for meniu interface
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sPan.Visibility == Visibility.Collapsed)
            {
                sPan.Visibility = Visibility.Visible;
                (sender as Button).Content = "☰";
            }
            else
            {
                sPan.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "☰";
            }
        }

        private void ServicesClick(object sender, RoutedEventArgs e)
        {
            if (servicePanel.Visibility == Visibility.Collapsed)
            {
                servicePanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Vendors";
            }
            else
            {
                servicePanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Vendors";
            }
        }

        private void FilterClick(object sender, RoutedEventArgs e)
        {
            if (filterPanel.Visibility == Visibility.Collapsed)
            {
                filterPanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Filters";
            }
            else
            {
                filterPanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Filters";
            }
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //true if the shell should be used when starting the process; false if the process should be created directly from the executable file.
            //The default is true on.NET Framework apps and false on.NET Core apps.
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        // Information about the app
        private void AboutButtonClicked(object sender, RoutedEventArgs e)
        {

            //ReviewsWindow rWindow = new ReviewsWindow();
            //rWindow.Show();
        }
    }
}
