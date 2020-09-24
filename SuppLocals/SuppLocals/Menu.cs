using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SuppLocals
{
    public partial class MainWindow
    {
        // Methods for meniu interface
        private void buttonClick(object sender, RoutedEventArgs e)
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

        private void servicesClick(object sender, RoutedEventArgs e)
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

        private void filterClick(object sender, RoutedEventArgs e)
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

        private void reviewsClick(object sender, RoutedEventArgs e)
        {
            if (reviewsPanel.Visibility == Visibility.Collapsed)
            {
                reviewsPanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Reviews";
            }
            else
            {
                reviewsPanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Reviews";
            }
        }

        private void MoreButtonClicked(object sender, RoutedEventArgs e)
        {
            ReviewsWindow rWindow = new ReviewsWindow();
            rWindow.Show();
        }


        private void hyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //true if the shell should be used when starting the process; false if the process should be created directly from the executable file.
            //The default is true on.NET Framework apps and false on.NET Core apps.
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

    }
}
