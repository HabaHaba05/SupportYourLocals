using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.Services.Maps;
using System.Runtime.Loader;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for EditVendor.xaml
    /// </summary>
    public partial class EditVendor : Window
    {
        private Location _myMapCenter = new Location(54.68, 25.27);
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
            var address = await MapMethods.ConvertLocationToAddress(SelectLocMap.Center);
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Sorry, we cant get an address of this place");
                return;
            }
            addressBox.Text = address;
        }
    }
}
