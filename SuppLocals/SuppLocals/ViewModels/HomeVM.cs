using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuppLocals.Classes.Enums;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Animation;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals.ViewModels
{
    public class HomeVM : BaseViewModel
    {
        private readonly Window mainWindow;
        private readonly List<Vendor> allVendorsList;
        public readonly User activeUser;


        #region Private props
        private List<Vendor> _vendorList;

        private Vendor _selectedVendor;

        private string _vendorTypeSelected;

        private bool _useDistanceFilter;

        private double _circleRadius;

        private LocationCollection _routeLine;

        private Visibility _selectedVendorInfoGrid = Visibility.Collapsed;

        private double _zoomLevel;

        private ObservableCollection<Area> _counties;

        private Area _selectedArea;

        #endregion

        #region Public props

        public double CircleRadius
        {
            get { return _circleRadius; }
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
                ZoomLevel += 0.01;
                NotifyPropertyChanged("RouteLine");
                ZoomLevel -= 0.01;
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

        public Visibility SelectedVendorInfoGrid
        {
            get { return _selectedVendorInfoGrid; }
            set
            {
                _selectedVendorInfoGrid = value;
                NotifyPropertyChanged("SelectedVendorInfoGrid");
            }
        }

        public double ZoomLevel
        {
            get { return _zoomLevel; }
            set
            {
                _zoomLevel = value;
                NotifyPropertyChanged("ZoomLevel");
            }
        }

        public ObservableCollection<Area> Counties
        {
            get { return _counties; }
            private set
            {
                _counties = value;
                NotifyPropertyChanged("Counties");
            }
        }

        

        public Area SelectedArea{ 
            get { return _selectedArea; }
            set 
            {

                _selectedArea = value;
                VendorsList = null;
                ZoomLevel = _selectedArea.Zoom;
                NotifyPropertyChanged("SelectedArea");
            }
        }

        #endregion


        #region Commands

        public RelayCommand FindRouteBtnClick {get; private set;}    

        public RelayCommand HideBtnClick {get; private set;}

        public RelayCommand ReviewBtnClick { get; private set; }
        public RelayCommand JumpBackClick { get; private set; }
        public RelayCommand ShowVendorsClick { get; private set; }


        #endregion

        #region Constructor
        public HomeVM(Window window, User user)
        {
            this.mainWindow = window;
            this.activeUser = user;
            
            Config.Country.Children ??= Config.Country.ParseCounties();

            SelectedArea = Config.Country;

            using (VendorsDbTable db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                allVendorsList = new List<Vendor>(data);
                allVendorsList.ForEach(x => x.Location = new Location(x.Latitude, x.Longitude));
            }

            FindRouteBtnClick = new RelayCommand(o => CalcRoute(), o => true);
            HideBtnClick = new RelayCommand(o => ResetToNulls(), o => true);
            
            ReviewBtnClick = new RelayCommand(o =>
            {
                ReviewsWindow reviewsWindow = new ReviewsWindow(_selectedVendor, user);
                reviewsWindow.ShowDialog();
            },
                o => true
            );

            JumpBackClick = new RelayCommand(o => SelectedArea = _selectedArea.Parent  , o => { 
                //This line is needed only to hide BUG :))))
                ZoomLevel +=0.0001f; ZoomLevel -= 0.0001f; 
                return _selectedArea.Parent != null; } );

            ShowVendorsClick = new RelayCommand(o => UpdateVendorsList(), o => true);
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
            //Updating by filter criterium
            if (VendorTypeSelected.ToString() == "ALL")
            {
                VendorsList = allVendorsList.Where(x => !_useDistanceFilter || MapMethods.DistanceBetweenPlaces(activeUser.Location, x.Location) <= CircleRadius).ToList();
            }
            else
            {
                VendorsList = allVendorsList.Where(x =>
                        (x.VendorType == VendorTypeSelected.ToString()) && (!_useDistanceFilter || MapMethods.DistanceBetweenPlaces(activeUser.Location, x.Location) <= CircleRadius)).ToList();
            }
            //Updating by selected area
            if (_selectedArea.Level == 1)
            {
                VendorsList = _vendorList.Where(x => new string(x.County.Take(4).ToArray()) == new string(_selectedArea.Name.Take(4).ToArray())).ToList();
            }
            else if (_selectedArea.Level == 2)
            {
                VendorsList = _vendorList.Where(x => x.Municipality == _selectedArea.Name).ToList();
            }


        }

        private async Task GetLiveLocation()
        {
            activeUser.Location ??= await MapMethods.GetLiveLocation(mainWindow);
        }

        private async void CalcRoute()
        {
            await GetLiveLocation();
            RouteLine = MapMethods.GetRoute(activeUser.Location, _selectedVendor.Location);
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
