using System;
using System.Windows;
using System.Windows.Controls;

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

            SelectLocMap.CredentialsProvider = Config.Bing_Api_Key;

            TypeCB.ItemsSource = Enum.GetValues(typeof(VendorType));
            TypeCB.SelectedIndex = 0;
        }

        private async void ConfirmLoc_Click(object sender, RoutedEventArgs e)
        {
            var address = await MapMethods.ConvertLocationToAddress(SelectLocMap.Center);
            if (string.IsNullOrEmpty(address))
            {
                MessageBox.Show("Sorry, we cant get an address of this place");
                return;
            }

            AddressTB.Text = address;
        }
    }
}