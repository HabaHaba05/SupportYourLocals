using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Location = Microsoft.Maps.MapControl.WPF.Location;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Linq;

namespace SuppLocals
{

    public partial class MainWindow : Window
    {
        //The user
        public User ActiveUser;

        public List<Vendor> VendorsList;
        public List<User> UsersList;
        public List<Review> ReviewsList;


        public double circleRadius = 0;

       
        public MainWindow(User user)
        {
            //By default
            InitializeComponent();

            ActiveUser = user;

            using(AppDbContext db = new AppDbContext())
            {
                VendorsList = db.Vendors.ToList();
                UsersList = db.Users.ToList();
                ReviewsList = db.Reviews.ToList();
            }

            //Activates the + and – keys to allow the user to manually zoom in and out of the map
            myMap.Focus();
            myMap.CredentialsProvider = Config.BING_API_KEY;


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
            UpdateMapChildrens(null,null);
        }

        private void UpdateMapChildrens(object sender, SelectionChangedEventArgs args)
        {
            //Update VendorsList
            using (AppDbContext db = new AppDbContext())
            {
                VendorsList = db.Vendors.ToList();
            }

            myMap.Children.Clear();

            foreach (Vendor vendor in VendorsList)
            {
                Location loc = new Location(vendor.Latitude,vendor.Longitude);

                if (!(bool)filterDistanceCheck.IsChecked ||
                    MapMethods.DistanceBetweenPlaces(loc, ActiveUser.Location) <= circleRadius)
                {
                    Pushpin pushpin = new Pushpin();
                    pushpin.MouseUp += PinClicked;
                    pushpin.Tag = vendor;

                    pushpin.Location = new Location(loc.Latitude, loc.Longitude);
                    //pushpin.Background = vendor.color;
                    myMap.Children.Add(pushpin);
                }
            }

            if ((bool)filterDistanceCheck.IsChecked)
            {
                //Filter circle
                MapMethods.DrawCircle(MapMethods.GetCircleVertices(ActiveUser.Location, circleRadius), myMap, Color.FromRgb(240, 248, 255));
            }
        }


        #region Filters change

        public async void DistanceFilterChecked(object sender, RoutedEventArgs e)
        {

            if (ActiveUser.Location == null)
            {
                ActiveUser.Location = await MapMethods.GetLiveLocation(this);

                if (ActiveUser.Location == null)
                {
                    filterDistanceCheck.IsChecked = false;
                    return;
                }
            }
            distanceFilterPanel.Visibility = Visibility.Visible;
            circleRadius = radiusSlider.Value;
            myMap.Center = ActiveUser.Location;
            UpdateMapChildrens(null, null);
        }
        private void RadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            circleRadius = (float)radiusSlider.Value;
            if (myMap != null)
                UpdateMapChildrens(null, null);
        }

        public void DistanceFilterUnchecked(object sender, RoutedEventArgs e)
        {
            circleRadius = 0;
            distanceFilterPanel.Visibility = Visibility.Hidden;
            UpdateMapChildrens(null, null);


        }


        #endregion


        #region Vendor About Tab

        public void Hide_BtnClick(object sender, RoutedEventArgs e)
        {
            UpdateMapChildrens(null, null);
            selectedServiceInfoGrid.Visibility = Visibility.Collapsed;
            Grid.SetColumnSpan(myMap, 3);
        }

        private void PinClicked(object sender, MouseButtonEventArgs e)
        {
            Pushpin p = sender as Pushpin;
            Vendor vendor = p.Tag as Vendor;

            selectedServiceTitle.Text = vendor.Title;
            selectedServiceAbout.Text = vendor.About;
            selectedServiceAddress.Text = $"Address: {vendor.Address} \nLat: {vendor.Latitude} \nLong: { vendor.Longitude} ";
            review_Btn.Tag = vendor;
            findRoute_Btn.Tag = vendor;


            selectedServiceInfoGrid.Visibility = Visibility.Visible;
            Grid.SetColumnSpan(myMap, 2);
        }

        public void Review_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Vendor vendor = b.Tag as Vendor;

            ReviewsWindow reviewsWindow = new ReviewsWindow(vendor, ActiveUser);
            reviewsWindow.Show();
        }

        public async void FindRoute_BtnClick(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            Vendor service = b.Tag as Vendor;
            
            if(ActiveUser.Location == null)
            {
                ActiveUser.Location = await MapMethods.GetLiveLocation(this);
            }
            if (ActiveUser.Location != null)
            {
                LocationCollection tempRouteLine = MapMethods.GetRoute(ActiveUser.Location, new Location(service.Latitude,service.Longitude));

                if (tempRouteLine == null)
                {
                    MessageBox.Show("Because we can't find you live location, we cant calculate route for You. Sorry");
                    return;
                }

                MapPolyline routeLine = new MapPolyline()
                {
                    Locations = tempRouteLine,
                    Stroke = new SolidColorBrush(Colors.Green),
                    StrokeThickness = 5
                };

                myMap.Children.Add(routeLine);
            }


        }

        #endregion


        #region Menu methods

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (sPan.Visibility == Visibility.Collapsed)
            {
                sPan.Visibility = Visibility.Visible;
                (sender as Button).Content = "X";
            }
            else
            {
                sPan.Visibility = Visibility.Collapsed;
                (sender as Button).Content = "☰ Filters";
            }
        }

        private void FilterClick(object sender, RoutedEventArgs e)
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

        private void TabClicked(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            ThicknessAnimation ta = new ThicknessAnimation
            {
                From = TabCursor.Margin,
                To = new Thickness((95 * index), 0, 0, 10),
                Duration = new Duration(TimeSpan.FromSeconds(0.2))
            };
            TabCursor.BeginAnimation(Button.MarginProperty, ta);
        }

        private void HyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }


        #endregion

        private void CreateVendorClick(object sender, RoutedEventArgs e)
        {
            CreateVendor cv = new CreateVendor(ActiveUser);
            cv.ShowDialog();
            UpdateMapChildrens(null,null);
        }
    }

}