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
using System.ServiceModel;
using Microsoft.Maps.MapControl;
using System.Collections;
using ODAF.SilverlightApp.GeocodeService;

namespace ODAF.SilverlightApp.CloudService
{
    public delegate void SearchResultUpdateEventHandler(ServiceController sender);


    public class ServiceController
    {
        public event SearchResultUpdateEventHandler SearchResultUpdate;

        public string BaseURL { get; set; }
        public SummariesController Summaries { get; set; }
        public UserController User{ get; set; }
        public FeedController Feeds { get; set; }

       // public  SearchResults { get; set; }

        public System.Collections.ObjectModel.ObservableCollection<GeocodeResult> SearchResults { get; set; }


        public ServiceController(string baseURL)
        {
            BaseURL = baseURL;

            Summaries = new SummariesController();
            Summaries.baseController = this;
            Summaries.BaseURL = BaseURL;

            User = new UserController();
            User.BaseURL = BaseURL;

            Feeds = new FeedController();
            Feeds.baseController = this;
            Feeds.BaseURL = BaseURL;

            Feeds.GetFeedList();
        }

        public void GeoSearch(string keywords,Map mapView)
        {
            UriBuilder serviceUri = new UriBuilder("http://dev.virtualearth.net/webservices/v1/GeocodeService/GeocodeService.svc");
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;


            GeocodeServiceClient geoSearchClient = new GeocodeServiceClient(binding, new EndpointAddress(serviceUri.Uri));
            GeocodeRequest req = new GeocodeRequest();
            req.Query = keywords;
            req.Credentials = new Credentials();
            req.Credentials.ApplicationId = "AiN8LzMeybPbj9CSsLqgdeCG86jg08SJsjm7pms3UNtNTe8YJHINtYVxGO5l4jBj"; // TODO: Get from Resource

            req.ExecutionOptions = new ExecutionOptions();
            req.ExecutionOptions.SuppressFaults = true;

            LocationRect mapControlView = mapView.BoundingRectangle;
            LocationRect mapBounds = new LocationRect(mapControlView);

            req.Culture = mapView.Culture;
            req.UserProfile = new UserProfile();
            req.UserProfile.MapView = mapBounds;


            geoSearchClient.GeocodeCompleted += new EventHandler<GeocodeCompletedEventArgs>(geoSearchClient_GeocodeCompleted);
            geoSearchClient.GeocodeAsync(req);

        }

        void geoSearchClient_GeocodeCompleted(object sender, GeocodeCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else
            {
                GeocodeResponse searchResponse = e.Result;
                SearchResults = searchResponse.Results;
                SearchResultUpdate(this);
            }
        }

    }
}
