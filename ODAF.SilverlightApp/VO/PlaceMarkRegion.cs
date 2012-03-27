using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Maps.MapControl;
using System.Collections.Generic;

namespace ODAF.SilverlightApp.VO
{
    public class PlaceMarkRegion
    {
        public string description
        {
            get;
            set;
        }
        public List<Location> coordList { get; set; }
        public string coords
        {
            get { return coordList.Count.ToString(); }
            set
            {
                coordList = new List<Location>();
                // Note kml 2.2 defines coord like this: "D,D,D D,D,D D,D,D ..."
                // While 2.1 defines them like this: "D,D,D,D,D,D,D ..."

                string temp = value.Replace("\n", "").Replace(", ",",").Replace(" ", ",");

                string[] strCoords = temp.Split(',');
                //System.Diagnostics.Debug.WriteLine("strCoordsstrCoords:" + strCoords.Length);
                for (int n = 0; (n + 3) < strCoords.Length; n += 3)
                {
                    Location loc = new Location(double.Parse(strCoords[n + 1]), double.Parse(strCoords[n]), double.Parse(strCoords[n + 2]));
                    coordList.Add(loc);
                }
            }
        }
    }
}
