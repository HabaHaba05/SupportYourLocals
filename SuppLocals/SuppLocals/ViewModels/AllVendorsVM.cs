using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SuppLocals.ViewModels
{
    public class AllVendorsVM: BaseViewModel
    {
        private List<Vendor> vendorList;
        private List<User> userList;

        public List<Vendor> VendorList
        {
            get
            {
                return vendorList;
            }
        }

        public AllVendorsVM(string username)
        {
            MessageBox.Show(username);
            vendorList = new List<Vendor>();
            userList = new List<User>();
            using UsersDbTable db = new UsersDbTable();
            userList = db.Users.ToList();
            using VendorsDbTable db1 = new VendorsDbTable();
            vendorList = db1.Vendors.ToList();
            var user = userList.SingleOrDefault(x => (x.Username == username));
            foreach (var vendor in vendorList)
            {
                if (user.ID == vendor.UserID)
                {
                    vendorList.Add(vendor);
                }
            }
        }
    }
}
