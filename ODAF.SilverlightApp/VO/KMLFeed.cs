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
using Microsoft.Maps.MapControl.ExtendedModes;

namespace ODAF.SilverlightApp.VO
{
    public class KMLFeed
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public MapLayer RefMapLayer { get; set; }
        public bool IsRegionData { get; set; }
        public bool IsSystem { get; set; }
    }
}
