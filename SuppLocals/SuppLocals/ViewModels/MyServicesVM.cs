using System.Collections.Generic;
using System.Linq;

namespace SuppLocals.ViewModels
{
    public class MyServicesVM : ObservableObject
    {

        public MyServicesVM(User user)
        {
            VendorList = new List<Vendor>();

            using (var db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                foreach (var vendor in data.Where(vendor => vendor.UserID == user.ID))
                {
                    VendorList.Add(vendor);
                }
            }
        }

        public List<Vendor> VendorList { get; }

    }
}