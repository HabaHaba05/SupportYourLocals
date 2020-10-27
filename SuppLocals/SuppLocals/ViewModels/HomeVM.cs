using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;

namespace SuppLocals.ViewModels
{
    public class HomeVM : ObservableObject
    {
        public readonly User ActiveUser;
        private readonly List<Vendor> _allVendorsList;
        private readonly Window _mainWindow;


        #region Constructor

        public HomeVM(Window window, User user)
        {
            this._mainWindow = window;
            this.ActiveUser = user;
            SelectedArea = Config.Country;

            InitializeCommands();


            _vendorTypesList = new List<string>
            {
                "ALL"
            };

            foreach (var type in Enum.GetValues(typeof(VendorType)))
            {
                _vendorTypesList.Add(type.ToString());
            }

            _vendorTypeSelected = _vendorTypesList[0];


            //Reading all vendors
            using (var db = new AppDbContext())
            {
                var data = db.Vendors.ToList();
                _allVendorsList = new List<Vendor>(data);
                _allVendorsList.ForEach(x => x.Location = new Location(x.Latitude, x.Longitude));
            }


            //If homeView opened first time then we will parse counties and municipalities boundaries

            Config.Country.Children ??= Config.Country.ParseCounties();

            foreach (var x in Config.Country.Children)
            {
                x.Children ??= x.ParseMunicipalities();
            }

            UpdateAreasVendors();
        }

        #endregion

        #region Private props

        private List<Vendor> _vendorList;

        private Vendor _selectedVendor;

        private string _vendorTypeSelected;

        private bool _useDistanceFilter;

        private double _circleRadius;

        private LocationCollection _routeLine;

        private Visibility _selectedVendorInfoGrid = Visibility.Collapsed;

        private double _zoomLevel;

        private Area _selectedArea;

        private readonly List<string> _vendorTypesList;

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
            get => _vendorTypesList;
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
                UserDistanceFilterChangeAsync();
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

        public Area SelectedArea
        {
            get => _selectedArea;
            set
            {
                _selectedArea = value;
                VendorsList = null;
                ZoomLevel = _selectedArea.Zoom;
                if (!_selectedArea.HasChildren)
                {
                    VendorsList = SelectedArea.Vendors;
                }

                if (VendorTypesList != null && (!_selectedArea.HasChildren || _useDistanceFilter))
                {
                    UpdateVendorsList();
                }

                NotifyPropertyChanged("SelectedArea");
            }
        }

        #endregion

        #region Commands

        public RelayCommand FindRouteBtnClick { get; private set; }
        public RelayCommand HideBtnClick { get; private set; }
        public RelayCommand ReviewBtnClick { get; private set; }
        public RelayCommand JumpBackClick { get; private set; }
        public RelayCommand ShowVendorsClick { get; private set; }

        #endregion

        #region Methods

        private void InitializeCommands()
        {
            FindRouteBtnClick = new RelayCommand(o => CalcRouteAsync(), o => true);
            HideBtnClick = new RelayCommand(o => ResetToNulls(), o => true);

            ReviewBtnClick = new RelayCommand(
                o =>
                {
                    ReviewsWindow reviewsWindow = new ReviewsWindow(_selectedVendor, ActiveUser);
                    reviewsWindow.ShowDialog();
                },
                o => true
            );

            JumpBackClick = new RelayCommand(
                o => { SelectedArea = _selectedArea.Parent; },
                o =>
                {
                    //This line is needed only to hide BUG :))))
                    ZoomLevel += 0.0001f;
                    ZoomLevel -= 0.0001f;
                    return _selectedArea.Parent != null;
                });

            ShowVendorsClick = new RelayCommand(o => UpdateVendorsList(), o => true);
        }

        private void UpdateAreasVendors()
        {
            Config.Country.Vendors.Clear();
            PutVendors(Config.Country);

            foreach (var x in Config.Country.Children)
            {
                if (x.HasChildren)
                {
                    x.Vendors.Clear();
                    foreach (var y in x.Children)
                    {
                        x.Vendors.AddRange(y.Vendors);
                    }
                }

                Config.Country.Vendors.AddRange(x.Vendors);
            }
        }

        private async Task UserDistanceFilterChangeAsync()
        {
            if (_useDistanceFilter)
            {
                await GetLiveLocationAsync();
            }

            UpdateVendorsList();
            if (!_useDistanceFilter && _selectedArea.HasChildren)
            {
                VendorsList = null;
            }
        }

        private void UpdateVendorsList()
        {
            if (_vendorTypeSelected == "ALL")
            {
                VendorsList = SelectedArea.Vendors.Where(x =>
                    !_useDistanceFilter || MapMethods.DistanceBetweenPlaces(ActiveUser.Location, x.Location) <=
                    CircleRadius).ToList();
            }
            else
            {
                VendorsList = SelectedArea.Vendors.Where(x =>
                    (x.VendorType == _vendorTypeSelected) && (!_useDistanceFilter ||
                                                              MapMethods.DistanceBetweenPlaces(
                                                                  ActiveUser.Location, x.Location) <=
                                                              CircleRadius)).ToList();
            }
        }

        private async Task GetLiveLocationAsync()
        {
            ActiveUser.Location ??= await MapMethods.GetLiveLocationAsync(_mainWindow);
        }

        private async Task CalcRouteAsync()
        {
            await GetLiveLocationAsync();
            RouteLine = MapMethods.GetRoute(ActiveUser.Location, _selectedVendor.Location);
        }

        private void ResetToNulls()
        {
            SelectedVendorInfoGrid = Visibility.Collapsed;
            RouteLine = null;
            SelectedVendor = null;
        }

        private void PutVendors(Area area)
        {
            if (area.HasChildren)
            {
                foreach (var x in area.Children)
                {
                    PutVendors(x);
                }
            }
            else
            {
                if (area.Level == 1)
                {
                    area.Vendors = _allVendorsList.Where(x =>
                        new string(x.County.Take(4).ToArray()) == new string(area.Name.Take(4).ToArray())).ToList();
                }
                else if (area.Level == 2)
                {
                    area.Vendors = _allVendorsList.Where(x => x.Municipality == area.Name).ToList();
                }
            }
        }

        #endregion
    }
}