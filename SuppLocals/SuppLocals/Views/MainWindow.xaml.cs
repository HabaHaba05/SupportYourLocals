using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Geocoding.Microsoft;
using Geocoding;
using SuppLocals.Services;
using Windows.Devices.Geolocation;
using System.Windows.Input;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

using Location = Microsoft.Maps.MapControl.WPF.Location;
using Windows.UI.Xaml.Media.Animation;
using System.Diagnostics;
using GMap.NET.MapProviders;
using GMap.NET;

namespace SuppLocals
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //serviceList[0] - FOOD |  [1] - Car Repair |  [2] - OTHER
        public List<List<Vendor>> servicesList = new List<List<Vendor>>();

        //Variables for testing
        public double circleRadius = 0;

        public Location myCurrLocation = null;
        private readonly double myCurrLocationRadius = 0.01;

        public MainWindow()
        {
            // Testing stuff
            List<Vendor> testFood = new List<Vendor>
            {
                new FoodVendor("Vilnius Didlaukio 59", new Location(54.73146, 25.2621)),
                new FoodVendor("Vilnius Baltupiai", new Location(54.732730865478519, 25.266708374023438))
            };
            List<Vendor> testCar = new List<Vendor>
            {
                new CarRepairVendor("Vilnius Sauletekis", new Location(54.72392, 25.33686)),
                new CarRepairVendor("Ukmerge ", new Location(55.2453, 24.7761))
            };
            List<Vendor> testOther = new List<Vendor>
            {
                new OtherVendor("Kaunas", new Location(54.896873474121094, 23.892425537109375)),
                new OtherVendor("Vilnius Senamiestis", new Location(54.67876052856445, 25.287307739257812))
            };

            servicesList.Add(testFood);
            servicesList.Add(testCar);
            servicesList.Add(testOther);

            //By default
            InitializeComponent();

            //Activates the + and – keys to allow the user to manually zoom in and out of the map
            myMap.Focus();
            myMap.CredentialsProvider = Config.BING_API_KEY;

            //Add List<Service> to List of list
            for (int i = 0; i < Enum.GetNames(typeof(VendorType)).Length; i++)
            {
                servicesList.Add(new List<Vendor>());
            }

            //Add Service Types to the ComboBox of service type
            createServiceCB.ItemsSource = Enum.GetValues(typeof(VendorType));
            createServiceCB.SelectedIndex = 0;

            //Add service types to the filterServiceTypeCB in order to filter services by their type
            List<String> types = new List<String>
            {
                "ALL"
            };
            foreach (var type in Enum.GetValues(typeof(VendorType)))
            {
                types.Add(type.ToString());
            }

            filterServiceTypeCB.ItemsSource = types;
            filterServiceTypeCB.SelectedIndex = 0;
        }


        #region Autocomplete Address TextBox

        //Method to get JSON from google API
        public async Task<List<string>> GetData()
        {
            List<string> data = new List<string>();

            try
            {
                string uri = Config.host + Config.path + "?input=" + addressTextBox.Text + "&types=geocode&language=lt&components=country:lt&key=" + Config.GOOGLE_API_KEY;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject o = JObject.Parse(responseBody);


                JObject jObj = (JObject)JsonConvert.DeserializeObject(responseBody);
                var ob = jObj["predictions"];
                int count = ob.Count();
                if (count > 3)
                {
                    addressSuggest.Height = 90;
                }
                for (int i = 0; i < count; i++)
                {
                    data.Add((string)o.SelectToken("predictions[" + i + "].description"));
                }


                return data;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return data;
        }

        //Method to add addresses suggestions
        private void AddItem(string text)
        {
            TextBlock block = new TextBlock
            {

                // Add the text   
                Text = text,

                // A little style...   
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                addressTextBox.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            resultStack.Children.Add(block);
        }

        private void AddressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addressSuggest.Visibility = Visibility.Visible;
        }
        private void AddressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(200);
            addressSuggest.Visibility = Visibility.Collapsed;
        }
        private async void AddressTextBox_Changed(object sender, KeyEventArgs e)
        {
            string query = (sender as TextBox).Text;
            if (String.IsNullOrEmpty(query))
            {
                return;
            };

            bool found = false;
            var data = await GetData();

            if (query.Length == 0)
            {
                resultStack.Children.Clear();
            }

            // Clear the list   
            resultStack.Children.Clear();

            // Add the result   
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        #endregion

        #region Methods associated with current user location

        private async Task GetLivelocation()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    ProgressDialog progressDialog = new ProgressDialog
                    {
                        Owner = this
                    };
                    Application.Current.Dispatcher.Invoke(new Action(() => this.IsEnabled = false));
                    _ = progressDialog.Dispatcher.BeginInvoke(new Action(() => progressDialog.ShowDialog()));

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };

                    // Carry out the operation
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    myCurrLocation = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);

                    Application.Current.Dispatcher.Invoke(new Action(() => this.IsEnabled = true));
                    progressDialog.Close();


                    break;
                default:
                    MessageBox.Show("We can't reach your location. Please check that the following location privacy are turned on:\n" +
                                    "Location for this device... is turned on (not applicable in Windows 10 Mobile\n" +
                                    "The location services setting, Location, is turned on\n" +
                                    "Under Choose apps that can use your location, your app is set to on\n ");

                    break;
            }

        }

        public void DrawCircle(Location Loc, double dRadius, Color fillColor)
                {

                    var locCollection = new LocationCollection();
                    var EarthRadius = 6367; // Earth Radius in Kilometers

                    //Convert location to radians based on
                    var latitude = (Math.PI / 180) * (Loc.Latitude);
                    var longitude = (Math.PI / 180) * (Loc.Longitude);

                    var d = dRadius / EarthRadius;

                    for (int x = 0; x < 360; x++)
                    {
                        var angle = x * (Math.PI / 180); //radians
                        var latRadians = Math.Asin(Math.Sin(latitude) * Math.Cos(d) + Math.Cos(latitude) * Math.Sin(d) * Math.Cos(angle));
                        var lngRadians = longitude + Math.Atan2(Math.Sin(angle) * Math.Sin(d) * Math.Cos(latitude), Math.Cos(d) - Math.Sin(latitude) * Math.Sin(latRadians));

                        //Get location of the point
                        var pt = new Location(180.0 * latRadians / Math.PI, 180.0 * lngRadians / Math.PI);

                        //Add the new calculatied poitn to the collection
                        locCollection.Add(pt);
                    }

                    MapPolygon polygon = new MapPolygon
                    {
                        Fill = new SolidColorBrush(fillColor),
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 1,
                        Opacity = 0.65,
                        Locations = locCollection
                    };

                    myMap.Children.Add(polygon);
                }

        public double DistanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6371; // Earth radius km

            double sLat1 = Math.Sin(lat1 * (Math.PI / 180));
            double sLat2 = Math.Sin(lat2 * (Math.PI / 180));
            double cLat1 = Math.Cos(lat1 * (Math.PI / 180));
            double cLat2 = Math.Cos(lat2 * (Math.PI / 180));
            double cLon = Math.Cos(lon1 * (Math.PI / 180) - lon2 * (Math.PI / 180));

            double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            double d = Math.Acos(cosD);

            double dist = R * d;

            return dist;
        }


        #endregion

        #region Filters change

        public async void DistanceFilterChecked(object sender, RoutedEventArgs e)
        {

            if (myCurrLocation == null)
            {
                await GetLivelocation();
                if (myCurrLocation == null)
                {
                    filterDistanceCheck.IsChecked = false;
                    return;
                }
            }
            distanceFilterPanel.Visibility = Visibility.Visible;
            circleRadius = radiusSlider.Value;
            myMap.Center = myCurrLocation;
            UpdateServiceListAndMap(null, null);
        }
        private void RadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
                {

                    circleRadius = (float)radiusSlider.Value;
                    if (myMap != null)
                        UpdateServiceListAndMap(null, null);
                }

        public void DistanceFilterUnchecked(object sender, RoutedEventArgs e)
                {
                    circleRadius = 0;
                    distanceFilterPanel.Visibility = Visibility.Hidden;
                    UpdateServiceListAndMap(null, null);


                }


        #endregion

        #region Changing myMap

        public void SelectedServiceLB_Changed(object sender, SelectionChangedEventArgs args)
        {

            int selectedIndex = servicesLB.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }


            //ALL services
            if (filterServiceTypeCB.SelectedIndex == 0)
            {
                List<Vendor> tempList = servicesList.SelectMany(x => x).ToList();

                if (!(bool)filterDistanceCheck.IsChecked)
                {
                    Vendor service = tempList[selectedIndex];
                    myMap.Center = service.location;

                    selectedServiceTitle.Text = service.address;
                    selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
                    review_Btn.Tag = service;
                    findRoute_Btn.Tag = service;

                    selectedServiceInfoGrid.Visibility = Visibility.Visible;
                    Grid.SetColumnSpan(myMap, 1);
                }
                else
                {
                    foreach (Vendor service in tempList)
                    {
                        if (DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                        {
                            selectedIndex--;
                            if (selectedIndex == -1)
                            {
                                myMap.Center = service.location;

                                selectedServiceTitle.Text = service.address;
                                selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
                                review_Btn.Tag = service;
                                findRoute_Btn.Tag = service;


                                selectedServiceInfoGrid.Visibility = Visibility.Visible;
                                Grid.SetColumnSpan(myMap, 1);
                            }
                        }
                    }
                }
            }
            else
            {
                if (!(bool)filterDistanceCheck.IsChecked)
                {
                    Vendor service = servicesList[(int)filterServiceTypeCB.SelectedIndex - 1][selectedIndex];

                    myMap.Center = new Location(service.location.Latitude, service.location.Longitude);

                    selectedServiceTitle.Text = service.address;
                    selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
                    review_Btn.Tag = service;
                    findRoute_Btn.Tag = service;

                    selectedServiceInfoGrid.Visibility = Visibility.Visible;
                    Grid.SetColumnSpan(myMap, 1);

                }
                else
                {
                    foreach (Vendor service in servicesList[(int)filterServiceTypeCB.SelectedIndex - 1])
                    {
                        if (DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                        {
                            selectedIndex--;
                            if (selectedIndex == -1)
                            {
                                myMap.Center = service.location;

                                selectedServiceTitle.Text = service.address;
                                selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
                                review_Btn.Tag = service;
                                findRoute_Btn.Tag = service;


                                selectedServiceInfoGrid.Visibility = Visibility.Visible;
                                Grid.SetColumnSpan(myMap, 1);
                            }
                        }
                    }

                }
            }
        }

        public async void AddPushPin(object sender, RoutedEventArgs e)
                {

                    if (!ValidFields())
                    {
                        MessageBox.Show("Please fill all text fields");
                        return;
                    }

                    Vendor newService = null;
                    Pushpin pushPin = new Pushpin();

                    Geocoding.IGeocoder geocoder = new BingMapsGeocoder("vuOU7tN47KBhly1BAyhi~SKpEroFcVqMGYOJVSj-2HA~AhGXS-dV_H6Ofvn920LLMyvxfUUaLfjpZTD54fSc3WO-qRE7x6225O22AP_0XjDn");
                    IEnumerable<Address> addresses = await geocoder.GeocodeAsync(addressTextBox.Text);

                    if (addresses.Count() == 0)
                    {
                        MessageBox.Show("We couldn't find that place, please try to clarify the address");
                        return;
                    }

                    double lati = addresses.First().Coordinates.Latitude;
                    double longi = addresses.First().Coordinates.Longitude;

                    pushPin.Location = new Location(lati, longi);
                    pushPin.MouseDown += PinClicked;



                    myMap.Children.Add(pushPin);
                    myMap.Center = pushPin.Location;


                    switch (createServiceCB.SelectedIndex)
                    {
                        //Food
                        case 0:
                            {
                                newService = new FoodVendor(addressTextBox.Text, new Location(lati, longi));
                                pushPin.Background = newService.color;
                                servicesList[(int)VendorType.FOOD].Add(newService);
                                break;
                            }
                        //Car Repair
                        case 1:
                            {
                                newService = new CarRepairVendor(addressTextBox.Text, new Location(lati, longi));
                                pushPin.Background = newService.color;
                                servicesList[(int)VendorType.CAR_REPAIR].Add(newService);
                                break;
                            }
                        //Other
                        case 2:
                            {
                                newService = new OtherVendor(addressTextBox.Text, new Location(lati, longi));
                                pushPin.Background = newService.color;
                                servicesList[(int)VendorType.OTHER].Add(newService);
                                break;
                            }
                    }
                    UpdateServiceListAndMap(null, null);

                }

        private void UpdateServiceListAndMap(object sender, SelectionChangedEventArgs args)
        {
            myMap.Children.Clear();
            servicesLB.Items.Clear();

            switch (filterServiceTypeCB.SelectedIndex)
            {
                //ALL
                case 0:
                    {
                        foreach (List<Vendor> serviceListTemp in servicesList)
                        {
                            foreach (Vendor service in serviceListTemp)
                            {
                                if (!(bool)filterDistanceCheck.IsChecked ||
                                    DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                                {
                                    Pushpin pushpin = new Pushpin();
                                    pushpin.MouseUp += PinClicked;
                                    pushpin.Tag = service;

                                    pushpin.Location = new Location(service.location.Latitude, service.location.Longitude);
                                    pushpin.Background = service.color;
                                    myMap.Children.Add(pushpin);
                                    servicesLB.Items.Add(service.address + "\nLat:" + service.location.Latitude.ToString("N2") + "\nLong:" + service.location.Longitude.ToString("N2"));
                                }
                            }
                        }
                        break;
                    }
                //Food
                case 1:
                    {
                        foreach (Vendor service in servicesList[(int)VendorType.FOOD])
                        {
                            if (!(bool)filterDistanceCheck.IsChecked ||
                                DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                            {
                                Pushpin pushpin = new Pushpin();
                                pushpin.MouseUp += PinClicked;
                                pushpin.Tag = service;


                                pushpin.Location = new Location(service.location.Latitude, service.location.Longitude);
                                pushpin.Background = service.color;
                                myMap.Children.Add(pushpin);
                                servicesLB.Items.Add(service.address + "\nLat:" + service.location.Latitude.ToString("N2") + "\nLong:" + service.location.Longitude.ToString("N2"));
                            }
                        }
                        break;
                    }
                //Car Repair
                case 2:
                    {
                        foreach (Vendor service in servicesList[(int)VendorType.CAR_REPAIR])
                        {
                            if (!(bool)filterDistanceCheck.IsChecked ||
                                DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                            {
                                Pushpin pushpin = new Pushpin();
                                pushpin.MouseUp += PinClicked;
                                pushpin.Tag = service;

                                pushpin.Location = new Location(service.location.Latitude, service.location.Longitude);
                                pushpin.Background = service.color;
                                myMap.Children.Add(pushpin);
                                servicesLB.Items.Add(service.address + "\nLat:" + service.location.Latitude.ToString("N2") + "\nLong:" + service.location.Longitude.ToString("N2"));
                            }
                        }
                        break;
                    }
                //Other
                case 3:
                    {
                        foreach (Vendor service in servicesList[(int)VendorType.OTHER])
                        {
                            if (!(bool)filterDistanceCheck.IsChecked ||
                                DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                            {
                                Pushpin pushpin = new Pushpin();
                                pushpin.MouseUp += PinClicked;
                                pushpin.Tag = service;

                                pushpin.Location = new Location(service.location.Latitude, service.location.Longitude);
                                pushpin.Background = service.color;
                                myMap.Children.Add(pushpin);
                                servicesLB.Items.Add(service.address + "\nLat:" + service.location.Latitude.ToString("N2") + "\nLong:" + service.location.Longitude.ToString("N2"));
                            }
                        }
                        break;
                    }

            }
            if ((bool)filterDistanceCheck.IsChecked)
            {
                //Filter circle
                DrawCircle(myCurrLocation, circleRadius, Color.FromRgb(240, 248, 255));

                //Current location circle
                DrawCircle(myCurrLocation, myCurrLocationRadius, Color.FromRgb(0, 0, 255));

            }
        }

        #endregion

        #region Vendor About Tab

        public void Hide_BtnClick(object sender, RoutedEventArgs e)
        {
            UpdateServiceListAndMap(null, null);
            selectedServiceInfoGrid.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(myMap, 3);
        }

        private void PinClicked(object sender, MouseButtonEventArgs e)
        {
            Pushpin p = sender as Pushpin;
            Vendor service = p.Tag as Vendor;

            selectedServiceTitle.Text = service.address;
            selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
            review_Btn.Tag = service;
            findRoute_Btn.Tag = service;


            selectedServiceInfoGrid.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(myMap, 2);
        }

        public void Review_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Vendor service = b.Tag as Vendor;

            ReviewsWindow reviewsWindow = new ReviewsWindow(service);
            reviewsWindow.Show();
        }

        public void FindRoute_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Vendor service = b.Tag as Vendor;

            GetRoute(service.location);       
        }

        public async void GetRoute(Location finishLoc)
        {
            UpdateServiceListAndMap(null,null);
            if (myCurrLocation == null)
            {
                await GetLivelocation();
            }
            if (myCurrLocation != null) {
                GoogleMapProvider.Instance.ApiKey = Config.GOOGLE_API_KEY;
                MapRoute route = GoogleMapProvider.Instance.GetRoute(
                       new PointLatLng(myCurrLocation.Latitude, myCurrLocation.Longitude),
                       new PointLatLng(finishLoc.Latitude, finishLoc.Longitude), false, false, 15);

                LocationCollection points = new LocationCollection();
                List<Location> pointsL = new List<Location>(route.Points.ConvertAll(x => new Location(x.Lat, x.Lng)));

                foreach (var x in pointsL)
                {
                    points.Add(x);
                }

                MapPolyline routeLine = new MapPolyline()
                {
                    Locations = points,
                    Stroke = new SolidColorBrush(Colors.Green),
                    StrokeThickness = 5
                };

                myMap.Children.Add(routeLine);
            }
        }
        #endregion
        
        private bool ValidFields()   //True if valid fields , false - invalid
        {
            if (addressTextBox.Text == "")
            {
                return false;
            }

            return true;
        }     
    }

}