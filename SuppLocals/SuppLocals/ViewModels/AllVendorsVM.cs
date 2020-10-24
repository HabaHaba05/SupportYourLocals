using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.ViewModels
{
    public class AllVendorsVM: ObservableObject
    {
        public AllVendorsVM(string username)
        {
            MessageBox.Show(username);

            using var db = new AppDbContext();
            var userList = db.Users.ToList();
            var vendorList = db.Vendors.ToList();

            var user = userList.SingleOrDefault(x => (x.Username == username));

            foreach (var vendor in vendorList.Where(vendor => user.ID == vendor.UserID))
            {
                vendorList.Add(vendor);
            }
        }
    }
}
