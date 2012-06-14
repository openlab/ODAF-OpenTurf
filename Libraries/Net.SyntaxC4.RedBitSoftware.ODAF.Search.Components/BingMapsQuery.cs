
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
    public class BingMapsQuery : ISearchComponent, IBingComponent
    {
        public BingMapsQuery()
        {
            WaterMarkText = "Search Bing";
            DisplayText = "Search Bing";
            Icon = new Uri("/Resources/Images/SearchIcon.png", UriKind.Relative);
            PerformAction = new RelayCommand<RoutedEventArgs>((e) =>
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
                    bingIt.SearchCompleted += new SearchRequestCompletedEventHandler(bingIt_SearchCompleted);
                    bingIt.Search(UserInput, mapDetails.BindingArea, mapDetails.Culture, mapDetails.Credentials);
                }
            });
        }

        void bingIt_SearchCompleted(SearchRequestCompletedEventArgs e)
        {
            // Send Location to Map
            if (e != null)
                Messenger.Default.Send<MapItemsControl>(e.SearchResultLayer);
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
        public string UserInput
        {
            get;
            set;
        }
        public Uri Icon
        {
            get;
            set;
        }
        public RelayCommand<RoutedEventArgs> PerformAction
        {
            get;
            set;
        }
        #endregion

        #region IBingComponent Members
        public string Culture
        {
            get;
            set;
        }
        public LocationRect BindingArea { get; set; }
        public Credentials Credentials
        {
            get;
            set;
        }
        #endregion
    }
}
