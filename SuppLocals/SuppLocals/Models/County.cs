using Microsoft.Maps.MapControl.WPF;
using SuppLocals.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuppLocals
{
    public class County:BaseViewModel
    {
        public County(LocationCollection locations)
        {
            Locations = locations;
        }

        public LocationCollection Locations {
            get;
            set;
        }
    }
}
