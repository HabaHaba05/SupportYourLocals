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

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for AllVendors.xaml
    /// </summary>
    public partial class AllVendors : Window
    {
        private List<User> userList;
        private List<Vendor> vendorList;

        public AllVendors(string username)
        {
            InitializeComponent();
            GetData(username);
        }

        class vendor
        {
            public string title { get; set; }
            public string about { get; set; }
            public string adress { get; set; }
            public string vendorType { get; set; }
        }

        public void GetData(string username)
        { 
            using UsersDbTable db = new UsersDbTable();
            userList = db.Users.ToList();
            using VendorsDbTable db1 = new VendorsDbTable();
            vendorList = db1.Vendors.ToList();
            var user = userList.SingleOrDefault(x => (x.Username == username));
            foreach (var vendor in vendorList)
            {
                if (user.ID == vendor.UserID)
                {
                    listView.Items.Add(new vendor { title = vendor.Title, about = vendor.About, adress = vendor.Address, vendorType = vendor.VendorType });
                }
            }
        }
    }
}
