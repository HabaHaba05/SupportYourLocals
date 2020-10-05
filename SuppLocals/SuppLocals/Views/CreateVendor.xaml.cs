using Geocoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for CreateVendor.xaml
    /// </summary>
    public partial class CreateVendor : Window
    {
        private readonly User currUser;
        public CreateVendor(User user)
        {
            currUser = user;
            InitializeComponent();

           
            TypeCB.ItemsSource = Enum.GetValues(typeof(VendorType));
            TypeCB.SelectedIndex = 0;
        }

        private async void CreateBtnClick(object sender, RoutedEventArgs e)
        {
            if(!String.IsNullOrEmpty(TitleTB.Text) || !String.IsNullOrEmpty(AboutTB.Text) || !String.IsNullOrEmpty(AddressTB.Text))
            {
                IEnumerable<Address> addresses = await MapMethods.ConvertAddressToLocation(AddressTB.Text);
                if(addresses.Count() == 0)
                {
                    MessageBox.Show("Sorry, we can't find that location");
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
                        Latitude = addresses.First().Coordinates.Latitude,
                        Longitude = addresses.First().Coordinates.Longitude,
                        UserID = currUser.ID
                    };

                    using (UsersDbTable dbUser = new UsersDbTable())
                    {
                        var user = dbUser.Users.SingleOrDefault(x => x.ID == currUser.ID);
                        user.VendorsCount++;
                    }

                    db.Vendors.Add(vendor);

                    db.SaveChanges();
                }

                this.Close();
                return;
            }

            MessageBox.Show("All fields are required");
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
