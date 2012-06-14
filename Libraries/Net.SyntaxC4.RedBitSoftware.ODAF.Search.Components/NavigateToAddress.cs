
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Components
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Abstractions;
    using Common;
    using GalaSoft.MvvmLight.Command;
    using GalaSoft.MvvmLight.Messaging;
    using Microsoft.Maps.MapControl;

    [Export(typeof(ISearchComponent))]
    public class NavigateToAddress : ISearchComponent, IBingComponent
    {
        public NavigateToAddress()
        {
            WaterMarkText = "Enter an Address";
            DisplayText = "Locate Address";
            Icon = new Uri("/Resources/Images/SearchIcon.png", UriKind.Relative);
            PerformAction = new RelayCommand<RoutedEventArgs>(e =>
            {
                BingMapHelper bingIt = new BingMapHelper();

                MapDetails mapDetails = null;
                Messenger.Default.Register<MapDetails>(this, NotificationTokens.SendMapDetails, d =>
                {
                    mapDetails = d;
                });
                Messenger.Default.Send<object>(null, NotificationTokens.GetMapDetails);

                if (mapDetails != null)
                {
                    // Subscript to Completed Event to get Data after Async Call.
                    bingIt.LocateAddressCompleted += new GeoCodeRequestCompletedEventHandler(bingIt_LocateAddressCompleted);
                    bingIt.LocateAddress(UserInput, mapDetails.Culture, mapDetails.Credentials);
                }
            });
        }

        private void bingIt_LocateAddressCompleted(GeoCodeRequestCompletedEventArgs e)
        {
            // Send Location to Map View.
            if (e != null)
                Messenger.Default.Send<Location>(e.BestView.Center);
        }


        #region ISearchComponent Members
        public string WaterMarkText
        {
            get;
            set;
        }
        public string DisplayText
        {
            get;
            set;
        }
        public string UserInput { get; set; }

        public Uri Icon
        {
            get;
            set;
        }

        public RelayCommand<RoutedEventArgs> PerformAction { get; set; }
        #endregion

        #region IBingComponent Members
        public string Culture { get; set; }
        public LocationRect BindingArea { get; set; }
        public Credentials Credentials { get; set; }
        #endregion
    }
}
