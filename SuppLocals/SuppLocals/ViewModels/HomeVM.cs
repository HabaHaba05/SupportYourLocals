using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;

namespace SuppLocals.ViewModels
{
    public class HomeVM : BaseViewModel
    {
        public readonly User ActiveUser;
        private readonly List<Vendor> _allVendorsList;
        private readonly Window _mainWindow;


        #region Constructor

        public HomeVM(Window window, User user)
        {
            _mainWindow = window;
            ActiveUser = user;

            using (var db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                _allVendorsList = new List<Vendor>(data);
                _allVendorsList.ForEach(x => x.Location = new Location(x.Latitude, x.Longitude));
                VendorsList = _allVendorsList;
            }

            FindRouteBtnClick = new RelayCommand(o => CalcRoute(), o => true);
            HideBtnClick = new RelayCommand(o => ResetToNulls(), o => true);

            ReviewBtnClick = new RelayCommand(o =>
                {
                    var reviewsWindow = new ReviewsWindow(_selectedVendor, user);
                    reviewsWindow.ShowDialog();
                },
                o => true
            );
        }

        #endregion


        public RelayCommand FindRouteBtnClick { get; }

        public RelayCommand HideBtnClick { get; }

        public RelayCommand ReviewBtnClick { get; }


        #region Private props

        private List<Vendor> _vendorList;

        private Vendor _selectedVendor;

        private string _vendorTypeSelected;

        private bool _useDistanceFilter;

        private double _circleRadius;

        private LocationCollection _routeLine;

        private Visibility _selectedVendorInfoGrid = Visibility.Collapsed;

        private double _zoomLevel = 12;

        #endregion

        #region Public props

        public double CircleRadius
        {
            get => _circleRadius;
            set
            {
                _circleRadius = value;
                ResetToNulls();
                UpdateVendorsList();
                NotifyPropertyChanged("CircleRadius");
            }
        }

        public List<Vendor> VendorsList
        {
            get => _vendorList;
            set
            {
                _vendorList = value;
                NotifyPropertyChanged("VendorsList");
            }
        }

        public Vendor SelectedVendor
        {
            get => _selectedVendor;
            set
            {
                _selectedVendor = value;
                RouteLine = null;
                NotifyPropertyChanged("SelectedVendor");
            }
        }

        public LocationCollection RouteLine
        {
            get => _routeLine;
            set
            {
                _routeLine = value;
                ZoomLevel += 0.01;
                NotifyPropertyChanged("RouteLine");
                ZoomLevel -= 0.01;
            }
        }

        public List<string> VendorTypesList
        {
            get
            {
                var x = new List<string>
                {
                    "ALL"
                };

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
            get => _vendorTypeSelected;
            set
            {
                _vendorTypeSelected = value;
                ResetToNulls();
                UpdateVendorsList();
                NotifyPropertyChanged("VendorTypeSelected");
            }
        }

        public bool UseDistanceFilter
        {
            get => _useDistanceFilter;
            set
            {
                _useDistanceFilter = value;
                ResetToNulls();
                UserDistanceFilterChange();
                NotifyPropertyChanged("UseDistanceFilter");
            }
        }

        public Visibility SelectedVendorInfoGrid
        {
            get => _selectedVendorInfoGrid;
            set
            {
                _selectedVendorInfoGrid = value;
                NotifyPropertyChanged("SelectedVendorInfoGrid");
            }
        }

        public double ZoomLevel
        {
            get => _zoomLevel;
            set
            {
                _zoomLevel = value;
                NotifyPropertyChanged("ZoomLevel");
            }
        }

        #endregion

        #region Methods

        private async void UserDistanceFilterChange()
        {
            if (_useDistanceFilter)
            {
                await GetLiveLocation();
            }

            UpdateVendorsList();
        }

        private void UpdateVendorsList()
        {
            if (VendorTypeSelected == "ALL")
            {
                VendorsList = _allVendorsList.Where(x =>
                    !_useDistanceFilter || MapMethods.DistanceBetweenPlaces(ActiveUser.Location, x.Location) <=
                    CircleRadius).ToList();
            }
            else
            {
                VendorsList = _allVendorsList.Where(x =>
                    x.VendorType == VendorTypeSelected && (!_useDistanceFilter ||
                                                           MapMethods.DistanceBetweenPlaces(ActiveUser.Location,
                                                               x.Location) <= CircleRadius)).ToList();
            }
        }

        private async Task GetLiveLocation()
        {
            ActiveUser.Location ??= await MapMethods.GetLiveLocation(_mainWindow);
        }

        private async void CalcRoute()
        {
            await GetLiveLocation();
            RouteLine = MapMethods.GetRoute(ActiveUser.Location, _selectedVendor.Location);
        }

        private void ResetToNulls()
        {
            SelectedVendorInfoGrid = Visibility.Collapsed;
            RouteLine = null;
            SelectedVendor = null;
        }

        #endregion
    }
}