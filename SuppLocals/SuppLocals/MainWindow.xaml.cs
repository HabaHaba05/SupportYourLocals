﻿using Microsoft.Maps.MapControl.WPF;
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
        public List<List<Service>> servicesList = new List<List<Service>>();

        //Variables for testing
        public double circleRadius = 0;

        public Location myCurrLocation = null;
        private double myCurrLocationRadius = 0.01;

        public MainWindow()
        {


            // Testing stuff
            List<Service> testFood = new List<Service>();
            testFood.Add(new FoodService("Vilnius Didlaukio 59", new Location(54.73146, 25.2621)));
            testFood.Add(new FoodService("Vilnius Baltupiai", new Location(54.732730865478519, 25.266708374023438)));
            List<Service> testCar = new List<Service>();
            testCar.Add(new CarRepairService("Vilnius Sauletekis", new Location(54.72392, 25.33686)));
            testCar.Add(new CarRepairService("Ukmerge ", new Location(55.2453, 24.7761)));
            List<Service> testOther = new List<Service>();
            testOther.Add(new OtherService("Kaunas", new Location(54.896873474121094, 23.892425537109375)));
            testOther.Add(new OtherService("Vilnius Senamiestis", new Location(54.67876052856445, 25.287307739257812)));

            servicesList.Add(testFood);
            servicesList.Add(testCar);
            servicesList.Add(testOther);

            //By default
            InitializeComponent();

            //Activates the + and – keys to allow the user to manually zoom in and out of the map
            myMap.Focus();
            myMap.CredentialsProvider = Config.BING_API_KEY;

            //Add List<Service> to List of list
            for (int i = 0; i < Enum.GetNames(typeof(ServiceType)).Length; i++)
            {
                servicesList.Add(new List<Service>());
            }

            //Add Service Types to the ComboBox of service type
            createServiceCB.ItemsSource = Enum.GetValues(typeof(ServiceType));
            createServiceCB.SelectedIndex = 0;

            //Add service types to the filterServiceTypeCB in order to filter services by their type
            List<String> types = new List<String>();
            types.Add("ALL");
            foreach (var type in Enum.GetValues(typeof(ServiceType)))
            {
                types.Add(type.ToString());
            }

            filterServiceTypeCB.ItemsSource = types;
            filterServiceTypeCB.SelectedIndex = 0;
        }

        #region Address suggestions

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

        private void addressTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            addressSuggest.Visibility = Visibility.Visible;
        }
        private void addressTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(200);
            addressSuggest.Visibility = Visibility.Collapsed;
        }


        private async void AddressChanged(object sender, KeyEventArgs e)
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
                    addItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void addItem(string text)
        {
            TextBlock block = new TextBlock();

            // Add the text   
            block.Text = text;

            // A little style...   
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

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

        #endregion

        #region Filters
        public void drawCircle(Location Loc, double dRadius, Color fillColor)
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

            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new SolidColorBrush(fillColor);
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            polygon.StrokeThickness = 1;
            polygon.Opacity = 0.65;
            polygon.Locations = locCollection;

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

        private void radiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            circleRadius = (float)radiusSlider.Value;
            if (myMap != null)
                updateServiceListAndMap(null, null);
        }

        public void serviceChanged(object sender, SelectionChangedEventArgs args)
        {

            int selectedIndex = servicesLB.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }


            //ALL services
            if (filterServiceTypeCB.SelectedIndex == 0)
            {
                List<Service> tempList = servicesList.SelectMany(x => x).ToList();

                if (!(bool)filterDistanceCheck.IsChecked)
                {
                    Service service = tempList[selectedIndex];
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
                    foreach (Service service in tempList)
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
                    Service service = servicesList[(int)filterServiceTypeCB.SelectedIndex - 1][selectedIndex];

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
                    foreach (Service service in servicesList[(int)filterServiceTypeCB.SelectedIndex - 1])
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

        public async void distanceFilterChecked(object sender, RoutedEventArgs e)
        {

            if (myCurrLocation == null)
            {
                await getLivelocation();
                if (myCurrLocation == null)
                {
                    filterDistanceCheck.IsChecked = false;
                    return;
                }
            }
            distanceFilterPanel.Visibility = Visibility.Visible;
            circleRadius = radiusSlider.Value;
            myMap.Center = myCurrLocation;
            updateServiceListAndMap(null, null);
        }

        public void distanceFilterUnchecked(object sender, RoutedEventArgs e)
        {
            circleRadius = 0;
            distanceFilterPanel.Visibility = Visibility.Hidden;
            updateServiceListAndMap(null, null);


        }

        #endregion



        public async void addPushPin(object sender, RoutedEventArgs e)
        {

            if (!validFields())
            {
                MessageBox.Show("Please fill all text fields");
                return;
            }

            Service newService = null;
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
                        newService = new FoodService(addressTextBox.Text, new Location(lati, longi));
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.FOOD].Add(newService);
                        break;
                    }
                //Car Repair
                case 1:
                    {
                        newService = new CarRepairService(addressTextBox.Text, new Location(lati, longi));
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.CAR_REPAIR].Add(newService);
                        break;
                    }
                //Other
                case 2:
                    {
                        newService = new OtherService(addressTextBox.Text, new Location(lati, longi));
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.OTHER].Add(newService);
                        break;
                    }
            }
            updateServiceListAndMap(null, null);

        }

        
        private bool validFields()   //True if valid fields , false - invalid
        {
            if (addressTextBox.Text == "")
            {
                return false;
            }

            return true;
        }

        private void updateServiceListAndMap(object sender, SelectionChangedEventArgs args)
        {
            myMap.Children.Clear();
            servicesLB.Items.Clear();

            switch (filterServiceTypeCB.SelectedIndex)
            {
                //ALL
                case 0:
                    {
                        foreach (List<Service> serviceListTemp in servicesList)
                        {
                            foreach (Service service in serviceListTemp)
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
                        foreach (Service service in servicesList[(int)ServiceType.FOOD])
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
                        foreach (Service service in servicesList[(int)ServiceType.CAR_REPAIR])
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
                        foreach (Service service in servicesList[(int)ServiceType.OTHER])
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
                drawCircle(myCurrLocation, circleRadius, Color.FromRgb(240, 248, 255));

                //Current location circle
                drawCircle(myCurrLocation, myCurrLocationRadius, Color.FromRgb(0, 0, 255));

            }
        }

        private async Task getLivelocation()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    ProgressDialog progressDialog = new ProgressDialog();
                    progressDialog.Owner = this;
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


        public void hide_BtnClick(object sender, RoutedEventArgs e)
        {
            selectedServiceInfoGrid.Visibility = Visibility.Hidden;
            Grid.SetColumnSpan(myMap, 2);
        }
        private void PinClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Pushpin p = sender as Pushpin;
            Service service = p.Tag as Service;

            selectedServiceTitle.Text = service.address;
            selectedServiceAbout.Text = "Lat: " + service.location.Latitude + "\nLong: " + service.location.Longitude;
            review_Btn.Tag = service;
            findRoute_Btn.Tag = service;


            selectedServiceInfoGrid.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(myMap, 1);

        }

        public void review_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Service service = b.Tag as Service;

            ReviewsWindow reviewsWindow = new ReviewsWindow(service);
            reviewsWindow.Show();
        }

        public void findRoute_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Service service = b.Tag as Service;

            getR(service.location);
        
        }

        public async void getR(Location finishLoc)
        {

            if (myCurrLocation == null)
            {
                await getLivelocation();
            }
            if (myCurrLocation != null) {
                GoogleMapProvider.Instance.ApiKey = Config.GOOGLE_API_KEY;
                MapRoute route = GoogleMapProvider.Instance.GetRoute(
                       new PointLatLng(myCurrLocation.Latitude, myCurrLocation.Longitude),
                       new PointLatLng(finishLoc.Latitude, finishLoc.Longitude), false, false, 15);

                LocationCollection points = new LocationCollection();
                List<Location> pointsL = new List<Location>();

                foreach (var x in route.Points)
                {
                    pointsL.Add(new Location(x.Lat, x.Lng));
                }

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
    }

}