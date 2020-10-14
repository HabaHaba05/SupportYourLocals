using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;
using GMap.NET;
using GMap.NET.MapProviders;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json.Linq;
using SuppLocals.Views;
using Location = Microsoft.Maps.MapControl.WPF.Location;

namespace SuppLocals
{
    public static class MapMethods
    {
        // Returns current user position
        public static async Task<Location> GetLiveLocation(Window window)
        {
            var loc = new Location(0, 0);

            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                var progressDialog = new ProgressDialog
                {
                    Owner = window
                };
                Application.Current.Dispatcher.Invoke(new Action(() => window.IsEnabled = false));
                _ = progressDialog.Dispatcher.BeginInvoke(new Action(() => progressDialog.ShowDialog()));

                // If DesiredAccuracy or DesiredAccuracyInMeters are not set (or value is 0), DesiredAccuracy.Default is used.
                var geolocator = new Geolocator {DesiredAccuracyInMeters = 0};

                // Carry out the operation
                var pos = await geolocator.GetGeopositionAsync();

                loc = new Location(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);

                Application.Current.Dispatcher.Invoke(new Action(() => window.IsEnabled = true));
                progressDialog.Close();
            }
            else
            {
                MessageBox.Show(
                    "We can't reach your location. Please check that the following location privacy are turned on:\n" +
                    "Location for this device... is turned on (not applicable in Windows 10 Mobile\n" +
                    "The location services setting, Location, is turned on\n" +
                    "Under Choose apps that can use your location, your app is set to on\n ");
            }

            return loc;
        }

        //Returns locations collection of route points
        public static LocationCollection GetRoute(Location userLoc, Location finishLoc)
        {
            GoogleMapProvider.Instance.ApiKey = Config.GoogleApiKey;
            var route = GoogleMapProvider.Instance.GetRoute(
                new PointLatLng(userLoc.Latitude, userLoc.Longitude),
                new PointLatLng(finishLoc.Latitude, finishLoc.Longitude), false, false, 15);

            if (route == null)
            {
                MessageBox.Show("Sorry we can't find route to this location");
                return null;
            }

            var points = new LocationCollection();
            var pointsL = new List<Location>(route.Points.ConvertAll(x => new Location(x.Lat, x.Lng)));


            foreach (var x in pointsL) points.Add(x);

            return points;
        }

        //Return distance between two locations
        public static double DistanceBetweenPlaces(Location loc1, Location loc2)
        {
            double R = 6371; // Earth radius km

            var sLat1 = Math.Sin(loc1.Latitude * (Math.PI / 180));
            var sLat2 = Math.Sin(loc2.Latitude * (Math.PI / 180));
            var cLat1 = Math.Cos(loc1.Latitude * (Math.PI / 180));
            var cLat2 = Math.Cos(loc2.Latitude * (Math.PI / 180));
            var cLon = Math.Cos(loc1.Longitude * (Math.PI / 180) - loc2.Longitude * (Math.PI / 180));

            var cosD = sLat1 * sLat2 + cLat1 * cLat2 * cLon;

            var d = Math.Acos(cosD);

            var dist = R * d;

            return dist;
        }

        public static LocationCollection GetCircleVertices(Location Loc, double dRadius)
        {
            var locCollection = new LocationCollection();
            var EarthRadius = 6367; // Earth Radius in Kilometers

            //Convert location to radians based on
            var latitude = Math.PI / 180 * Loc.Latitude;
            var longitude = Math.PI / 180 * Loc.Longitude;

            var d = dRadius / EarthRadius;

            for (var x = 0; x < 360; x++)
            {
                var angle = x * (Math.PI / 180); //radians
                var latRadians = Math.Asin(Math.Sin(latitude) * Math.Cos(d) +
                                           Math.Cos(latitude) * Math.Sin(d) * Math.Cos(angle));
                var lngRadians = longitude + Math.Atan2(Math.Sin(angle) * Math.Sin(d) * Math.Cos(latitude),
                    Math.Cos(d) - Math.Sin(latitude) * Math.Sin(latRadians));

                //Get location of the point
                var pt = new Location(180.0 * latRadians / Math.PI, 180.0 * lngRadians / Math.PI);

                //Add the new calculatied poitn to the collection
                locCollection.Add(pt);
            }

            return locCollection;
        }

        public static async Task<Location> ConvertAddressToLocation(string address)
        {
            var data = new Location();

            try
            {
                var uri = Config.Host + "/maps/api/geocode/json?address=" + address + "language=lt&key=" +
                          Config.GoogleApiKey;

                var client = new HttpClient();
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var o = JObject.Parse(responseBody);

                var lat = (double) o.SelectToken("results[0].geometry.location.lat");
                var lng = (double) o.SelectToken("results[0].geometry.location.lng");


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
            var data = "";

            try
            {
                var uri = Config.Host + "/maps/api/geocode/json?latlng=" + location.Latitude + "," +
                          location.Longitude + "&language=lt&key=" + Config.GoogleApiKey;

                var client = new HttpClient();
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var o = JObject.Parse(responseBody);

                data = (string) o.SelectToken("results[0].formatted_address");

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