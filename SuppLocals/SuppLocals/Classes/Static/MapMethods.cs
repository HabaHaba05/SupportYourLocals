using Geocoding;
using Geocoding.Microsoft;
using GMap.NET;
using GMap.NET.MapProviders;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SuppLocals.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Windows.Devices.Geolocation;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals
{
    public static class MapMethods
    {
        // Returns current user position
        public static async Task<Location> GetLiveLocation(Window window)
        {
            Location loc = new Location(0, 0);

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    ProgressDialog progressDialog = new ProgressDialog
                    {
                        Owner = window
                    };
                    Application.Current.Dispatcher.Invoke(new Action(() => window.IsEnabled = false));
                    _ = progressDialog.Dispatcher.BeginInvoke(new Action(() => progressDialog.ShowDialog()));

                    // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 0 };

                    // Carry out the operation
                    Geoposition pos = await geolocator.GetGeopositionAsync();

                    loc = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);

                    Application.Current.Dispatcher.Invoke(new Action(() => window.IsEnabled = true));
                    progressDialog.Close();


                    break;
                default:
                    MessageBox.Show("We can't reach your location. Please check that the following location privacy are turned on:\n" +
                                    "Location for this device... is turned on (not applicable in Windows 10 Mobile\n" +
                                    "The location services setting, Location, is turned on\n" +
                                    "Under Choose apps that can use your location, your app is set to on\n ");

                    break;
            }

            return loc;
        }

        //Returns locations collection of route points
        public static LocationCollection GetRoute(Location userLoc , Location finishLoc)
        {
            GoogleMapProvider.Instance.ApiKey = Config.GOOGLE_API_KEY;
            MapRoute route = GoogleMapProvider.Instance.GetRoute(
                   new PointLatLng(userLoc.Latitude, userLoc.Longitude),
                   new PointLatLng(finishLoc.Latitude, finishLoc.Longitude), false, false, 15);
            
            if(route == null)
            {
                MessageBox.Show("Sorry we can't find route to this location");
                return null;
            }

            LocationCollection points = new LocationCollection();
            List<Location> pointsL = new List<Location>(route.Points.ConvertAll(x => new Location(x.Lat, x.Lng)));
            

            foreach (var x in pointsL)
            {
                points.Add(x);
            }
            
            return points;
        }

        //Return distance between two locations
        public static double DistanceBetweenPlaces(Location loc1, Location loc2)
        {
            double R = 6371; // Earth radius km

            double sLat1 = Math.Sin(loc1.Latitude * (Math.PI / 180));
            double sLat2 = Math.Sin(loc2.Latitude * (Math.PI / 180));
            double cLat1 = Math.Cos(loc1.Latitude * (Math.PI / 180));
            double cLat2 = Math.Cos(loc2.Latitude * (Math.PI / 180));
            double cLon = Math.Cos(loc1.Longitude* (Math.PI / 180) - loc2.Longitude * (Math.PI / 180));

            double cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            double d = Math.Acos(cosD);

            double dist = R * d;

            return dist;
        }

        public static LocationCollection GetCircleVertices(Location Loc, double dRadius)
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

            return locCollection;
        }

        public static void DrawCircle(LocationCollection locCollection, MapLayer map, Color fillColor)
        {
            MapPolygon polygon = new MapPolygon
            {
                Fill = new SolidColorBrush(fillColor),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Opacity = 0.65,
                Locations = locCollection
            };

            map.Children.Add(polygon);
        }

        public static async Task<Location> ConvertAddressToLocation(string address)
        {
            Location data = new Location();

            try
            {
                string uri = Config.host + "/maps/api/geocode/json?address=" + address + "language=lt&key=" + Config.GOOGLE_API_KEY;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject o = JObject.Parse(responseBody);

                double lat = (double)o.SelectToken("results[0].geometry.location.lat");
                double lng = (double)o.SelectToken("results[0].geometry.location.lng");


                data = new Location(lat, lng);

                return data;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return data;
        }

        public static async Task<string> ConvertLocationToAddress(Location location)
        {
            string data ="";

            try
            {
                string uri = Config.host + "/maps/api/geocode/json?latlng=" + location.Latitude.ToString()+","+location.Longitude.ToString() + "&language=lt&key=" + Config.GOOGLE_API_KEY;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject o = JObject.Parse(responseBody);

                data = (string)o.SelectToken("results[0].formatted_address");

                return data;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return data;
        }

    }
}
