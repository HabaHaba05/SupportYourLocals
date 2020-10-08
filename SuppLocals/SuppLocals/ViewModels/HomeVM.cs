using Geocoding;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.ViewModels
{
    public class HomeVM : BaseViewModel
    {
        private readonly Window mainWindow;
        private readonly User user;
        private readonly List<Vendor> allVendorsList;


        #region Private props
        private List<Vendor> _vendorList;

        private Vendor _selectedVendor;

        private string _vendorTypeSelected;

        private bool _useDistanceFilter;

        private double _circleRadius;


        private LocationCollection _routeLine;

        private LocationCollection _circle;

        private Visibility _selectedVendorInfoGrid = Visibility.Collapsed;

        #endregion

        #region Public props

        public double CircleRadius
        {
            get { return _circleRadius; }
            set
            {
                _circleRadius = value;
                UpdateCircle();
                UpdateVendorsList();
                NotifyPropertyChanged("CircleRadius");
            }
        }

        public List<Vendor> VendorsList 
        {
            get 
            {
                return _vendorList;
            } 
            set 
            {
                _vendorList = value;
                NotifyPropertyChanged("VendorsList");
            }
        }

        public Vendor SelectedVendor
        {
            get { return _selectedVendor; }
            set
            {
                _selectedVendor = value;
                NotifyPropertyChanged("SelectedVendor");
            }
        }

        public LocationCollection RouteLine
        {
            get
            {
                return _routeLine;
            }
            set
            {
                _routeLine = value;
                NotifyPropertyChanged("RouteLine");
            }
        }

        public List<string> VendorTypesList 
        { 
            get 
            {
                List<string> x = new List<string>();
                x.Add("ALL");
                foreach (var type in Enum.GetValues(typeof(VendorType)))
                {
                    x.Add(type.ToString());
                }
                _vendorTypeSelected = x[0];
                return x;
            } 
        }

        public string VendorTypeSelected 
        {
            get
            {
                return _vendorTypeSelected;
            }
            set
            {
                _vendorTypeSelected = value;
                UpdateVendorsList();
                NotifyPropertyChanged("VendorTypeSelected");
            }
        }

        public bool UseDistanceFilter
        { 
            get { return _useDistanceFilter; } 
            set 
            {
                _useDistanceFilter = value;
                if (value)
                {
                    GetLiveLocation();
                }
                else
                {
                    Circle = null;
                    UpdateVendorsList();
                }
                
                NotifyPropertyChanged("UseDistanceFilter");
            }
        }

        public LocationCollection Circle
        {
            get { return _circle; }
            set
            {
                _circle = value;
                NotifyPropertyChanged("Circle");
            }
        }

        public Visibility SelectedVendorInfoGrid
        {
            get { return _selectedVendorInfoGrid; }
            set
            {
                _selectedVendorInfoGrid = value;
                NotifyPropertyChanged("SelectedVendorInfoGrid");
            }
        }

        #endregion

        #region Commands

        #region private

        private ICommand _findRoute;
        private ICommand _hideBtnClick;
        private ICommand _reviewBtnClick;
        private ICommand _pushPinClick;
        #endregion

        #region public

        public ICommand FindRoute
        {
            get
            {
                return _findRoute ??= new CommandHandler(() => 
                {
                    RouteLine = MapMethods.GetRoute(user.Location, _selectedVendor.Location);
                });
            }
        }    

        public ICommand HideBtnClick
        {
            get
            {
                return _hideBtnClick ??= new CommandHandler(() => { RouteLine = null; SelectedVendorInfoGrid = Visibility.Collapsed; });
            }
        }

        public ICommand ReviewBtnClick { get
            {
                return _reviewBtnClick ??= new CommandHandler(() => {
                    return;
                });
            } }

        public ICommand PushPinClick
        {
            get
            {
                return _pushPinClick ??= new CommandHandler(() =>
                {
                    SelectedVendorInfoGrid = Visibility.Visible;
                });
            }
        }

        #endregion
        #endregion

        public HomeVM(Window window, User user)
        {
            this.mainWindow = window;
            this.user = user;

            using (VendorsDbTable db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                allVendorsList = new List<Vendor>(data);
                allVendorsList.ForEach(x => x.Location = new Location(x.Latitude, x.Longitude));
                VendorsList = allVendorsList;
            }

        }


        #region Methods

        private void UpdateVendorsList()
        {
            if (VendorTypeSelected.ToString() == "ALL")
            {
                VendorsList = allVendorsList.Where(x => !_useDistanceFilter || MapMethods.DistanceBetweenPlaces(user.Location, x.Location) <= CircleRadius).ToList();
            }
            else
            {
                VendorsList = allVendorsList.Where(x =>
                        (x.VendorType == VendorTypeSelected.ToString()) && (!_useDistanceFilter || MapMethods.DistanceBetweenPlaces(user.Location, x.Location) <= CircleRadius)).ToList();
            }
        }

        private async void GetLiveLocation()
        {
            if(user.Location == null && _useDistanceFilter)
            {
                user.Location = await MapMethods.GetLiveLocation(mainWindow);
            }
            UpdateVendorsList();
        }

        private void UpdateCircle()
        {
            Circle = MapMethods.GetCircleVertices(user.Location, _circleRadius);
            
        }

        #endregion


    }
}
