using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xaml.Schema;
using Microsoft.Maps.MapControl.WPF;
using SuppLocals.ViewModels;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private User _activeUser;

        public Home()
        {
            //By default
            InitializeComponent();

            MyMap.CredentialsProvider = Config.Bing_Api_Key;

            // Disable zoom
            MyMap.MouseDoubleClick += (s,e)=>e.Handled=true;
            MyMap.MouseWheel += Map_MouseWheelOff;

        }


        private void Map_MouseWheelOff(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
        }

        private void Map_MouseWheelOn(object sender, MouseWheelEventArgs e)
        {
            var dataContext = (HomeVM)this.DataContext;
            MyMap.ZoomLevel = Math.Max(dataContext.SelectedArea.Zoom, MyMap.ZoomLevel+Math.Sign(e.Delta)*0.25 );
            e.Handled = true;
        }

        private void Pushpin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var selectedVendor = (sender as FrameworkElement)?.DataContext as Vendor;
            SelectedServiceInfoGrid.Visibility = Visibility.Visible;
            var x = (HomeVM)this.DataContext;
            x.SelectedVendor = selectedVendor;
        }

        private void RadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _activeUser ??= ((HomeVM)this.DataContext).ActiveUser;

            var circleVertices = MapMethods.GetCircleVertices(_activeUser.Location, RadiusSlider.Value);

            var circle = new MapPolygon
            {
                Fill = new SolidColorBrush(Colors.AliceBlue),
                Stroke = new SolidColorBrush(Colors.Black),
                StrokeThickness = 1,
                Opacity = 0.65,
                Locations = circleVertices
            };

            CircleLayer.Children.Clear();
            CircleLayer.Children.Add(circle);
        }

        private void MapPolygon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var selectedArea = (sender as FrameworkElement).DataContext as Area;
            var dataContext = (HomeVM)this.DataContext;

            SelectedBoundary.Children.Clear();

            if (!selectedArea.HasChildren)
            {
                MapPolyline boundary = new MapPolyline
                {
                    Stroke = new SolidColorBrush(Colors.Red),
                    Opacity = 0.85,
                    Locations = selectedArea.Locations
                };
                SelectedBoundary.Children.Add(boundary);
                //Enable map zoom
                MyMap.MouseWheel -= Map_MouseWheelOff;
                MyMap.MouseWheel += Map_MouseWheelOn;
            }

            dataContext.SelectedArea = selectedArea;

            MyMap.Center = selectedArea.Center;
        }

        private void MapPolygon_MouseLeave(object sender, MouseEventArgs e)
        {
            var polygon = sender as MapPolygon;
            polygon.Fill = new SolidColorBrush(Colors.DarkCyan);
        }

        private void MapPolygon_MouseEnter(object sender, MouseEventArgs e)
        {
            var polygon = sender as MapPolygon;
            polygon.Fill = new SolidColorBrush(Colors.Yellow);
        }

        private void JumpBackBtn_Click(object sender, RoutedEventArgs e)
        {
            var dataContext = (HomeVM)this.DataContext;
            MyMap.Center = dataContext.SelectedArea.Parent.Center;
            SelectedBoundary.Children.Clear();

            //Disable zoom
            MyMap.MouseWheel += Map_MouseWheelOff;
            MyMap.MouseWheel -= Map_MouseWheelOn;
        }

        private void DistanceFilterCB_Click(object sender, RoutedEventArgs e)
        {
            if (!(bool)DistanceFilterCB.IsChecked)
            {
                CircleLayer.Children.Clear();
            }
            else if(_activeUser != null)
            {
                RadiusSlider_ValueChanged(null, null);
            }
        }
    }
}
