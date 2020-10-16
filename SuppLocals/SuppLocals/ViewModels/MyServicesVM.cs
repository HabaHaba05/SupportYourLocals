using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SuppLocals.ViewModels
{
    public class MyServicesVM : BaseViewModel
    {

        public List<Vendor> VendorList
        {
            get;
        }

        public MyServicesVM(User user)
        {
            VendorList = new List<Vendor>();
            using (VendorsDbTable db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                foreach(var vendor in data)
                {
                    if(vendor.UserID == user.ID)
                    {
                        VendorList.Add(vendor);
                    }
                }
            }
        }
    }
}
