using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.ViewModels
{
    public class MyServicesVM : BaseViewModel
    {
        private readonly User user;
        private List<Vendor> vendorList;

        public List<Vendor> VendorList
        {
            get
            {
                return vendorList;
            }
        }

        public MyServicesVM(User user)
        {
            this.user = user;
            vendorList = new List<Vendor>();
            using (VendorsDbTable db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                foreach(var vendor in data)
                {
                    if(vendor.UserID == user.ID)
                    {
                        vendorList.Add(vendor);
                    }
                }
            }
        }
    }
}
