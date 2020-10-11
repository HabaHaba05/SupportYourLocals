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
        private readonly User currUser;

        public AddService(User user)
        {
            InitializeComponent();

            currUser = user;

            TypeCB.ItemsSource = Enum.GetValues(typeof(VendorType));
            TypeCB.SelectedIndex = 0;

            SelectLocMap.CredentialsProvider = Config.BING_API_KEY;
        }

        public AddService()
        {

        }

        #region Buttons

        private async void CreateBtnClick(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TitleTB.Text) || !String.IsNullOrEmpty(AboutTB.Text) || !String.IsNullOrEmpty(AddressTB.Text))
            {
                Location location = await MapMethods.ConvertAddressToLocation(AddressTB.Text);
                if (location.Longitude == 0)
                {
                    MessageBox.Show("We cant find an address");
                    return;
                }

                using (VendorsDbTable db = new VendorsDbTable())
                {
                    Vendor vendor = new Vendor()
                    {
                        Title = TitleTB.Text,
                        About = AboutTB.Text,
                        Address = AddressTB.Text,
                        VendorType = TypeCB.Text,
                        Latitude = location.Latitude,
                        Longitude = location.Longitude,
                        UserID = currUser.ID
                    };

                    using (UsersDbTable dbUser = new UsersDbTable())
                    {
                        var user = dbUser.Users.SingleOrDefault(x => x.ID == currUser.ID);
                        user.VendorsCount++;
                        dbUser.SaveChanges();
                    }

                    db.Vendors.Add(vendor);
                    db.SaveChanges();
                }

                MessageBox.Show("Created");
                TitleTB.Text = "";
                AboutTB.Text = "";
                AddressTB.Text = "";

                return;
            }

            MessageBox.Show("All fields are required");
        }

        private async void UpdatePinLocation(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(AddressTB.Text))
            {
                Location location = await MapMethods.ConvertAddressToLocation(AddressTB.Text);
                if (location == null)
                {
                    MessageBox.Show("We cant find an address");
                    return;
                }
                SelectLocMap.Center = location;
            }
            ResultStack.Children.Clear();
        }
        #endregion

        #region Autocomplete address

        private void AddItem(string text)
        {
            TextBlock block = new TextBlock
            {

                // Add the text   
                Text = text,

                // A little style...   
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                AddressTB.Text = (sender as TextBlock).Text;
                UpdatePinLocation(null, null);
                ResultStack.Children.Clear();
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            ResultStack.Children.Add(block);
        }

        private async void AddressChanged(object sender, KeyEventArgs e)
        {
            string query = (sender as TextBox).Text;
            if (String.IsNullOrEmpty(query))
            {
                return;
            };

            var data = await AutoComplete.GetData(query);

            if (query.Length == 0)
            {
                ResultStack.Children.Clear();
            }

            // Clear the list   
            ResultStack.Children.Clear();

            // Add the result   
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj);
                }
            }
        }


        #endregion

        #region Buttons on map

        private void Plus_Click(object sender, RoutedEventArgs e)
        {
            SelectLocMap.ZoomLevel = Math.Min(SelectLocMap.ZoomLevel + 0.5, 17);
        }

        private void Minus_Click(object sender, RoutedEventArgs e)
        {
            SelectLocMap.ZoomLevel = Math.Max(SelectLocMap.ZoomLevel - 0.5, 3);
        }

        private async void ConfirmLoc_Click(object sender, RoutedEventArgs e)
        {
            string address = await MapMethods.ConvertLocationToAddress(SelectLocMap.Center);
            if (String.IsNullOrEmpty(address))
            {
                AddressTB.Text = "";
                MessageBox.Show("Sorry, we cant get address of this position");
                return;
            }
            AddressTB.Text = address;
        }

        #endregion

        #region Disable zoom with mouse or double click
        private void SelectLocMap_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void SelectLocMap_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion

    }
}
