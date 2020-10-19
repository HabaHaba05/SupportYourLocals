using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for AllVendors.xaml
    /// </summary>
    public partial class AllVendors : Window
    {
        private List<User> _userList;
        private List<SuppLocals.Vendor> _vendorList;

        public AllVendors(string username)
        {
            InitializeComponent();
            GetData(username);
        }

        private class Vendor
        {
            public string Title { get; set; }
            public string About { get; set; }
            public string Address { get; set; }
            public string VendorType { get; set; }
        }

        public void GetData(string username)
        { 
            using var db = new UsersDbTable();
            _userList = db.Users.ToList();

            using var db1 = new VendorsDbTable();
            _vendorList = db1.Vendors.ToList();

            var user = _userList.SingleOrDefault(x => (x.Username == username));
            foreach (var vendor in _vendorList.Where(vendor => user.ID == vendor.UserID))
            {
                listView.Items.Add(new Vendor { Title = vendor.Title, About = vendor.About, Address = vendor.Address, VendorType = vendor.VendorType });
            }
        }
    }
}
