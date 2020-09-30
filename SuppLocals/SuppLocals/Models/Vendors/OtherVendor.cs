using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows.Media;

namespace SuppLocals.Services
{
    public class OtherVendor : Vendor
    {
        public OtherVendor(String address, Location location)
        {
            color = new SolidColorBrush(Color.FromRgb(0, 0, 255));
            this.address = address;
            this.location = location;
        }
    }
}
