
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Maps.MapControl;
    using SearchService;

    public class SearchRequestCompletedEventArgs : EventArgs
    {
        public SearchRequestCompletedEventArgs()
        {
            Locations = new List<Location>();
            SearchResults = new Dictionary<string, SearchResultBase>();
        }

        public IList<Location> Locations { get; set; }

        public LocationRect BindingArea { get; set; }

        public MapItemsControl SearchResultLayer { get; set; }

        public IDictionary<string, SearchResultBase> SearchResults { get; set; }
    }
}
