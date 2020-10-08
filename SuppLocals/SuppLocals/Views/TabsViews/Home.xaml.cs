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
using System.Windows.Shapes;

namespace SuppLocals.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            //By default
            InitializeComponent();
            MyMap.CredentialsProvider = Config.BING_API_KEY;
        }
    }
}
