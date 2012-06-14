
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common
{
    using System;
    using System.Collections.ObjectModel;
    using Microsoft.Maps.MapControl;

    public class GeoCodeRequestCompletedEventArgs : EventArgs
    {
        public GeoCodeRequestCompletedEventArgs()
        {
            Locations = new ObservableCollection<Location>();
        }

        public string DisplayName { get; set; }

        public ObservableCollection<Location> Locations { get; set; }

        public string Address { get; set; }

        public LocationRect BestView { get; set; }
    }
}
