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

namespace ODAF.SilverlightApp.VO
{
    public class PlaceMark
    {
        public string Id { get; set; }
        public string FeedId { get; set; }
        public bool IsSystem { get; set; }

        public Location Location { get; set; }

        public string Coords
        {
            set
            {
                double lat = double.Parse(value.Split(',')[1]);
                double lon = double.Parse(value.Split(',')[0]);
                this.Location = new Location(lat, lon);
            }
        }
        // this is the description we get from the KML feed info
        // server overriden version is in Summary.Description
        public string name { get; set; }
        public int ratingCount;
        public int ratingTotal;
        public int commentCount;

        public UserComment[]  Comments { get; set; }
        public PointDataSummary Summary { get; set; }
    }
}
