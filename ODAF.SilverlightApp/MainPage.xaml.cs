using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Syndication;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Maps.MapControl;
using ODAF.SilverlightApp.CloudService;
using ODAF.SilverlightApp.UserControls;
using ODAF.SilverlightApp.VO;

#region Search Component Imports
using GalaSoft.MvvmLight.Messaging;
using Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common;
using System.ComponentModel;
#endregion

namespace ODAF.SilverlightApp
{

     /*	"{
      * \"results\":[
      * {"location":"\u00dcT: 49.241023,-123.099394",
      * \"profile_image_url\":\"http://a1.twimg.com/profile_images/641097698/IMGP1724_normal.jpg\",
      * \"created_at\":\"Thu, 04 Feb 2010 10:17:31 +0000\",
      * \"from_user\":\"bbbrittney\",
      * \"to_user_id\":null,
      * \"text\":\"Ceilies with shan!\",
      * \"id\":8626944454,
      * \"from_user_id\":16380368,
      * \"geo\":null,
      * \"iso_language_code\":\"en\",
      * \"source\":\"&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;\"}
      * 
      * ,{\"location\":\"iPhone: 49.228973,-123.007278\",
      * \"profile_image_url\":\"http://a1.twimg.com/profile_images/489349396/ME_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 10:09:57 +0000\",\"from_user\":\"nicmic62\",\"to_user_id\":null,\"text\":\"http://www.upscalehype.com/wp-content/uploads/2010/02/Swizz-CL.jpg My god man!! Look at these babies! &lt;3\",\"id\":8626791370,\"from_user_id\":9682170,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://echofon.com/&quot; rel=&quot;nofollow&quot;&gt;Echofon&lt;/a&gt;\"},{\"location\":\"\\u00dcT: 49.265457,-123.030732\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/631146574/Picture_308_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 10:00:46 +0000\",\"from_user\":\"IamCanadaGreen\",\"to_user_id\":null,\"text\":\"The Latest News About Green Living  From Iamcanadagreen http://tinyurl.com/y8eoryu\",\"id\":8626604120,\"from_user_id\":90452804,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://apiwiki.twitter.com/&quot; rel=&quot;nofollow&quot;&gt;API&lt;/a&gt;\"},{\"location\":\"\\u00dcT: 49.265457,-123.030732\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/631146574/Picture_308_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 10:00:45 +0000\",\"from_user\":\"IamCanadaGreen\",\"to_user_id\":null,\"text\":\"The Latest News About Living Green  From Iamcanadagreen http://tinyurl.com/ybls6kz\",\"id\":8626603723,\"from_user_id\":90452804,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://apiwiki.twitter.com/&quot; rel=&quot;nofollow&quot;&gt;API&lt;/a&gt;\"},{\"location\":\"iPhone: 49.254196,-123.016579\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/644214085/meskla_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:48:37 +0000\",\"from_user\":\"DJSKLA\",\"to_user_id\":null,\"text\":\"I have a crush on @karleighsedar call me the toof fairy baby #wink @republic #let'sgo\",\"id\":8626360056,\"from_user_id\":23074939,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://www.tweetdeck.com/&quot; rel=&quot;nofollow&quot;&gt;TweetDeck&lt;/a&gt;\"},{\"location\":\"iPhone: 49.254196,-123.016579\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/644214085/meskla_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:46:14 +0000\",\"from_user\":\"DJSKLA\",\"to_user_id\":5883570,\"text\":\"@DJ_Johnny_Omega @IAMRUBY @Ian_Ross_ @Lauren_Horton_ @s_smalls this is how we do!  #papi #fuego @karleighsedar\",\"id\":8626313538,\"from_user_id\":23074939,\"to_user\":\"DJ_Johnny_Omega\",\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://www.tweetdeck.com/&quot; rel=&quot;nofollow&quot;&gt;TweetDeck&lt;/a&gt;\"},{\"location\":\"\\u00dcT: 49.225895,-123.106161\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/271799240/katiekang_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:43:13 +0000\",\"from_user\":\"KatieKang\",\"to_user_id\":4929176,\"text\":\"@pcordero so jealous you have a snuggie... I want.\",\"id\":8626253693,\"from_user_id\":14981572,\"to_user\":\"pcordero\",\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;\"},{\"location\":\"iPhone: 49.213303,-123.124130\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/63703417/tapastree_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:43:12 +0000\",\"from_user\":\"steffy_h\",\"to_user_id\":null,\"text\":\"Yes I did it! A fitness milestone. Now here comes phase II http://twitpic.com/11b7zi\",\"id\":8626253344,\"from_user_id\":2320694,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://echofon.com/&quot; rel=&quot;nofollow&quot;&gt;Echofon&lt;/a&gt;\"},{\"location\":\"iPhone: 49.254196,-123.016579\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/644214085/meskla_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:41:44 +0000\",\"from_user\":\"DJSKLA\",\"to_user_id\":5883570,\"text\":\"@DJ_Johnny_Omega @IAMRUBY @Ian_Ross_\",\"id\":8626224320,\"from_user_id\":23074939,\"to_user\":\"DJ_Johnny_Omega\",\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://www.tweetdeck.com/&quot; rel=&quot;nofollow&quot;&gt;TweetDeck&lt;/a&gt;\"},{\"location\":\"iPhone: 49.228973,-123.007278\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/489349396/ME_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:38:48 +0000\",\"from_user\":\"nicmic62\",\"to_user_id\":null,\"text\":\"dude....Duuuude...DUUUUUUUDE!\",\"id\":8626165611,\"from_user_id\":9682170,\"geo\":null,\"iso_language_code\":\"nl\",\"source\":\"&lt;a href=&quot;http://echofon.com/&quot; rel=&quot;nofollow&quot;&gt;Echofon&lt;/a&gt;\"},{\"location\":\"\\u00dcT: 49.241096,-123.09931\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/639001454/16852_390303435021_686170021_10413374_4449128_na_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:38:39 +0000\",\"from_user\":\"jamie_saywhaatt\",\"to_user_id\":null,\"text\":\"needs chapstick A to the SAP!!!\",\"id\":8626162773,\"from_user_id\":80702240,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;\"},{\"location\":\"1965 Main Street, Vancouver BC\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/541103518/vivo_logo_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:25:39 +0000\",\"from_user\":\"vivomediaarts\",\"to_user_id\":null,\"text\":\"VIVO's Dinka Pignon at Pecha Kucha in Vancouver - Video at: http://bit.ly/bwoXoA\",\"id\":8625899018,\"from_user_id\":62257887,\"geo\":null,\"iso_language_code\":\"it\",\"source\":\"&lt;a href=&quot;http://twitterrific.com&quot; rel=&quot;nofollow&quot;&gt;Twitterrific&lt;/a&gt;\"},{\"location\":\"iPhone: 49.234119,-123.128563\",\"profile_image_url\":\"http://a1.twimg.com/profile_images/530690700/Fuuuuuu_normal.JPG\",\"created_at\":\"Thu, 04 Feb 2010 09:17:26 +0000\",\"from_user\":\"saintjin\",\"to_user_id\":null,\"text\":\"RT @georgiastraight : Vanoc takes down video called &quot;incredibly offensive&quot; by Jewish torchbearer http://bit.ly/aljENn VANOC=head up own ass\",\"id\":8625734029,\"from_user_id\":25993005,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;\"},{\"location\":\"\\u00dcT: 49.206471,-123.129752\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/379485129/4477_190129425415_511705415_6812164_72721_n_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 09:05:51 +0000\",\"from_user\":\"limmeister\",\"to_user_id\":null,\"text\":\"Ackk. Too many things upon my mind. I need to sleep. It's gonna be a loong day tomorrow. Lord, I need youu.\",\"id\":8625499771,\"from_user_id\":22434105,\"geo\":null,\"iso_language_code\":\"en\",\"source\":\"&lt;a href=&quot;http://socialscope.net&quot; rel=&quot;nofollow&quot;&gt;SocialScope&lt;/a&gt;\"},{\"location\":\"iPhone: 49.256226,-123.114960\",\"profile_image_url\":\"http://a3.twimg.com/profile_images/669690179/keely_normal.jpg\",\"created_at\":\"Thu, 04 Feb 2010 08:55:58 +0000\",\"from_user\":\"StAlison\",\"to_user_id\":601388,\"text\":\"@nickmolnar nice tin\",\"id\":8625292333,\"from_user_id\":11574357,\"to_user\":\"nickmolnar\",\"geo\":null,\"iso_language_code\":\"es\",\"source\":\"&lt;a href=&quot;http://twitter.com/&quot;&gt;web&lt;/a&gt;\"}],\"max_id\":8626944454,\"since_id\":8317242441,\"refresh_url\":\"?since_id=8626944454&q=\",\"next_page\":\"?page=2&max_id=8626944454&geocode=49.2308851076672%2C-123.07209250293%2C5.0km&q=\",\"results_per_page\":15,\"page\":1,\"completed_in\":0.066041,\"warning\":\"adjusted since_id to 8317242441 (2010-01-28 10:00:00 UTC), requested since_id was older than allowed -- since_id removed for pagination.\",\"query\":\"\"}"	string
    */



    //public class OpenStreetMapTileSource : Microsoft.Maps.MapControl.TileSource
    //{
    //    public OpenStreetMapTileSource()
    //        : base("http://tile.openstreetmap.org/{2}/{0}/{1}.png")
    //    {
    //    }

    //    public override System.Uri GetUri(int x, int y, int zoomLevel)
    //    {
    //        return new Uri(string.Format(this.UriFormat, x, y, zoomLevel));
    //    }
    //}

    [ScriptableType()]

    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        
        private PointDataView _pointDataView;

        private PointDataViewCreate _pointDataViewCreate;

        private List<KMLFeed> kmlFeedList;

        private KMLFeed currentFeed;

        public ServiceController Service { get; set; }

        public string BaseURL { get; set; }
        public string PointSourceURL { get; set; }
        public string RegionSourceURL { get; set; }
        public string PointDataSummaryId { get; set; }

        public MainPage()
        {
            InitializeComponent();
            
            // Set up Main Map Events
            OpenDataMap.TargetViewChanged += new EventHandler<MapEventArgs>(OpenDataMap_TargetViewChanged);

            #region Advanced Search Component Wire-ups
            Messenger.Default.Register<object>(this, NotificationTokens.GetMapDetails, e =>
                {
                    ApplicationIdCredentialsProvider credProvider = App.Current.Resources["MyCredentials"] as ApplicationIdCredentialsProvider;

                    string culture = OpenDataMap.Culture;
                    Credentials creds = new Credentials() { ApplicationId = credProvider.ApplicationId };
                    LocationRect rect = OpenDataMap.BoundingRectangle;

                    MapDetails mapDetails = new MapDetails(creds, culture, rect);

                    Messenger.Default.Send<MapDetails>(mapDetails, NotificationTokens.SendMapDetails);
                });

            //ImageBrush brush = new ImageBrush();
            //brush.ImageSource = new BitmapImage(new Uri(tweet.profile_image_url));

            Messenger.Default.Register<MapItemsControl>(this, e =>
            {
                searchResultsMapLayer.Children.Clear();
                searchResultsMapLayer.Children.Add(e);
                // TODO: zoom to bounding rect of all added items.
                RaisePropertyChanged("OpenDataMap");
            });

            Messenger.Default.Register<Location>(this, (e) =>
            {
                searchResultsMapLayer.Children.Clear();
                Pushpin pin = new Pushpin();
                searchResultsMapLayer.AddChild(pin, e);
                OpenDataMap.SetView(e, 12);
                RaisePropertyChanged("OpenDataMap");
            });


            #endregion

            txtTitle.Text = ((App)Application.Current).appName;
            Uri baseUri = new Uri(((App)Application.Current).pageRootUrl);
            BaseURL = ((App)Application.Current).pageRootUrl;
            Service = new ServiceController(BaseURL);
            Service.SearchResultUpdate += new SearchResultUpdateEventHandler(service_SearchResultUpdate);

            Service.User.AuthUpdate += new AuthUpdateEventHandler(User_AuthUpdate);

            Service.Feeds.FeedUpdate += new FeedUpdateEventHandler(Feeds_FeedUpdate);

            Service.Summaries.SummaryDeleted += new SummaryDeletedEventHandler(Summaries_SummaryDeleted);

            // TODO: attach this to Vancouver Data
            // We always start out looking at Vancouver, from a distance, we will either zoom to edm,van,...

            // Easter egg! This is my house!
            Location loc = new Location(49.208882, -122.814293);

            OpenDataMap.AnimationLevel = AnimationLevel.Full;
            OpenDataMap.SetView(loc, 2.5);// TODO: attach this to Vancouver Data ( 2.5 zoom level )
            
            OpenDataMap.MousePan += new EventHandler<MapMouseDragEventArgs>(OpenDataMap_MousePan);

            _pointDataView = new PointDataView();
            _pointDataView.MainPage = this;
            userControlMapLayer.AddChild(_pointDataView,loc);
            _pointDataView.Summary = Service.Summaries;
            _pointDataView.User = Service.User;

            this.currentUserBadge.twitterImage.MouseLeftButtonUp +=new MouseButtonEventHandler(TwitterImage_MouseLeftButtonUp);

            // We need to receive the Twitter js callback message
            HtmlPage.RegisterScriptableObject("Page", this);

            // Removed by Cory Fowler to implement Advanced Search Component
            //txtSearchQuery.KeyUp += new KeyEventHandler(txtSearchQuery_KeyUp);

            
   
        }


        void Feeds_FeedUpdate(FeedController sender)
        {
            comboLocales.ItemsSource = Service.Feeds.CityFeeds;
            int selIndex = 0;

            foreach(CityFeed cf in Service.Feeds.CityFeeds)
            {
                // "4EDF9A51-4676-4fd2-BE27-8707875BA5BA" is the id of the Vancouver feed
                if (cf.Id == "4EDF9A51-4676-4fd2-BE27-8707875BA5BA" ) //"ce97c55a-9214-4a40-a2aa-b352e9f16c0b")//"4EDF9A51-4676-4fd2-BE27-8707875BA5BA")
                    break;

                selIndex++;
            }

            comboLocales.SelectedIndex = selIndex;

            InitSocialFeeds();

            
        }

        private void comboLocales_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CityFeed feed = comboLocales.SelectedItem as CityFeed;

            // This is a little hacky, TODO: look at using typed data + JSON serialization
            // Zoom is ignored
            Double lat = Double.Parse(feed.BoundaryPolygon.Split(',')[0].Split(':')[1]);
            Double lon = Double.Parse(feed.BoundaryPolygon.Split(',')[1].Split(':')[1]);

            Location loc = new Location(lat, lon); // Vancouver

            // Source URLs are based on the feedId
            PointSourceURL = BaseURL + "Feeds/" + feed.Id;
            RegionSourceURL = PointSourceURL + "/region";

            // Remove Existing Feeds
            pointsMapLayer.Children.Clear();
            regionsMapLayer.Children.Clear();

            PointDataSummaryId = ((App)Application.Current).pointDataSummaryId;
            LoadPointData();
            LoadRegionData();

            CubicEase ease = new CubicEase();
            ease.EasingMode = EasingMode.EaseIn;

            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromSeconds(3));
            da.From = OpenDataMap.ZoomLevel;
            da.To = 11.0;
            da.EasingFunction = ease;

            Storyboard.SetTarget(da, OpenDataMap);
            Storyboard.SetTargetProperty(da, new PropertyPath(Map.ZoomLevelProperty));
            Storyboard mySB = new Storyboard();
            mySB.Children.Add(da);
            mySB.Begin();

            OpenDataMap.SetView(loc, OpenDataMap.ZoomLevel);
        }

        void service_SearchResultUpdate(ServiceController sender)
        {
            searchResultsMapLayer.Children.Clear();
            IList<Location> locations = new List<Location>();

            for (int x = 0; x < sender.SearchResults.Count; x++)
            {
                 GeocodeService.GeocodeResult res = sender.SearchResults[x];

                 Pushpin pin = new Pushpin();

                 //ImageBrush brush = new ImageBrush();
                 //brush.ImageSource = new BitmapImage(new Uri(tweet.profile_image_url));
                 //pin.Background = brush;
                 pin.Location = res.Locations[0];
                 pin.Tag = res.DisplayName;

                 locations.Add(pin.Location);

                 ToolTipService.SetToolTip(pin, res.DisplayName);

                 searchResultsMapLayer.Children.Add(pin);
            }

            if (locations.Count > 0)
            {
                //If Search results were found, use the bounding area of the results
                OpenDataMap.SetView(new LocationRect(locations));
            }
        }

        // Removed by Cory Fowler to Implement Advanced Search Component
        //void txtSearchQuery_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Enter)
        //    {
        //        this.Service.GeoSearch(txtSearchQuery.Text, OpenDataMap);
        //    }
        //}

        //void Summaries_CommunitySummaryUpdate(SummariesController sender)
        //{
        //    //PointDataSummaryId
        //    int id = int.Parse(PointDataSummaryId);
        //    var list = from a in sender.PDSummaries where a.Value.Id == id select a;
        //    //if(sender.PDSummaries
        //}





        public void ShowTweetWin(PointDataSummary pds)
        {
            TweetEnterWin win = new TweetEnterWin();
            win.DataContext = pds;
            win.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(TweetWin_Closing);
            win.Show();
        }

        void TweetWin_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TweetEnterWin win = (TweetEnterWin)sender;
            if (win != null && win.DialogResult == true)
            {
                string tweet = win.twitterText.Text;
                PointDataSummary pds = win.DataContext as PointDataSummary;
                // TODO: Null?
                this.Service.User.UpdateTwitterStatus(tweet, pds.Latitude, pds.Longitude);
            }
        }

        // this callback is called from JavaScript after the user has logged in.
        [ScriptableMember()]
        public void OnJSTwitterCallback(string str)
        {
            this.Service.User.OnTwitterCallbackMessageReceived();
        }

        void User_AuthUpdate(UserController sender)
        {
            if (sender.IsAuthenticated)
            {
                // TODO: feed should be generic, not always KML
                KMLFeed feed = new KMLFeed();
                feed.Title = "YOUR LANDMARKS";
                feed.Icon = Service.User.currentUser.profile_image_url;
                feed.RefMapLayer = currentUserMapLayer;
                lbSocialFeeds.Items.Insert(0, feed); // we want it FIRST in the list

                socialStackPanel.Visibility = Visibility.Visible;

                btnAddLandmark.Visibility = Visibility.Visible;
                this.currentUserBadge.CurrentUser = sender.currentUser;
                InitUserLandmarkLayers();
            }
            else
            {
                
                btnAddLandmark.Visibility = Visibility.Collapsed;
                this.currentUserBadge.CurrentUser = null;
            }
        }

        void InitUserLandmarkLayers()
        {
            //this.service.Summaries.GetLayersForUserId(this.service.User.currentUser.Id);
            // Currently the UI is restricting each user to one layer, so we use their user_id as the layer id
            this.Service.Summaries.GetByLayerId(this.Service.User.currentUser.user_id.ToString());
            this.Service.Summaries.UserSummaryUpdate += new UserSummariesUpdateEventHandler(Summaries_UserSummaryUpdate);
        }

        void Summaries_UserSummaryUpdate(SummariesController sender)
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(Service.User.currentUser.profile_image_url));

            //SolidColorBrush borderBrush = new SolidColorBrush(Colors.Magenta);

            foreach(string key in sender.UserSummaries.Keys)
            {
                PointDataSummary pds = sender.UserSummaries[key];
                PlaceMark pm = new PlaceMark();
                pm.Location = new Location(pds.Latitude,pds.Longitude);
                pm.Summary = pds;
                pm.Id = pds.Guid;

                Pushpin oldElem = (Pushpin)communityMapLayer.FindName(pds.Guid);
                if (oldElem != null)
                {
                    communityMapLayer.Children.Remove(oldElem);
                    oldElem = null;
                }

                Pushpin pin = new Pushpin();
                pin.Background = brush;
                pin.Name = pds.Guid;
                pin.Tag = pm;

                ToolTipService.SetToolTip(pin, pds.Name);

                pin.MouseLeftButtonUp += new MouseButtonEventHandler(pin_MouseLeftButtonUp);
                currentUserMapLayer.AddChild(pin, pm.Location,PositionOrigin.BottomCenter);
            }
        }

        void Summaries_CommunityUpdate(SummariesController sender)
        {
          //  SolidColorBrush borderBrush = new SolidColorBrush(Colors.Magenta);

            foreach (string key in sender.CommunitySummaries.Keys)
            {
                
                PointDataSummary pds = sender.CommunitySummaries[key];
                ImageBrush brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(pds.CreatorProfileImageUrl));

                Pushpin elem = (Pushpin)this.communityMapLayer.FindName(pds.Guid);

                // only create a new item if it is not already on the map
                if(elem == null)
                {
                    PlaceMark pm = new PlaceMark();
                    pm.Location = new Location(pds.Latitude, pds.Longitude);
                    pm.Summary = pds;
                    pm.Id = pds.Guid;

                    Pushpin pin = new Pushpin();
                    //pin.Template = (ControlTemplate)Application.Current.Resources["PushPinTemplate"];
                    pin.Background = brush;
                   // pin.BorderBrush = borderBrush;
                 
                    pin.Tag = pm;
                    pin.Name = pm.Id; // use name/guid so we can find it later if need be

                    ToolTipService.SetToolTip(pin, pds.Name);

                    pin.MouseLeftButtonUp += new MouseButtonEventHandler(pin_MouseLeftButtonUp);
                    this.communityMapLayer.AddChild(pin, pm.Location,PositionOrigin.BottomCenter);
                }
            }
        }


        // Not Used currently, could be used to get more relevant data based on the new position/zoom of the map
        void OpenDataMap_MousePan(object sender, MapMouseDragEventArgs e)
        {
            // User has dragged the map
        }


        void InitSocialFeeds()
        {
            KMLFeed communityFeed = new KMLFeed();
            communityFeed.Title = "Shared Landmarks";
            communityFeed.Icon = "Images/icon_star.png";
            communityFeed.RefMapLayer = communityMapLayer;
            lbSocialFeeds.Items.Add(communityFeed);

            KMLFeed twitFeed = new KMLFeed();
            twitFeed.Title = "Nearby Tweets";
            twitFeed.Icon = BaseURL + "Images/TwitterSignin.png";
            twitFeed.RefMapLayer = twitterMapLayer;
            lbSocialFeeds.Items.Add(twitFeed);

            this.Service.User.GetUser();
        }

        void OpenDataMap_TargetViewChanged(object sender, MapEventArgs e)
        {
           // MiniMap.SetView(OpenDataMap.Center, Math.Max(1.0, OpenDataMap.ZoomLevel - 5));
        }

        public void LoadPointData()
        {
            // TODO; refactor this to service.Feeds.PointFeeds or something
            WebClient client = new WebClient();
            client.DownloadStringCompleted +=
                new DownloadStringCompletedEventHandler(onLoadPointData);
            client.DownloadStringAsync(new Uri(PointSourceURL));
        }

        void onLoadPointData(object sender, DownloadStringCompletedEventArgs e)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(new StringReader(e.Result),settings);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            kmlFeedList = new List<KMLFeed>();

            foreach (SyndicationItem item in feed.Items)
            {
                KMLFeed kFeed = new KMLFeed();
                kFeed.ID = item.Id;
                kFeed.Title = item.Title.Text;
                kFeed.Summary = item.Summary.Text;
                kFeed.IsSystem = true; //
                foreach (SyndicationLink link in item.Links)
                {
                    if (link.RelationshipType == "enclosure")
                    {
                        if (link.MediaType == "application/vnd.google-earth.kml+xml")
                        {
                            if (link.Uri.IsAbsoluteUri)
                            {
                                kFeed.Link = link.Uri.ToString();
                            }
                            else
                            {
                                kFeed.Link = BaseURL + link.Uri.ToString();
                            }
                        }
                        else if (link.MediaType == "image/png")
                        {
                            kFeed.Icon = link.Uri.ToString();
                        }
                    }
                }
                // we discard the item if it does NOT have a kml link
                if (kFeed.Link != null)
                {
                    kmlFeedList.Add(kFeed);
                }
            }

            lbPointFeeds.ItemsSource = kmlFeedList;

        }

        #region "Region Data"

        public void LoadRegionData()
        {
            // TODO; refactor this to service.Feeds.RegionFeeds or something
            WebClient client = new WebClient();
            client.DownloadStringCompleted +=
                new DownloadStringCompletedEventHandler(onLoadRegionData);
            client.DownloadStringAsync(new Uri(RegionSourceURL));
        }

        void onLoadRegionData(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.Error.WriteLine(e.Error.Message);
                return;
            }
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader reader = XmlReader.Create(new StringReader(e.Result), settings);
            SyndicationFeed feed = SyndicationFeed.Load(reader);

            kmlFeedList = new List<KMLFeed>();

            KMLFeed emptyFeed = new KMLFeed();
            emptyFeed.Title = "Hide all Map Attributes";
            emptyFeed.Link = null;
            kmlFeedList.Add(emptyFeed);

            foreach (SyndicationItem item in feed.Items)
            {
                KMLFeed kFeed = new KMLFeed();
                kFeed.ID = item.Id;
                kFeed.Title = item.Title.Text;
                kFeed.Summary = item.Summary.Text;
                kFeed.IsSystem = true;
                foreach (SyndicationLink link in item.Links)
                {
                    if (link.RelationshipType == "enclosure")
                    {
                        if (link.MediaType == "application/vnd.google-earth.kml+xml")
                        {
                            if (link.Uri.IsAbsoluteUri)
                            {
                                kFeed.Link = link.Uri.ToString();
                            }
                            else
                            {
                                kFeed.Link = BaseURL + link.Uri.ToString();
                            }
                        }
                        else if (link.MediaType == "image/png")
                        {
                            kFeed.Icon = BaseURL +  link.Uri.ToString();
                        }
                    }
                }
                // we discard the item if it does NOT have a kml link
                if (kFeed.Link != null)
                {
                    kmlFeedList.Add(kFeed);
                }
            }

            lbRegionFeeds.ItemsSource = kmlFeedList;
        }

        #endregion

        /*
        private Microsoft.Maps.MapControl.Design.LocationConverter locationConverter;
        private void ChangeMapView(object sender, RoutedEventArgs e)
        {
            if (locationConverter == null)
            {
                locationConverter = new Microsoft.Maps.MapControl.Design.LocationConverter();
                OpenDataMap.AnimationLevel = AnimationLevel.Full;
            }

            string serializedView = (string)((Button)sender).Tag;
            string[] splitView = serializedView.Split(' ');
            if (splitView.Length == 3)
            {
                Location center = (Location)locationConverter.ConvertFromString(splitView[0]);
                double zoomLevel = double.Parse(splitView[1]);
                OpenDataMap.SetView(center, zoomLevel);
            }
        }
         * */


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            KMLFeed feed = (KMLFeed)rb.DataContext;
            if (rb.IsChecked == true)
            {
                if (feed.RefMapLayer == null)
                {
                    CreateMapLayer(feed);
                }
                else
                {
                    feed.RefMapLayer.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (feed.RefMapLayer != null)
                {
                    feed.RefMapLayer.Visibility = Visibility.Collapsed;
                }
            }
        }


        private void cb1_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            KMLFeed feed = (KMLFeed)cb.DataContext;
            if (cb.IsChecked == true)
            {
                if (feed.RefMapLayer == null)
                {
                    CreateMapLayer(feed);
                }
                else
                {
                    feed.RefMapLayer.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (feed.RefMapLayer != null)
                {
                    feed.RefMapLayer.Visibility = Visibility.Collapsed;
                    if (this._pointDataView.CurrentSummary != null && this._pointDataView.CurrentSummary.LayerId == feed.ID)
                    {
                        _pointDataView.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }


        // TODO: Put this in service.Feeds or something
        private void CreateMapLayer(KMLFeed feed)
        {
            if (feed.Link == null)
            {
                // special case for the empty radio button to hide all
                return;
            }

            if (currentFeed == null)
            {
                currentFeed = feed; // one at a time
                WebClient client = new WebClient();

                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(request_MapDataDownloadCompleted);

                client.DownloadStringAsync(new Uri(feed.Link));
            }
            else
            {
                //throw new Exception("one at a time");
            }
        }


        void request_MapDataDownloadCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            KMLFeed feed = currentFeed;
            currentFeed = null; // let the next one go ...

            if (e.Error != null)
            {
                Console.WriteLine("Error : " + e.Error.Message);
                return;
            }

            Console.Error.WriteLine("Success : ");
            XDocument xDoc = XDocument.Parse(e.Result);
            XNamespace ns = xDoc.Root.Attributes("xmlns").First().Value;
            //http://earth.google.com/kml/2.1"; // seems some are 2.2

            if (xDoc.Descendants(ns + "Polygon").Count() > 0)
            {
                var results = xDoc.Descendants(ns + "Placemark");
                List<PlaceMarkRegion> elements = new List<PlaceMarkRegion>();

                foreach (XElement result in results)
                {
                    try{
                        PlaceMarkRegion reg = new PlaceMarkRegion();

                        XElement desc = result.Descendants(ns + "description").First();
                        reg.description = result.Descendants(ns + "description").First().Value;
                        if (reg.description.IndexOf("<") == 0)
                        {
                            // this looks like HTML
                            // Let's use ExtendedData SchemaData SimpleData @DESCNAME
                            IEnumerable<XElement> simpleDatas = result.Element(ns + "ExtendedData").Element(ns + "SchemaData").Elements(ns + "SimpleData");
                            reg.description = simpleDatas.Where(c => c.Attribute("name").Value == "DESCNAME").First().Value;
                        }

                        reg.coords = result.Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing").Element(ns + "coordinates").Value;
                        elements.Add(reg);
                    }
                    catch(Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception:" + ex.Message);
                    }
                }

                    feed.RefMapLayer = new MapLayer();
                    feed.IsRegionData = true;
                

                   foreach (PlaceMarkRegion reg in elements)
                    {
                        MapRegionControl polygon = new MapRegionControl();
                        polygon.Tag = reg.description;


                        polygon.Locations = new LocationCollection();
                        foreach (Location loc in reg.coordList)
                        {
                            polygon.Locations.Add(loc);
                        }
                        feed.RefMapLayer.Children.Add(polygon);
                    }
            }
            else
            {
                var elements = from results in xDoc.Descendants(ns + "Placemark")
                               select new PlaceMark
                               {
                                   Id = results.Element(ns + "description").Value,
                                   FeedId = feed.ID,
                                   IsSystem = feed.IsSystem,
                                   name = results.Element(ns + "name").Value,
                                  // string coords = results.Element(ns + "Point").Element(ns + "coordinates").Value;
                                  // Location = new Location(double.Parse(coords.Split(',')[0],double.Parse(coords.Split(',')[1])),
                                   Coords = results.Element(ns + "Point").Element(ns + "coordinates").Value

                               };

                ImageBrush brush = null;
                if (feed.Icon != null)
                {
                    brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(feed.Icon));

                }
                feed.RefMapLayer = new MapLayer();
                feed.IsRegionData = false;
                foreach (PlaceMark elem in elements)
                {
                    if (brush != null)
                    {
                        Pushpin pin = new Pushpin();
                        pin.Location = elem.Location;
                        pin.Background = brush;
                        pin.Tag = elem;

                        string hint = elem.name.Replace("<br/>", "\n");
                        ToolTipService.SetToolTip(pin, hint);

                        pin.MouseLeftButtonUp += new MouseButtonEventHandler(pin_MouseLeftButtonUp);
                        feed.RefMapLayer.AddChild(pin, elem.Location, PositionOrigin.BottomCenter);
                        //Rectangle rect = new Rectangle();
                        //rect.Fill = brush;

                        //rect.Width = 22;
                        //rect.Height = 22;
                        //rect.Opacity = 1.0;
                        
                        ////rect.SetValue(Canvas.TopProperty, -11.0);
                        ////rect.SetValue(Canvas.LeftProperty, -11.0);

                        

                        
                        //rect.MouseLeftButtonUp += new MouseButtonEventHandler(pin_MouseLeftButtonUp);
                        //rect.Tag = elem;
                        //feed.RefMapLayer.AddChild(rect, elem.Location,PositionOrigin.Center);
                    }
                    else
                    {
                        Pushpin pin = new Pushpin();
                        pin.Location = elem.Location;
                        pin.Height = pin.Width = 22;
                        pin.Tag = elem;

                        feed.RefMapLayer.AddChild(pin, elem.Location,PositionOrigin.BottomCenter);
                    }
                }
            }

            // we are seperating region layers from points, so everything stacks nicely
            if (feed.IsRegionData)
            {
                regionsMapLayer.Children.Add(feed.RefMapLayer);
            }
            else
            {
                pointsMapLayer.Children.Add(feed.RefMapLayer);
            }

        }
        
        void pin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement fromElem = (FrameworkElement)sender;
            PlaceMark elem = (PlaceMark)fromElem.Tag;
            Location fromLoc = elem.Location;

            // remove the _pointDataView control so we can move it
            userControlMapLayer.Children.Remove(_pointDataView);
            userControlMapLayer.AddChild(_pointDataView, fromLoc,PositionOrigin.BottomCenter);
            _pointDataView.Margin = new Thickness(0, 0, 0, 60);

            Point pt;
            OpenDataMap.SetView(fromLoc, OpenDataMap.ZoomLevel);
            // ensure the dataview is visible on the map
            if (OpenDataMap.TryLocationToViewportPoint(fromLoc, out pt))
            {
                double h = OpenDataMap.ViewportSize.Height;
                double w = OpenDataMap.ViewportSize.Width;
                if (pt.Y < (h / 3) ||
                     pt.Y > (h / 3 * 2) ||
                     pt.X < (w / 4) ||
                     pt.X > (w / 4 * 3)
                    )
                {
                    OpenDataMap.SetView(fromLoc, OpenDataMap.ZoomLevel);
                }
            }
            else
            {
                // item is off screen, should not be possible, but just in case
                OpenDataMap.SetView(fromLoc, OpenDataMap.ZoomLevel);
            }


            _pointDataView.CurrentPlaceMark = elem;
            _pointDataView.Visibility = Visibility.Visible;
            _pointDataView.infoViewBox.LayoutRoot.Visibility = Visibility.Visible;

        }


 

        #region Twitter

        private void UpdateTwitterLayer()
        {
            FetchTwitter(OpenDataMap.Center);
        }


        private void FetchTwitter(Location loc)
        {
            string twitSearchTemplate = @"http://search.twitter.com/search.json?geocode={0},{1},5km";
            string searchUrl = string.Format(twitSearchTemplate, loc.Latitude, loc.Longitude);

            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(HandleTwitterResult);

            client.OpenReadAsync(new Uri(searchUrl));


        }

        private void HandleTwitterResult(object sender, OpenReadCompletedEventArgs e)
        {
            // TODO: Serialize from Json
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(GeoTweetList));
            GeoTweetList tweets = (GeoTweetList)serializer.ReadObject(e.Result);
            int maxTweets = 100;
            for (int n = 0; n < tweets.results.Length; n++)
            {
                try
                {
                    GeoTweet tweet = tweets.results[n];
                    string[] parts = tweet.location.Split(':');
                    string part = parts.Length > 1 ? parts[1] : parts[0];
                    double lat = double.Parse(part.Split(',')[0]);
                    double lon = double.Parse(part.Split(',')[1]);
                    Location loc = new Location(lat, lon);
                    Pushpin pin = new Pushpin();

                    ImageBrush brush = new ImageBrush();
                    brush.ImageSource = new BitmapImage(new Uri(tweet.profile_image_url));

                    pin.Background = brush;
                    pin.Location = loc;
                    pin.Tag = tweet;

                    string hint = tweet.from_user + "\n" + tweet.text;
                    ToolTipService.SetToolTip(pin, hint);

                    twitterMapLayer.Children.Add(pin);
                    
                }
                catch(Exception)
                {
                }
            }

            int toRem = twitterMapLayer.Children.Count - maxTweets;
            if (toRem > 0)
            {
                for (int x = 0; x < toRem; x++)
                {
                    twitterMapLayer.Children.RemoveAt(0);
                }
            }

            if (twitterMapLayer.Visibility == Visibility.Visible)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(15.0);
                timer.Tick += new EventHandler(OnTwitter_Tick);
                timer.Start();
            }
        }

        void OnTwitter_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            if (twitterMapLayer.Visibility == Visibility.Visible)
            {
                UpdateTwitterLayer();
            }
            
        }


        #endregion




        private void lbPointFeeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #region "OAuth"

        // We could use this when the user attempts something that requires Auth,
        // I have chosen to prevent this through the UI with text that says "sign in to rate this ... "
        public void OnAuthRequired()
        {
            SignInPrompt prompt = new SignInPrompt();
            prompt.Show();
            prompt.Closed += new EventHandler(AuthPrompt_Closed);
        }

        void AuthPrompt_Closed(object sender, EventArgs e)
        {
            Service.User.RequestAuthToken();
        }

        private void TwitterImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Service.User.RequestAuthToken();
        }

        #endregion


        #region "Delete Landmark"



        public void DeleteLandmark(PlaceMark pm)
        {
            ConfirmDelete(pm.Id);
            
        }

        public void ConfirmDelete(string id)
        {
            ConfirmDeleteWin win = new ConfirmDeleteWin();
            win.DataContext = id;
            win.Closing += new EventHandler<CancelEventArgs>(ConfirmDeleteWinClosing);
            win.Show();
        }

        void ConfirmDeleteWinClosing(object sender, CancelEventArgs e)
        {
            ConfirmDeleteWin win = (ConfirmDeleteWin)sender;
            if (win != null && win.DialogResult == true)
            {
                string id = win.DataContext as string;
                this.Service.Summaries.RemovePointDataSummary(id);
            }
        }


        void Summaries_SummaryDeleted(object sender, SummaryDeletedArgs e)
        {

            Pushpin elem = (Pushpin)currentUserMapLayer.FindName(e.guid);

            if (elem != null)
            {
                currentUserMapLayer.Children.Remove(elem);
                elem = null;
            }
            _pointDataView.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region "Edit Landmark"

        public void EditLandmark(PlaceMark pm)
        {  
            if (_pointDataViewCreate == null)
            {
                _pointDataViewCreate = new PointDataViewCreate();
                _pointDataViewCreate.Summary = Service.Summaries;
                _pointDataViewCreate.User = Service.User;
                _pointDataViewCreate.MainPage = this; // HACK for callback
            }
            else 
            {
                if (_pointDataViewCreate.Pin != null)
                {
                    userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);

                    _pointDataViewCreate.Pin = null;
                }

                userGenMapLayer.Children.Remove(_pointDataViewCreate);
            }

            _pointDataView.Visibility = Visibility.Collapsed;

            _pointDataViewCreate.createViewBox.ClearForm();
            _pointDataViewCreate.createViewBox.Data = pm.Summary;

            Pushpin customPin = new Pushpin();
            customPin.Location = new Location(_pointDataView.CurrentSummary.Latitude, _pointDataView.CurrentSummary.Longitude);

            _pointDataViewCreate.Pin = customPin;

            userGenMapLayer.AddChild(_pointDataViewCreate, customPin.Location, PositionOrigin.BottomCenter);
            customPin.Background = new SolidColorBrush(Color.FromArgb(255, 0x40, 0x22, 0x5F));////40225F

            userGenMapLayer.Visibility = Visibility.Visible;
            userGenMapLayer.Children.Add(customPin);

            _pointDataViewCreate.Visibility = Visibility.Visible;

            customPin.MouseLeftButtonDown += new MouseButtonEventHandler(customPin_MouseLeftButtonDown);
            customPin.MouseLeftButtonUp += new MouseButtonEventHandler(customPin_MouseLeftButtonUp);
            
        }

        #endregion

        #region "Add Landmarks"

        private void AddLandmark_Click(object sender, RoutedEventArgs e)
        {
            if (_pointDataViewCreate == null)
            {
                _pointDataViewCreate = new PointDataViewCreate();
                _pointDataViewCreate.Summary = Service.Summaries;
                _pointDataViewCreate.User = Service.User;
                _pointDataViewCreate.MainPage = this; // HACK for callback
                
            }
            else if (_pointDataViewCreate.Pin != null)
            {
                userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);
                _pointDataViewCreate.Pin = null;
                _pointDataViewCreate.createViewBox.ClearForm();
                userGenMapLayer.Children.Remove(_pointDataViewCreate);
            }

            
            // Hide the current landmark data view
            _pointDataView.Visibility = Visibility.Collapsed;
            Pushpin customPin = new Pushpin();
            customPin.Location = OpenDataMap.Center;


            _pointDataViewCreate.Pin = customPin;
            userGenMapLayer.AddChild(_pointDataViewCreate, OpenDataMap.Center,PositionOrigin.BottomCenter);
            customPin.Background = new SolidColorBrush(Color.FromArgb(255, 0x40, 0x22, 0x5F));////40225F

            userGenMapLayer.Visibility = Visibility.Visible;
            userGenMapLayer.Children.Add(customPin);

            _pointDataViewCreate.Visibility = Visibility.Visible;

            customPin.MouseLeftButtonDown += new MouseButtonEventHandler(customPin_MouseLeftButtonDown);
            customPin.MouseLeftButtonUp += new MouseButtonEventHandler(customPin_MouseLeftButtonUp);

        }

        public void PointCreateCancelled()
        {
            userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);
            _pointDataViewCreate.Visibility = Visibility.Collapsed;
        }

        // Callback after point is created by _pointDataViewCreate
        public void PointCreated(PointDataSummary pds)
        {
            // Delegate method is called more than once, so we will check if it is already on the map, and remove it
            // before adding it.


            Pushpin elem = (Pushpin)currentUserMapLayer.FindName(pds.Guid);

            if (elem != null)
            {
                currentUserMapLayer.Children.Remove(elem);
                elem = null;
            }

            elem = new Pushpin();


            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(Service.User.currentUser.profile_image_url));


            bool wasRemoved = userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);
            _pointDataViewCreate.Visibility = Visibility.Collapsed;

            PlaceMark pm = new PlaceMark();
            pm.Location = new Location(pds.Latitude, pds.Longitude);
            pm.Summary = pds;
            pm.Id = pds.Guid;

            elem.Background = brush;
            elem.Tag = pm;
            elem.Name = pds.Guid;
            ToolTipService.SetToolTip(elem, pds.Name);


            elem.MouseLeftButtonUp += new MouseButtonEventHandler(pin_MouseLeftButtonUp);

            currentUserMapLayer.AddChild(elem, pm.Location, PositionOrigin.BottomCenter);

            pin_MouseLeftButtonUp(elem, null);
        }

        void customPin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Pushpin customPin = (Pushpin)sender;
            customPin.ReleaseMouseCapture();
            customPin.MouseMove -= customPin_MouseMove;

            userGenMapLayer.Children.Remove(_pointDataViewCreate);
            userGenMapLayer.AddChild(_pointDataViewCreate, customPin.Location,PositionOrigin.BottomCenter);
            _pointDataViewCreate.Visibility = Visibility.Visible;
        }

        private Point mouseOffset;

        void customPin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Pushpin customPin = (Pushpin)sender;
            customPin.CaptureMouse();
            mouseOffset = e.GetPosition(customPin);
            customPin.MouseMove += new MouseEventHandler(customPin_MouseMove);
            _pointDataViewCreate.Visibility = Visibility.Collapsed;
        }

        void customPin_MouseMove(object sender, MouseEventArgs e)
        {
            Pushpin customPin = (Pushpin)sender;
           
            Point point = e.GetPosition(userGenMapLayer);
            point.Y += mouseOffset.Y;
            point.X += mouseOffset.X;
            
            customPin.Location = OpenDataMap.ViewportPointToLocation(point);
           // MapLayer ml = (MapLayer)_pointDataViewCreate;
        }

        #endregion

        private void GetCommunityLayer()
        {
            Service.Summaries.GetSummariesForCommunity();
            Service.Summaries.CommunitySummaryUpdate += new CommunitySummariesUpdateEventHandler(Summaries_CommunityUpdate);
        }

        private void cbSocialFeedClick(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            KMLFeed feed = (KMLFeed)cb.DataContext;
            if (feed != null)
            {

                if (feed.RefMapLayer != null)
                {
                    feed.RefMapLayer.Visibility = cb.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
                    if (feed.RefMapLayer == twitterMapLayer && cb.IsChecked == true)
                    {
                        UpdateTwitterLayer();
                    }
                    else if (feed.RefMapLayer == this.communityMapLayer && cb.IsChecked == true)
                    {
                        GetCommunityLayer();
                    }
                    else if (feed.RefMapLayer == this.currentUserMapLayer && cb.IsChecked == true)
                    {

                    }
                }
                else
                {
                    // what
                }
            }
            
        }


        // HACK!
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           // this.ShowTweetWin(_pointDataView.CurrentSummary);
           // Note:: no text entry in fullscreen
           // Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
            
        }


        // Auto Select the 'SharedLandmarks' list item
        private void CheckBox_Loaded(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            KMLFeed feed = (KMLFeed)cb.DataContext;
            if (feed.RefMapLayer == communityMapLayer || feed.RefMapLayer == currentUserMapLayer)
            {
                cb.IsChecked = true;
                cbSocialFeedClick(sender, e);
            }
        }


        // Special case handler, we want the first radio button to be selected
        private void RadioButton_Loaded(object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            KMLFeed feed = (KMLFeed)rb.DataContext;
            if (feed.Link == null)
            {
                rb.IsChecked = true;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        } 
    }
}
