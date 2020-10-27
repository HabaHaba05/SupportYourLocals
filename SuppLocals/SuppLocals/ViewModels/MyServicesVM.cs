using SuppLocals.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SuppLocals.ViewModels
{
    public class MyServicesVM : ObservableObject
    {
        private EditVendor editVendor;
        private readonly User _user;
        public Vendor tempVendor;
        ObservableCollection<Vendor> vendorList;

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
            _user = user;
            VendorList = new ObservableCollection<Vendor>();
            GetData(user.ID);
            ButtonCommand = new RelayCommand(new Action<object>(EditButtonClick));
            DeleteButtonCommand = new RelayCommand(new Action<object>(PreviewDeleteButtonClick));
        }

        #endregion

        #region Methods

        private void GetData(int userID)
        {
            using var db = new AppDbContext();
            var data = db.Vendors.ToList();
            foreach (var vendor in data.Where(vendor => vendor.UserID == userID))
            {
                VendorList.Add(vendor);
            }
        }

        private void DeleteButtonClick(object sender)
        {
            var vendor = sender as Vendor;
            using (var db = new AppDbContext())
            {
                var vendorFromDB = db.Vendors.SingleOrDefault(x => x.ID == vendor.ID);
                db.Remove(vendorFromDB);
                var user = db.Users.SingleOrDefault(x => x.ID == _user.ID);
                user.VendorsCount--;
                var reviewList = db.Reviews.ToList();

                foreach(var review in reviewList.Where(x => x.VendorID == vendor.ID))
                {
                    db.Remove(review);
                }
                db.SaveChanges();
            }

            var itemToRemove = VendorList.Single(d => d.ID == vendor.ID);
            VendorList.Remove(itemToRemove);
        }

        private void PreviewDeleteButtonClick(object sender)
        {
            MessageBoxResult mBoxResult = MessageBox.Show("Are you sure you want to delete this?", "Delete Confirmation", MessageBoxButton.YesNo);
            if (mBoxResult == MessageBoxResult.Yes)
            {
                DeleteButtonClick(sender);
            }
            else
            {
                return;
            }
        }

        private void EditButtonClick(object sender)
        {
            var vendor = sender as Vendor;
            tempVendor = vendor;
            editVendor = new EditVendor(vendor);
            editVendor.Show();
            editVendor.SaveBtn.Click += WriteToDatabase;
        }

        private async void WriteToDatabase(object sender,EventArgs e)
        {
            var point = await MapMethods.ConvertAddressToLocationAsync(editVendor.addressBox.Text);
            var temp = new ObservableCollection<Vendor>();

            foreach(var vendor in VendorList)
            {
                if(vendor.ID != tempVendor.ID)
                {
                    temp.Add(vendor);
                }
                else
                {
                    tempVendor.Title = editVendor.titleBox.Text;
                    tempVendor.About = editVendor.aboutBox.Text;
                    tempVendor.Address = editVendor.addressBox.Text;
                    tempVendor.Latitude = double.Parse(point[0]);
                    tempVendor.Longitude = double.Parse(point[1]);
                    tempVendor.Municipality = point[2];
                    tempVendor.County = point[3];
                    temp.Add(tempVendor);
                }
            }

            using (var db = new AppDbContext())
            {
                var vendor = db.Vendors.SingleOrDefault(x => x.ID == tempVendor.ID);
                vendor.Title = editVendor.titleBox.Text;
                vendor.About = editVendor.aboutBox.Text;
                vendor.Address = editVendor.addressBox.Text;
                vendor.Latitude = double.Parse(point[0]);
                vendor.Longitude = double.Parse(point[1]);
                vendor.Municipality = point[2];
                vendor.County = point[3];
                db.SaveChanges();
            }
            VendorList = temp;
        }

        #endregion

    }
}