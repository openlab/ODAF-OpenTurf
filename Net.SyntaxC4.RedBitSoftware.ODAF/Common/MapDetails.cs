
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common
{
    using Microsoft.Maps.MapControl;

    public class MapDetails
    {
        private Credentials creds;
        private string culture;
        private LocationRect rect;

        public Credentials Credentials { get { return creds; } }
        public string Culture { get { return culture; } }
        public LocationRect BindingArea { get { return rect; } }

        public MapDetails(Credentials creds, string culture, LocationRect rect)
        {
            this.creds = creds;
            this.culture = culture;
            this.rect = rect;
        }
    }
}