using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SuppLocals.ViewModels;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            //By default
            InitializeComponent();
            MyMap.CredentialsProvider = Config.BING_API_KEY;
        }

        private void Pushpin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var selectedVendor = (sender as FrameworkElement).DataContext as Vendor;
            SelectedServiceInfoGrid.Visibility = Visibility.Visible;
            var x = (HomeVM)this.DataContext;
            x.SelectedVendor = selectedVendor;
        }   


    }
}
