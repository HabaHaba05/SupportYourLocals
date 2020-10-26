using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Windows.UI.Xaml;

namespace SuppLocals.ViewModels
{
    public class AllVendorsVM: BaseViewModel

    {
        ObservableCollection<Vendor> _vendorList;

        #region Public props

        public ObservableCollection<Vendor> VendorsList
        {
            get { return _vendorList; }
            set
            {
                _vendorList = value;
                NotifyPropertyChanged("VendorsList");
            }
        }

        #endregion

        #region Constructor

        public AllVendorsVM(string username)
        {
            VendorsList = new ObservableCollection<Vendor>();
            GetData(username);
        }

        #endregion

        #region Methods

        private void GetData(string username)
        {
            User user;
            using(var usersDB = new UsersDbTable())
            {
                var userList = usersDB.Users.ToList();
                user = userList.FirstOrDefault(x => x.Username == username);
                using (var vendorsDB = new VendorsDbTable())
                {
                    var vendorList = vendorsDB.Vendors.ToList();
                    foreach (var vendor in vendorList.Where(x => x.UserID == user.ID))
                    {
                        VendorsList.Add(vendor);
                    }
                }
            }
        }

        #endregion
    }
}
