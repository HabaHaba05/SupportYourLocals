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
        private EditVendor editVendor;
        public Vendor tempVendor;

        #region Public props

        public ObservableCollection<Vendor> VendorList
        {
            get
            {
                return vendorList;
            }
            set
            {
                vendorList = value;
                NotifyPropertyChanged("VendorList");
            }
        }

        #endregion

        #region Commands

        public ICommand ButtonCommand { get; private set; }

        public ICommand DeleteButtonCommand { get; private set; }

        #endregion

        #region Constructor

        public MyServicesVM(User user)
        {
            VendorList = new ObservableCollection<Vendor>();
            GetData(user.ID);
            ButtonCommand = new RelayCommand(new Action<object>(EditButtonClick));
            DeleteButtonCommand = new RelayCommand(new Action<object>(DeleteButtonClick));
        }

        #endregion

        #region Methods

        private void GetData(int userID)
        {
            using (var db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                foreach (var vendor in data.Where(vendor => vendor.UserID == userID))
                {
                    VendorList.Add(vendor);
                }
            }
        }

        private void DeleteButtonClick(object sender)
        {
            Vendor vendor = sender as Vendor;
            using (VendorsDbTable db = new VendorsDbTable())
            {
                var vendorFromDB = db.Vendors.SingleOrDefault(x => x.ID == vendor.ID);
                db.Remove(vendorFromDB);
                db.SaveChanges();
            }
            using (UsersDbTable dbUser = new UsersDbTable())
            {
                var user = dbUser.Users.SingleOrDefault(x => x.ID == vendor.ID);
                user.VendorsCount--;
                dbUser.SaveChanges();
            }
            var itemToRemove = VendorList.Single(d => d.ID == vendor.ID);
            VendorList.Remove(itemToRemove);
        }

        private void EditButtonClick(object sender)
        {
            
            Vendor vendor = sender as Vendor;
            tempVendor = vendor;
            editVendor = new EditVendor(vendor);
            editVendor.Show();
            editVendor.SaveBtn.Click += new RoutedEventHandler(writeToDatabase);

        }

        private async void writeToDatabase(object sender,EventArgs e)
        {
            var point = await MapMethods.ConvertAddressToLocation(editVendor.addressBox.Text);
            var temp = new ObservableCollection<Vendor>();

            for(int i = 0; i<VendorList.Count; i++)
            {
                if(VendorList[i].ID != tempVendor.ID)
                {
                    temp.Add(VendorList[i]);
                }
                else
                {
                    tempVendor.Title = editVendor.titleBox.Text;
                    tempVendor.About = editVendor.aboutBox.Text;
                    tempVendor.Address = editVendor.addressBox.Text;
                    tempVendor.Latitude = Double.Parse(point[0]);
                    tempVendor.Longitude = Double.Parse(point[1]);
                    tempVendor.Municipality = point[2];
                    tempVendor.County = point[3];
                    temp.Add(tempVendor);
                }
            }

            using (VendorsDbTable db = new VendorsDbTable())
            {
                var vendor = db.Vendors.SingleOrDefault(x => x.ID == tempVendor.ID);
                vendor.Title = editVendor.titleBox.Text;
                vendor.About = editVendor.aboutBox.Text;
                vendor.Address = editVendor.addressBox.Text;
                vendor.Latitude = Double.Parse(point[0]);
                vendor.Longitude = Double.Parse(point[1]);
                vendor.Municipality = point[2];
                vendor.County = point[3];
                db.SaveChanges();
            }
            VendorList = temp;
        }

        #endregion

    }
}