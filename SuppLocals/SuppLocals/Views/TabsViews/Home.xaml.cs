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
        private User _activeUser;

        public Home()
        {
            //By default
            InitializeComponent();
            MyMap.CredentialsProvider = Config.Bing_Api_Key;
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
    }
}
