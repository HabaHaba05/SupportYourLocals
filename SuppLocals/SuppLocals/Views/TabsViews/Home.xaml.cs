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
        private User activeUser;

        public Home()
        {
            //By default
            InitializeComponent();
            MyMap.CredentialsProvider = Config.BING_API_KEY;
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
    }
}
