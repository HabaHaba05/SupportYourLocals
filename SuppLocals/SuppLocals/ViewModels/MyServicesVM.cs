using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SuppLocals.ViewModels
{
    public class MyServicesVM : BaseViewModel
    {
        ObservableCollection<Vendor> vendorList;
        public string about;
        public string title;
        private EditVendor editVendor;
        public Vendor tempVendor;

        public MyServicesVM(User user)
        {
            VendorList = new ObservableCollection<Vendor>();

            using (var db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                foreach (var vendor in data.Where(vendor => vendor.UserID == user.ID))
                {
                    VendorList.Add(vendor);
                }
            }
            ButtonCommand = new RelayCommand(new Action<object>(MainButtonClick));
        }

        public ObservableCollection<Vendor> VendorList
        {
            get
            {
                return vendorList;
            }
            set
            {
                NotifyPropertyChanged("VendorList");
            }
        }
        public ICommand ButtonCommand { get; private set; }

        private void MainButtonClick(object sender)
        {
            
            Vendor vendor = sender as Vendor;
            tempVendor = vendor;
            editVendor = new EditVendor(vendor);
            editVendor.Show();
            editVendor.SaveBtn.Click += new RoutedEventHandler(buttonClick);

        }

        private void buttonClick(object sender,EventArgs e)
        {
            title = editVendor.titleBox.Text;
            about = editVendor.aboutBox.Text;
            var temp = new ObservableCollection<Vendor>();
            for(int i = 0; i<VendorList.Count; i++)
            {
                if(VendorList[i].ID != tempVendor.ID)
                {
                    temp.Add(VendorList[i]);
                }
                else
                {
                    tempVendor.Title = title;
                    tempVendor.About = about;
                    temp.Add(tempVendor);
                }
            }

            using (VendorsDbTable db = new VendorsDbTable())
            {
                var vendor = db.Vendors.SingleOrDefault(x => x.ID == tempVendor.ID);
                vendor.Title = title;
                vendor.About = about;
                db.SaveChanges();
            }
            VendorList = temp;
        }

    }
}