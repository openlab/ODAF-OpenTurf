
// Developer: Cory Fowler
// Company: RedBit Software
// Description:

namespace Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using GeocodeService;
    using Microsoft.Maps.MapControl;
    using SearchService;
    using System.Windows.Controls;

    public class BingMapHelper
    {
        private static IEnumerator<Color> colors;

        #region Properties
        private GeocodeServiceClient geocodeClient;
        private GeocodeServiceClient GeocodeClient
        {
            get
            {
                if (null == geocodeClient)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    UriBuilder serviceUri = new UriBuilder("http://dev.virtualearth.net/webservices/v1/GeocodeService/GeocodeService.svc");

                    //Create the Service Client
                    geocodeClient = new GeocodeServiceClient(binding, new EndpointAddress(serviceUri.Uri));
                    geocodeClient.GeocodeCompleted += new EventHandler<GeocodeCompletedEventArgs>(Client_GeoCodeCompleted);
                }
                return geocodeClient;
            }
        }

        private SearchServiceClient searchClient;
        private SearchServiceClient SearchClient
        {
            get
            {
                if (null == searchClient)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    binding.MaxReceivedMessageSize = int.MaxValue;
                    binding.MaxBufferSize = int.MaxValue;

                    UriBuilder serviceUri = new UriBuilder("http://dev.virtualearth.net/webservices/v1/SearchService/SearchService.svc");

                    // Create the Service Client
                    searchClient = new SearchServiceClient(binding, new EndpointAddress(serviceUri.Uri));
                    searchClient.SearchCompleted += new EventHandler<SearchCompletedEventArgs>(Client_SearchCompleted);
                }
                return searchClient;
            }
        }
        #endregion

        #region static members
        public static MapLayer CreateMapLayer()
        {
            return new MapLayer();
        }
        public static MapItemsControl CreateMapItemsControl()
        {
            return new MapItemsControl();
        }
        public static MapPolygon CreateMapPolygon()
        {
            return new MapPolygon();
        }

        private static Pushpin CreatePushPin(string name, Uri backgroundUri, Location location)
        {
            Pushpin pin = new Pushpin();

            pin.Name = name;
            pin.Location = location;
            try
            {
                pin.Background = new ImageBrush() { ImageSource = new BitmapImage(backgroundUri) };
            }
            catch (Exception)
            {
                // Could implement a Default image here.
                //pin.Background = new ImageBrush() {};
            }

            return pin;
        }
        private static Pushpin CreatePushPin(string tag, Location location)
        {
            Pushpin pin = new Pushpin();

            pin.Location = location;
            pin.Tag = tag;

            return pin;
        }

        private static SolidColorBrush CreateSolidBrushWithCurrentColor()
        {
            // If System Colors aren't loaded, load them up.
            if (colors == null) LoadColorPallette();

            TryNextColor();
            if (colors.Current == Colors.Transparent)
                TryNextColor();

            // Make a new brush with the current color.
            return new SolidColorBrush(colors.Current);
        }

        private static void TryNextColor()
        {
            /*
             * Check to make sure we're not at the beginning
             * or the end of the Enumerator. If at the end,
             * reset to recycle through the colors.
             */
            if (!colors.MoveNext())
            {
                colors.Reset();
                colors.MoveNext();
            }
        }
        private static void LoadColorPallette()
        {
            List<Color> colorList = new List<Color>();
            colorList.Add(Colors.Blue);
            colorList.Add(Colors.Brown);
            colorList.Add(Colors.Cyan);
            colorList.Add(Colors.DarkGray);
            colorList.Add(Colors.Green);
            colorList.Add(Colors.LightGray);
            colorList.Add(Colors.Magenta);
            colorList.Add(Colors.Orange);
            colorList.Add(Colors.Purple);
            colorList.Add(Colors.Red);
            colorList.Add(Colors.Yellow);
            colors = colorList.GetEnumerator();
        }
        #endregion

        #region public members
        /// <summary>
        /// Locates an Address and Provides the Address details for Map Repositioning
        /// </summary>
        /// <param name="address">User Provided Address details</param>
        /// <param name="culture">the current culture of the map</param>
        /// <param name="credentials">Your Bing API Credentials</param>
        /// /// <remarks>Retrieve Data by creating an Event Handler for LocateAddressCompleted</remarks>
        public void LocateAddress(string address, string culture, Credentials credentials)
        {
            GeocodeRequest request = new GeocodeRequest();
            request.Culture = culture;
            request.Query = address;

            // Don't raise exceptions.
            request.ExecutionOptions = new GeocodeService.ExecutionOptions();
            request.ExecutionOptions.SuppressFaults = true;

            // Only accept results with high confidence.
            request.Options = new GeocodeOptions();
            request.Options.Filters = new ObservableCollection<FilterBase>();

            ConfidenceFilter filter = new ConfidenceFilter();
            filter.MinimumConfidence = GeocodeService.Confidence.High;
            request.Options.Filters.Add(filter);

            //Pass in credentials for web services call.
            request.Credentials = credentials;

            // Make asynchronous call to fetch the data
            GeocodeClient.GeocodeAsync(request, address);
        }
        /// <summary>
        /// Performs a search against the Bing Maps Search API
        /// </summary>
        /// <param name="query">User provided Search term</param>
        /// <param name="searchBoundaries">The BoundingRectangle of the map.</param>
        /// <param name="culture">The Culture of the map</param>
        /// <param name="credentials">Your API Credentials</param>
        /// <remarks>Retrieve Data by creating an Event Handler for SearchCompleted</remarks>
        public void Search(string query, LocationRect searchBoundaries, string culture, Credentials credentials)
        {
            SearchRequest request = new SearchRequest();
            request.Culture = culture;
            request.Query = query;

            // Don't raise exceptions.
            request.ExecutionOptions = new SearchService.ExecutionOptions();
            request.ExecutionOptions.SuppressFaults = true;

            LocationRect mapBounds = new LocationRect(searchBoundaries);
            request.UserProfile = new SearchService.UserProfile();
            request.UserProfile.MapView = mapBounds;

            //Pass in credentials for web services call.
            request.Credentials = credentials;

            //execute the request
            SearchClient.SearchAsync(request);
        }
        #endregion

        #region Callbacks
        private void Client_GeoCodeCompleted(object sender, GeocodeCompletedEventArgs e)
        {
            GeoCodeRequestCompletedEventArgs args = null;

            if (e.Result.ResponseSummary.StatusCode == GeocodeService.ResponseStatusCode.Success && e.Result.Results.Count > 0)
            {
                args = new GeoCodeRequestCompletedEventArgs();

                args.DisplayName = e.Result.Results[0].DisplayName;
                args.Address = e.Result.Results[0].Address.FormattedAddress;
                args.BestView = e.Result.Results[0].BestView;

                foreach (var loc in e.Result.Results[0].Locations)
                {
                    args.Locations.Add(new Location(loc.Latitude, loc.Longitude, loc.Altitude));
                }
            }

            OnGeoCodeRequestCompleted(args);
        }
        private void Client_SearchCompleted(object sender, SearchCompletedEventArgs e)
        {
            SearchRequestCompletedEventArgs args = null;

            try
            {
                if (e.Result.ResponseSummary.StatusCode == SearchService.ResponseStatusCode.Success)
                {
                    IDictionary<string, SearchResultBase> searchResults = new Dictionary<string, SearchResultBase>();
                    IList<Location> locations = new List<Location>();
                    MapItemsControl searchResultMapLayer = CreateMapItemsControl();

                    if ((e.Result != null) && (e.Result.ResultSets.Count > 0))
                    {
                        foreach (SearchResultBase result in e.Result.ResultSets[0].Results)
                        {
                            searchResults.Add(result.Id, result);

                            if (result.LocationData.Locations.Count > 0)
                            {
                                Location location = new Location(result.LocationData.Locations[0]);
                                Pushpin pin = CreatePushPin(result.Id, location);
                                ToolTipService.SetToolTip(pin, result.Name);
                                searchResultMapLayer.Items.Add(pin);
                            }
                        }

                        args = new SearchRequestCompletedEventArgs();

                        args.SearchResults = searchResults;
                        args.SearchResultLayer = searchResultMapLayer;
                        args.Locations = locations;
                        args.BindingArea = e.Result.ResultSets[0].SearchRegion.BoundingArea as LocationRect;
                    }
                }
            }
            catch (Exception)
            {

            }

            OnSearchRequestCompleted(args);
        }
        #endregion

        #region events
        public event GeoCodeRequestCompletedEventHandler LocateAddressCompleted;
        private void OnGeoCodeRequestCompleted(GeoCodeRequestCompletedEventArgs e)
        {
            if (LocateAddressCompleted != null)
                LocateAddressCompleted(e);
        }
        public event SearchRequestCompletedEventHandler SearchCompleted;
        private void OnSearchRequestCompleted(SearchRequestCompletedEventArgs e)
        {
            if (SearchCompleted != null)
                SearchCompleted(e);
        }
        #endregion
    }
}