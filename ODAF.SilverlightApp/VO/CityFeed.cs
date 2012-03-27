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

using System.Json;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ODAF.SilverlightApp.VO
{
    public class MapZoomLoc
    {
        public Double Lat { get; set; }
        public Double Lon { get; set; }
        public Double Zoom { get; set; }
    }

    public class CityFeed
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }
        public String BoundaryPolygon { get; set; }
    }
}
