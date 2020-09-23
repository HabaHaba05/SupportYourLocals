using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Xml.Linq;
using Geocoding.Microsoft;
using Geocoding;
using SuppLocals.Services;
using System.Numerics;

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

        public Microsoft.Maps.MapControl.WPF.Location myCurrLocation = new Microsoft.Maps.MapControl.WPF.Location(54.6872, 25.2797);
       
        public MainWindow()
        {
            // Testing stuff
            List<Service> testFood = new List<Service>();
            testFood.Add(new FoodService("Vilnius Didlaukio 59", new Microsoft.Maps.MapControl.WPF.Location(54.73146, 25.2621)));
            testFood.Add(new FoodService("Vilnius Baltupiai", new Microsoft.Maps.MapControl.WPF.Location(54.732730865478519, 25.266708374023438)));
            List<Service> testCar = new List<Service>();
            testCar.Add(new CarRepairService("Vilnius Sauletekis", new Microsoft.Maps.MapControl.WPF.Location(54.72392, 25.33686)));
            testCar.Add(new CarRepairService("Ukmerge ", new Microsoft.Maps.MapControl.WPF.Location(55.2453, 24.7761)));
            List<Service> testOther = new List<Service>();
            testOther.Add(new OtherService("Kaunas", new Microsoft.Maps.MapControl.WPF.Location(54.896873474121094, 23.892425537109375)));
            testOther.Add(new OtherService("Vilnius Senamiestis", new Microsoft.Maps.MapControl.WPF.Location(54.67876052856445, 25.287307739257812)));

            servicesList.Add(testFood);
            servicesList.Add(testCar);
            servicesList.Add(testOther);


            //By default
            InitializeComponent();

            //Activates the + and – keys to allow the user to manually zoom in and out of the map
            myMap.Focus();

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

          //  updateServiceListAndMap(null, null);
        }
        private void buttonClick(object sender, RoutedEventArgs e)
        {
            if (sPan.Visibility == Visibility.Collapsed)
            {
                sPan.Visibility = Visibility.Visible;
                (sender as Button).Content = "☰";
            }
            else
            {
                sPan.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "☰";
            }
        }

        private void servicesClick(object sender, RoutedEventArgs e)
        {
            if (servicePanel.Visibility == Visibility.Collapsed)
            {
                servicePanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Vendors";
            }
            else
            {
                servicePanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Vendors";
            }
        }

        private void filterClick(object sender, RoutedEventArgs e)
        {
            if (filterPanel.Visibility == Visibility.Collapsed)
            {
                filterPanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Filters";
            }
            else
            {
                filterPanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Filters";
            }
        }

        private void reviewsClick(object sender, RoutedEventArgs e)
        {
            if (reviewsPanel.Visibility == Visibility.Collapsed)
            {
                reviewsPanel.Visibility = Visibility.Visible;
                (sender as Button).Content = "Reviews";
            }
            else
            {
                reviewsPanel.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "Reviews";
            }
        }

        private void MoreButtonClicked(object sender, RoutedEventArgs e)
        {
            ReviewsWindow rWindow = new ReviewsWindow();
            rWindow.Show();
        }


        private void hyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //true if the shell should be used when starting the process; false if the process should be created directly from the executable file.
            //The default is true on.NET Framework apps and false on.NET Core apps.
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }


     public void drawCircle(Microsoft.Maps.MapControl.WPF.Location Loc, double dRadius)
        {

            var locCollection = new LocationCollection();
            var EarthRadius = 6367; // Earth Radius in Kilometers

            //Convert location to radians based on
            var latitude = (Math.PI / 180) * (Loc.Latitude);
            var longitude = (Math.PI / 180) * (Loc.Longitude);
         
            //Angular distance covered on earth surface
            var d = dRadius / EarthRadius;
         
            for (int x = 0; x < 360; x++)
            {
                var angle = x * (Math.PI/180); //radians
                var latRadians = Math.Asin(Math.Sin(latitude) * Math.Cos(d) + Math.Cos(latitude) * Math.Sin(d) * Math.Cos(angle));
                var lngRadians = longitude + Math.Atan2(Math.Sin(angle) * Math.Sin(d) * Math.Cos(latitude), Math.Cos(d) - Math.Sin(latitude) * Math.Sin(latRadians));
         
                //Get location of the point
                var pt = new Microsoft.Maps.MapControl.WPF.Location(180.0 * latRadians / Math.PI, 180.0 * lngRadians / Math.PI);
         
                //Add the new calculatied poitn to the collection
                locCollection.Add(pt);
            }
            
           
            MapPolygon polygon = new MapPolygon();
            polygon.Fill = new SolidColorBrush(Colors.AliceBlue);
            polygon.Stroke = new SolidColorBrush(Colors.Black);
            polygon.StrokeThickness = 1;
            polygon.Opacity = 0.65;
            polygon.Locations = locCollection;

            myMap.Children.Add(polygon);
        }
       
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

            if(addresses.Count() == 0)
            {
                MessageBox.Show("We couldn't find that place, please try to clarify the address");
                return;
            }

            double lati = addresses.First().Coordinates.Latitude;
            double longi = addresses.First().Coordinates.Longitude;

            pushPin.Location = new Microsoft.Maps.MapControl.WPF.Location(lati , longi);
            myMap.Children.Add(pushPin);
            myMap.Center = pushPin.Location;

 
            switch (createServiceCB.SelectedIndex)
            {
                //Food
                case 0:
                    {
                        newService = new FoodService(addressTextBox.Text, new Microsoft.Maps.MapControl.WPF.Location(lati, longi));
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.FOOD].Add(newService);
                        break;
                    }
                //Car Repair
                case 1:
                    {
                        newService = new CarRepairService(addressTextBox.Text, new Microsoft.Maps.MapControl.WPF.Location(lati , longi) );
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.CAR_REPAIR].Add(newService);
                        break;
                    }
                //Other
                case 2:
                    {
                        newService = new OtherService(addressTextBox.Text, new Microsoft.Maps.MapControl.WPF.Location(lati, longi));
                        pushPin.Background = newService.color;
                        servicesList[(int)ServiceType.OTHER].Add(newService);
                        break;
                    }
            }
            updateServiceListAndMap(null, null);

        }

        private bool validFields()   //True if valid fields , false - invalid
        {
            if(addressTextBox.Text == "")
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
                            foreach (Service service in serviceListTemp) {
                                if (!(bool)filterDistanceCheck.IsChecked || 
                                    DistanceBetweenPlaces(service.location.Longitude , service.location.Latitude , myCurrLocation.Longitude , myCurrLocation.Latitude) <= circleRadius) {
                                    Pushpin pushpin = new Pushpin();
                                    pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(service.location.Latitude, service.location.Longitude);
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
                                pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(service.location.Latitude, service.location.Longitude);
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
                                pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(service.location.Latitude, service.location.Longitude);
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
                            if ( !(bool)filterDistanceCheck.IsChecked ||
                                DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                            {
                                Pushpin pushpin = new Pushpin();
                                pushpin.Location = new Microsoft.Maps.MapControl.WPF.Location(service.location.Latitude, service.location.Longitude);
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
                drawCircle(myCurrLocation, circleRadius);
            }
        }
        public double DistanceBetweenPlaces(double lon1, double lat1, double lon2, double lat2)
        {
            double R = 6371; // km

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

        public void serviceChanged(object sender , SelectionChangedEventArgs args)
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
                    myMap.Center = tempList[selectedIndex].location;
                }
                else
                {
                    foreach (Service service in tempList)
                    {
                        if (DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                        {
                            selectedIndex--;
                            if(selectedIndex == -1)
                            {
                                myMap.Center = service.location;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!(bool)filterDistanceCheck.IsChecked)
                {
                    myMap.Center = new Microsoft.Maps.MapControl.WPF.Location(servicesList[(int)filterServiceTypeCB.SelectedIndex - 1][selectedIndex].location.Latitude, servicesList[(int)filterServiceTypeCB.SelectedIndex - 1][selectedIndex].location.Longitude);
                }
                else
                {
                    foreach (Service service in servicesList[(int)filterServiceTypeCB.SelectedIndex-1])
                    {
                        if (DistanceBetweenPlaces(service.location.Longitude, service.location.Latitude, myCurrLocation.Longitude, myCurrLocation.Latitude) <= circleRadius)
                        {
                            selectedIndex--;
                            if (selectedIndex == -1)
                            {
                                myMap.Center = service.location;
                            }
                        }
                    }

                }
            }
        }
        
        private void radiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            circleRadius = (float)radiusSlider.Value;
            if(myMap != null)
            updateServiceListAndMap(null, null);
            
        }

        public void distanceFilterChecked(object sender, RoutedEventArgs e)
        {
            distanceFilterPanel.Visibility=Visibility.Visible;
            circleRadius = radiusSlider.Value;
            updateServiceListAndMap(null, null);
        }

        public void distanceFilterUnchecked(object sender, RoutedEventArgs e)
        {
            circleRadius = 0;
            distanceFilterPanel.Visibility = Visibility.Hidden;
            updateServiceListAndMap(null, null);


        }
    }

}
