using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for AddService.xaml
    /// </summary>
    public partial class AddService : UserControl
    {
        public AddService()
        {
            InitializeComponent();

            SelectLocMap.CredentialsProvider = Config.BING_API_KEY;

            TypeCB.ItemsSource = Enum.GetValues(typeof(VendorType));
            TypeCB.SelectedIndex = 0;
        }

        private async void ConfirmLoc_Click(object sender, RoutedEventArgs e)
        {
            string address = await MapMethods.ConvertLocationToAddress(SelectLocMap.Center);
            if (String.IsNullOrEmpty(address))
            {
                MessageBox.Show("Sorry, we cant get an address of this place");
                return;
            }
            AddressTB.Text = address;
        }
       

    }
}
