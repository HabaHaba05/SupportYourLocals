using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json.Linq;
using SuppLocals.Classes.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SuppLocals
{
    public class Area
    {
        public Area Parent;
        public List<Area> Children { get; set; }
        public LocationCollection Locations { get; set; }
        public Location Center { get; set; }
        public double Zoom { get; set; }
        public string Name { get; set; }
        public List<Vendor> Vendors { get; set; } = new List<Vendor>();

        public bool HasChildren;

        public int Level;


        //Constructor for country (root). This object won't have parent
        public Area(Location center, double zoom)
        {
            Center = center;
            Zoom = zoom;
            Parent = null;
            HasChildren = true;
            Name = "Lietuva";
            Level = 0;
        }


        public Area(string name, LocationCollection locations,Location center, double zoom, Area parent, bool hasChildren)
        {
            Locations = locations;
            Center = center;
            Zoom = zoom;
            Name = name;
            Parent = parent;
            HasChildren = hasChildren;
            Level = parent.Level + 1;
        }


        public List<Area> ParseCounties()
        {
            List<Area> counties = new List<Area>();
            
            foreach (var name in Enum.GetNames(typeof(CountiesNames)))
            {
                JObject o1 = JObject.Parse(File.ReadAllText((string)(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent + $"/Assets/CountiesJsons/{name}County.json")));
                var locations = o1.SelectToken("locations");
                var center = o1.SelectToken("center");
                var zoom = o1.SelectToken("zoom");
                LocationCollection locColl = new LocationCollection();

                for (int i = 0; i < locations.Count(); i++)
                {
                    locColl.Add(new Location((double)locations[i][1], (double)locations[i][0]));
                }

                var centerLoc = new Location((double)center[1], (double)center[0]);

                var municipality = o1.SelectToken("municipality");

                var hasChild = municipality.Count() != 0;

                counties.Add(new Area(name, locColl, centerLoc, (double)zoom, this, hasChild));
            }

            return counties;
        }

        public List<Area> ParseMunicipalities()
        {
            JObject o1 = JObject.Parse(File.ReadAllText((string)(Directory.GetParent(Environment.CurrentDirectory).Parent.Parent + $"/Assets/CountiesJsons/{Name}County.json")));
            var municipality = o1.SelectToken("municipality");
            if (municipality.Count() == 0)
            {
                return null;
            }

            List<Area> municipalities = new List<Area>();

            for (int i = 0; i < municipality.Count(); i++)
            {
                var name = o1.SelectToken($"municipality[{i}].name");
                var locations = o1.SelectToken($"municipality[{i}].locations");
                var center = o1.SelectToken($"municipality[{i}].center");
                var zoom = o1.SelectToken($"municipality[{i}].zoom");

                LocationCollection locColl = new LocationCollection();
                for (int j = 0; j < locations.Count(); j++)
                {
                    locColl.Add(new Location((double)locations[j][1], (double)locations[j][0])); 
                }
                municipalities.Add(new Area(name.ToString(), locColl, new Location((double)center[1], (double)center[0]), (double)zoom, this, false));
            }

            return municipalities;
        }


    }
}
