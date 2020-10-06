using System;
using Microsoft.Maps.MapControl.WPF;
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
using SuppLocals.TabsModel;
using System.Windows.Shapes;

namespace SuppLocals.Tabs
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public double circleRadius = 0;
        public List<Vendor> VendorsList;

        public Home()
        {
            //By default
            InitializeComponent();

           

            //Activates the + and – keys to allow the user to manually zoom in and out of the map
            myMap.Focus();
            myMap.CredentialsProvider = Config.BING_API_KEY;
           
        }
    }
}
