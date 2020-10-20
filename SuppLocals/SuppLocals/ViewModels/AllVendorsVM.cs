using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Windows.UI.Xaml;

namespace SuppLocals.ViewModels
{
    public class AllVendorsVM: BaseViewModel, INotifyPropertyChanged

    {
        ObservableCollection<Vendor> _vendorList;
        private List<User> _userList;
        public event PropertyChangedEventHandler PropertyChanged;

        public AllVendorsVM(string username)
        {
            VendorsList = new ObservableCollection<Vendor>();
            int ID = -1;
            using var db = new UsersDbTable();
            var userList = db.Users.ToList();

            using var db1 = new VendorsDbTable();
            var vendorList = db1.Vendors.ToList();

            foreach(var v in userList.Where(x => x.Username == username))
            {
                ID = v.ID;
            }
            foreach(var v in vendorList.Where(x => x.UserID == ID))
            {
                VendorsList.Add(v);
            }




        }

        public ObservableCollection<Vendor> VendorsList
        {
            get { return _vendorList; }
            set
            {
                _vendorList = value;
                if(PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("VendorsList"));
                }
            }
        }
    }
}
