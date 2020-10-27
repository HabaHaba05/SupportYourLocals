using System;
using System.Windows;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for EditVendor.xaml
    /// </summary>
    public partial class EditVendor : Window
    {
        public EditVendor(Vendor vendor)
        {
            InitializeComponent();
            SelectLocMap.CredentialsProvider = Config.Bing_Api_Key;
            titleBox.Text = vendor.Title;
            aboutBox.Text = vendor.About;
            addressBox.Text = vendor.Address;
            SelectLocMap.Center = new Location(vendor.Latitude, vendor.Longitude);
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void ConfirmLoc_Click(object sender, RoutedEventArgs e)
        {
            addressLbl.Content = "New address:";
            var address = await MapMethods.ConvertLocationToAddressAsync(SelectLocMap.Center);
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Sorry, we cant get an address of this place");
                return;
            }
            addressBox.Text = address;
        }
    }
}
