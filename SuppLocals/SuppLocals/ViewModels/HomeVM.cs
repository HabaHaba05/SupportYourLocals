using Microsoft.Maps.MapControl.WPF;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

        private ObservableCollection<Location> _circle;

        private Visibility _selectedVendorInfoGrid = Visibility.Collapsed;

        #endregion

        #region Public props

        public double CircleRadius
        {
            get { return _circleRadius; }
            set
            {
                _circleRadius = value;
                ResetToNulls();
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
                RouteLine = null;
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
                List<string> x = new List<string>
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
            get
            {
                return _vendorTypeSelected;
            }
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
            get { return _useDistanceFilter; } 
            set 
            {
                _useDistanceFilter = value;
                ResetToNulls();
                UserDistanceFilterChange();
                NotifyPropertyChanged("UseDistanceFilter");
            }
        }

        public ObservableCollection<Location> Circle
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

        #endregion

        #region public

        public ICommand FindRoute
        {
            get
            {
                return _findRoute ??= new CommandHandler(() => CalcRoute()); ;
            }
        }    

        public ICommand HideBtnClick
        {
            get
            {
                return _hideBtnClick ??= new CommandHandler(() => { ResetToNulls(); });
            }
        }

        public ICommand ReviewBtnClick { get
            {
                return _reviewBtnClick ??= new CommandHandler(() => {
                    ReviewsWindow reviewsWindow = new ReviewsWindow(_selectedVendor, user);
                    reviewsWindow.ShowDialog();
                });
            } }

        #endregion
        #endregion

        #region Constructor
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

        #endregion

        #region Methods

        private async void UserDistanceFilterChange()
        {
            if (_useDistanceFilter)
            {
                await GetLiveLocation();
            }
            else
            {
                Circle = null;
            }
            UpdateVendorsList();

        }

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

        private async Task GetLiveLocation()
        {
            user.Location ??= await MapMethods.GetLiveLocation(mainWindow);
        }

        private void UpdateCircle()
        {
            Circle = MapMethods.GetCircleVertices(user.Location, _circleRadius);
            
        }

        private async void CalcRoute()
        {
            await GetLiveLocation();
            RouteLine = MapMethods.GetRoute(user.Location, _selectedVendor.Location);
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
