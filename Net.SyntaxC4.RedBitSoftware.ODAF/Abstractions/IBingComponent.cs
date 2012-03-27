
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Abstractions
{
    using Microsoft.Maps.MapControl;

    public interface IBingComponent
    {
        string Culture { get; set; }
        LocationRect BindingArea { get; set; }
        Credentials Credentials { get; set; }
    }
}