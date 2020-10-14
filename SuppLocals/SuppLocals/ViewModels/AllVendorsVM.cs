using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.ViewModels
{
    public class AllVendorsVM: BaseViewModel
    {
        public AllVendorsVM(string username)
        {
            MessageBox.Show(username);

            using var db = new UsersDbTable();
            var userList = db.Users.ToList();

            using var db1 = new VendorsDbTable();
            var vendorList = db1.Vendors.ToList();

            var user = userList.SingleOrDefault(x => (x.Username == username));

            foreach (var vendor in vendorList.Where(vendor => user.ID == vendor.UserID))
            {
                vendorList.Add(vendor);
            }
        }
    }
}
