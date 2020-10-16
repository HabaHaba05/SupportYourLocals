using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;
using SuppLocals.ViewModels;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private readonly Location _myMapCenter;
        private User activeUser;

        public Home()
        {
            //By default
            InitializeComponent();
            _myMapCenter = MyMap.Center;

            MyMap.CredentialsProvider = Config.BING_API_KEY;

            // Disable zooming and "moving" map
            MyMap.MouseDoubleClick += (s, e) => e.Handled = true;
            MyMap.MouseWheel += (s, e) => e.Handled = true;
           /* MyMap.ViewChangeOnFrame += (s, e) =>
            {             
                if (!MyMap.Center.Equals(_myMapCenter))
                {
                    MyMap.Center = _myMapCenter;
                }
            };*/

        }

        private void Pushpin_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var selectedVendor = (sender as FrameworkElement).DataContext as Vendor;
            SelectedServiceInfoGrid.Visibility = Visibility.Visible;
            var x = (HomeVM)this.DataContext;
            x.SelectedVendor = selectedVendor;
        }

        private void RadiusSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            activeUser ??= ((HomeVM)this.DataContext).activeUser;

            LocationCollection circleVertices = MapMethods.GetCircleVertices(activeUser.Location, RadiusSlider.Value);

            MapPolygon circle = new MapPolygon
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

            if (selectedArea.HasChildren && selectedArea.Children==null)
            {
                selectedArea.Children = selectedArea.ParseMunicipalities();

            }
            else if (!selectedArea.HasChildren)
            {
               //Show boundaries around selected area
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

        private void MyMap_ViewChangeEnd(object sender, MapEventArgs e)
        {
            Debug.WriteLine(MyMap.Center);
            Debug.WriteLine(MyMap.ZoomLevel);
        }

    }
}
