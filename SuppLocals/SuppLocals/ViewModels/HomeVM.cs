using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuppLocals.Classes.Enums;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private double _zoomLevel = 12;

        private ObservableCollection<County> _counties;


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

        public ObservableCollection<County> Counties
        {
            get { return _counties; }
            private set
            {
                _counties = value;
                NotifyPropertyChanged("Counties");
            }
        }


        #endregion


        #region Commands

        public RelayCommand FindRouteBtnClick {get; private set;}    

        public RelayCommand HideBtnClick {get; private set;}

        public RelayCommand ReviewBtnClick { get; private set; }

        #endregion

        #region Constructor
        public HomeVM(Window window, User user)
        {
            this.mainWindow = window;
            this.activeUser = user;

            LoadCounties();

            using (VendorsDbTable db = new VendorsDbTable())
            {
                var data = db.Vendors.ToList();
                allVendorsList = new List<Vendor>(data);
                allVendorsList.ForEach(x => x.Location = new Location(x.Latitude, x.Longitude));
                VendorsList = allVendorsList;
            }

            FindRouteBtnClick = new RelayCommand(o => CalcRoute(), o => true);
            HideBtnClick = new RelayCommand(o => ResetToNulls(), o => true);
            
            ReviewBtnClick = new RelayCommand(o =>
            {
                ReviewsWindow reviewsWindow = new ReviewsWindow(_selectedVendor, user);
                reviewsWindow.ShowDialog();
            },
                o => true
            ); ;
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
            if (VendorTypeSelected.ToString() == "ALL")
            {
                VendorsList = allVendorsList.Where(x => !_useDistanceFilter || MapMethods.DistanceBetweenPlaces(activeUser.Location, x.Location) <= CircleRadius).ToList();
            }
            else
            {
                VendorsList = allVendorsList.Where(x =>
                        (x.VendorType == VendorTypeSelected.ToString()) && (!_useDistanceFilter || MapMethods.DistanceBetweenPlaces(activeUser.Location, x.Location) <= CircleRadius)).ToList();
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


        #region Counties layers


        private void LoadCounties()
        {
            List<County> lcnt = new List<County>();

            foreach(var name in Enum.GetNames(typeof(CountiesNames)))
            {
                lcnt.Add(ParseJsons($"/Assets/CountiesJsons/{name}County.json"));
            }

            Counties = new ObservableCollection<County>(lcnt);

        }

        private County ParseJsons(string filePath)
        {
            JObject o1 = JObject.Parse(File.ReadAllText((string)(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent + filePath)));
            var locations = o1.SelectToken("locations");

            LocationCollection locColl = new LocationCollection();

            for (int i = 0; i < locations.Count(); i++)
            {
                locColl.Add(new Location((double)locations[i][1], (double)locations[i][0]));
            }

            return new County(locColl);
        }

        #endregion

    }
}
