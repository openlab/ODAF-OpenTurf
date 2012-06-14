using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Maps.MapControl;
using Microsoft.Maps.MapControl.ExtendedModes;
using Net.SyntaxC4.RedBitSoftware.ODAF.Search.Common;
using ODAF.SilverlightApp.CloudService;
using ODAF.SilverlightApp.Resources;
using ODAF.SilverlightApp.UserControls;
using ODAF.SilverlightApp.VO;


namespace ODAF.SilverlightApp
{
	[ScriptableType()]
	public partial class MainPage : UserControl, INotifyPropertyChanged
	{
		private PointDataView _pointDataView;
		private PointDataViewCreate _pointDataViewCreate;
		private KMLFeed currentFeed;
		public ServiceController Service { get; set; }
		
		public string BaseURL { get; set; }
		public string PointSourceURL { get; set; }
		public string RegionSourceURL { get; set; }
		public string PointDataSummaryId { get; set; }

		public MainPage()
		{
			InitializeComponent();

			BirdseyeMode.AddModeToNavigationBar(OpenDataMap);

			txtTitle.Text = ((App)Application.Current).appName;
			Uri baseUri = new Uri(((App)Application.Current).pageRootUrl);
			BaseURL = ((App)Application.Current).pageRootUrl;
			Service = new ServiceController(BaseURL);

			Service.SearchResultUpdate += new SearchResultUpdateEventHandler(service_SearchResultUpdate);
			Service.User.AuthUpdate += new AuthUpdateEventHandler(User_AuthUpdate);
			Service.Feeds.FeedUpdate += new FeedUpdateEventHandler(Feeds_FeedUpdate);
			Service.Summaries.SummaryDeleted += new SummaryDeletedEventHandler(Summaries_SummaryDeleted);

			// Easter egg! This is my house!
			Location loc = new Location(49.208882, -122.814293);

			OpenDataMap.AnimationLevel = AnimationLevel.Full;
			OpenDataMap.SetView(loc, 4);// TODO: attach this to Vancouver Data ( 4 zoom level )
			OpenDataMap.MousePan += new EventHandler<MapMouseDragEventArgs>(OpenDataMap_MousePan);

			_pointDataView = new PointDataView();
			_pointDataView.MainPage = this;
			userControlMapLayer.AddChild(_pointDataView, loc);
			_pointDataView.Summary = Service.Summaries;
			_pointDataView.User = Service.User;

			this.currentUserBadge.twitterImage.MouseLeftButtonUp += TwitterImage_MouseLeftButtonUp;
			this.currentUserBadge.twitterText.MouseLeftButtonUp += TwitterImage_MouseLeftButtonUp;

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

			// We need to receive the Twitter js callback message
			HtmlPage.RegisterScriptableObject("Page", this);
		}

		void Feeds_FeedUpdate(FeedController sender)
		{
			comboLocales.ItemsSource = Service.Feeds.CityFeeds;
			int selIndex = Service.Feeds.CityFeeds.Length > 0 ? Service.Feeds.CityFeeds.Length - 1 : -1;

			comboLocales.SelectedIndex = selIndex;

			InitSocialFeeds();
		}

		private void comboLocales_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			CityFeed feed = comboLocales.SelectedItem as CityFeed;

			// This is a little hacky, TODO: look at using typed data + JSON serialization
			// Zoom is ignored
			Double lat = Double.Parse(feed.BoundaryPolygon.Split(',')[0].Split(':')[1], System.Globalization.CultureInfo.InvariantCulture);
			Double lon = Double.Parse(feed.BoundaryPolygon.Split(',')[1].Split(':')[1], System.Globalization.CultureInfo.InvariantCulture);

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
			win.Closing += TweetWin_Closing;
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
				Service.User.UpdateTwitterStatus(tweet, pds.Latitude, pds.Longitude);
			}
		}

		// this callback is called from JavaScript after the user has logged in.
		[ScriptableMember()]
		public void OnJSTwitterCallback(string oauth_token)
		{
			Service.User.OnTwitterCallbackMessageReceived(oauth_token);
		}

		void User_AuthUpdate(UserController sender)
		{
			if (sender.IsAuthenticated)
			{
				// TODO: feed should be generic, not always KML
				KMLFeed feed = new KMLFeed();
				feed.Title = MainPageResource.YourLandmarks;
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
			// Currently the UI is restricting each user to one layer, so we use their user_id as the layer id
			Service.Summaries.GetByLayerId(Service.User.currentUser.user_id.ToString());
			Service.Summaries.UserSummaryUpdate += new UserSummariesUpdateEventHandler(Summaries_UserSummaryUpdate);
		}

		void Summaries_UserSummaryUpdate(SummariesController sender)
		{
			ImageBrush brush = new ImageBrush();
			brush.ImageSource = new BitmapImage(new Uri(Service.User.currentUser.profile_image_url));

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

				pin.MouseLeftButtonUp += pin_MouseLeftButtonUp;
				currentUserMapLayer.AddChild(pin, pm.Location,PositionOrigin.BottomCenter);
			}
		}

		void Summaries_CommunityUpdate(SummariesController sender)
		{
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

					pin.MouseLeftButtonUp += pin_MouseLeftButtonUp;
					this.communityMapLayer.AddChild(pin, pm.Location,PositionOrigin.BottomCenter);
				}
			}
		}

		// Not Used currently, could be used to get more relevant data based on the new position/zoom of the map
		void OpenDataMap_MousePan(object sender, MapMouseDragEventArgs e)
		{
			// User has dragged the map
		}

		void DelayedLoadTimer(object sender, EventArgs e)
		{

			DispatcherTimer timer = (DispatcherTimer)sender;
			timer.Stop();
			// Default Vancouver but it points to the correct location when the page is loaded
			Location loc = new Location(48.8566140, 2.3522219);

			CubicEase ease = new CubicEase();
			ease.EasingMode = EasingMode.EaseIn;
			

			DoubleAnimation da = new DoubleAnimation();
			da.Duration = new Duration(TimeSpan.FromSeconds(3));
			da.From = OpenDataMap.ZoomLevel;
			da.To = 13.0;
			da.EasingFunction = ease;

			Storyboard.SetTarget(da, OpenDataMap);
			Storyboard.SetTargetProperty(da, new PropertyPath(Map.ZoomLevelProperty));
			Storyboard mySB = new Storyboard();
			mySB.Children.Add(da);
			mySB.Begin();

			OpenDataMap.SetView(loc,OpenDataMap.ZoomLevel);

			InitSocialFeeds();
		}

		void InitSocialFeeds()
		{
			KMLFeed communityFeed = new KMLFeed();
			communityFeed.Title = MainPageResource.SharedLandmarks;
			communityFeed.Icon = "Images/icon_star.png";
			communityFeed.RefMapLayer = communityMapLayer;
			lbSocialFeeds.Items.Add(communityFeed);

			KMLFeed twitFeed = new KMLFeed();
			twitFeed.Title = MainPageResource.NearbyTweets;
			twitFeed.Icon = BaseURL + "Images/TwitterSignin.png";
			twitFeed.RefMapLayer = twitterMapLayer;
			lbSocialFeeds.Items.Add(twitFeed);

			Service.User.GetUser();
		}

		public void LoadPointData()
		{
			// TODO; refactor this to service.Feeds.PointFeeds or something
			WebClient client = new WebClient();
			client.DownloadStringCompleted += onLoadPointData;
			client.DownloadStringAsync(new Uri(PointSourceURL));
		}

		void onLoadPointData(object sender, DownloadStringCompletedEventArgs e)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.DtdProcessing = DtdProcessing.Ignore;
			XmlReader reader = XmlReader.Create(new StringReader(e.Result),settings);
			SyndicationFeed feed = SyndicationFeed.Load(reader);

			List<KMLFeed> kmlFeedList = new List<KMLFeed>();

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
			client.DownloadStringCompleted += onLoadRegionData;
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

			List<KMLFeed> kmlFeedList = new List<KMLFeed>();

			KMLFeed emptyFeed = new KMLFeed();
			emptyFeed.Title = MainPageResource.HideallMapAttributes;
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

			lbRegionFeeds.ItemsSource = kmlFeedList;
		}

		#endregion

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
					foreach (DictionaryEntry de in this.Resources)
					{
                        try
                        {
                            Storyboard s = de.Value as Storyboard;
                            if (s != null)
                                s.Begin();
                        }
                        catch { }
					}
				}
			}
			else
			{
				if (feed.RefMapLayer != null)
				{
					feed.RefMapLayer.Visibility = Visibility.Collapsed;
					//if (this._pointDataView.CurrentSummary != null && this._pointDataView.CurrentSummary.LayerId == feed.ID)
					//{
						_pointDataView.Visibility = Visibility.Collapsed;
					//}
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
				client.DownloadStringCompleted += request_MapDataDownloadCompleted;
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
			XNamespace ns = xDoc.Root.Attributes("xmlns").First().Value;//http://earth.google.com/kml/2.1"; // seems some are 2.2

			if (xDoc.Descendants(ns + "Polygon").Count() > 0)
			{
				var elements = from results in xDoc.Descendants(ns + "Placemark")
							   select new PlaceMarkRegion
							   {
								   description = results.Element(ns + "description").Value,
								   // Polygon, outerBoundaryIs, LinearRing, coordinates
								   coords = results.Element(ns + "Polygon").Element(ns + "outerBoundaryIs").Element(ns + "LinearRing").Element(ns + "coordinates").Value
							   };
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
								   Name = results.Element(ns + "name").Value,
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
						pin.Background = brush;
						pin.MouseLeftButtonUp += pin_MouseLeftButtonUp;
						pin.Tag = elem;
						pin.Name = Guid.NewGuid().ToString();
						pin.RenderTransform = new CompositeTransform();
						(pin.RenderTransform as CompositeTransform).TranslateY = -300;
						pin.Loaded += (loadedSender, loadedEventArgs) =>
						{
							Pushpin currentPin = loadedSender as Pushpin;
							Guid random = new Guid(currentPin.Name);
							byte[] bytes = random.ToByteArray();
							int seed = BitConverter.ToInt32(bytes, 0);

							Random fromValueRandom = new Random(seed);
							Random durationRandom = new Random(seed);

							var storyboard = new Storyboard();
							var translateAnimation = new DoubleAnimation()
							{
								From = -fromValueRandom.Next(100, 400),
								To = 0,
								Duration = TimeSpan.FromMilliseconds(durationRandom.Next(1000, 5000)),
								EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut }
							};
							translateAnimation.SetValue(Storyboard.TargetNameProperty, currentPin.Name);
							translateAnimation.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(UIElement.RenderTransform).(CompositeTransform.TranslateY)"));
							storyboard.Children.Add(translateAnimation);
							Resources.Add(currentPin.Name, storyboard);
							storyboard.Begin();
						};

						feed.RefMapLayer.AddChild(pin, elem.Location, PositionOrigin.BottomCenter);
						string hint = elem.Id.Replace("<br/>", "\n");
						ToolTipService.SetToolTip(pin, hint);               
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
			userControlMapLayer.AddChild(_pointDataView, fromLoc, PositionOrigin.BottomCenter);
			_pointDataView.Margin = new Thickness(0, 0, 0, 60);

			Point pt;
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
			client.OpenReadCompleted += HandleTwitterResult;
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
				{ }
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

		#region "OAuth"

		// We could use this when the user attempts something that requires Auth,
		// I have chosen to prevent this through the UI with text that says "sign in to rate this ... "
		public void OnAuthRequired()
		{
			SignInPrompt prompt = new SignInPrompt();
			prompt.Show();
			prompt.Closed += AuthPrompt_Closed;
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
				Service.Summaries.RemovePointDataSummary(id);
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

			customPin.MouseLeftButtonDown += customPin_MouseLeftButtonDown;
			customPin.MouseLeftButtonUp += customPin_MouseLeftButtonUp;
		}

		public void PointCreateCancelled()
		{
			userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);
			_pointDataViewCreate.Visibility = Visibility.Collapsed;
		}

		// HACK: Callback after point is created by _pointDataViewCreate
		public void PointCreated(PointDataSummary pds)
		{
			// Delegate method is called more than once, so we will check if it is already on the map, and remove it
			// before adding it.
			UIElement elem = (UIElement)this.currentUserMapLayer.FindName(pds.Guid);
			if (elem != null)
			{
				currentUserMapLayer.Children.Remove(elem);
			}

			ImageBrush brush = new ImageBrush();
			SolidColorBrush stroke = new SolidColorBrush(Colors.White);
			brush.ImageSource = new BitmapImage(new Uri(Service.User.currentUser.profile_image_url));

			userGenMapLayer.Children.Remove(_pointDataViewCreate.Pin);
			_pointDataViewCreate.Visibility = Visibility.Collapsed;

			PlaceMark pm = new PlaceMark();
			pm.Location = new Location(pds.Latitude, pds.Longitude);
			pm.Summary = pds;
			pm.Id = pds.Guid;

			Ellipse pin = new Ellipse();
			pin.Fill = brush;
			pin.StrokeThickness = 1;
			pin.Stroke = stroke;
			pin.Width = 22;
			pin.Height = 22;
			pin.Opacity = 0.9;
			pin.Tag = pm;
			pin.Name = pm.Id;

			ToolTipService.SetToolTip(pin, pds.Name);

			pin.MouseLeftButtonUp += pin_MouseLeftButtonUp;
			currentUserMapLayer.AddChild(pin, pm.Location,PositionOrigin.Center);
			pin_MouseLeftButtonUp(pin, null);
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