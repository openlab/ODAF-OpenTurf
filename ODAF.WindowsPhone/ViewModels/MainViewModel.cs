using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls.Maps;
using ODAF.WindowsPhone.Commands;
using ODAF.WindowsPhone.Models;
using ODAF.WindowsPhone.Services;
using System.IO.IsolatedStorage;


namespace ODAF.WindowsPhone.ViewModels
{
    /// <summary>
    /// This class represents the logical model of the MainPage.xaml view.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {      

        #region Properties

        private GeoCoordinate _CenterLocation = App.CenterLocation;
        /// <summary>
        /// Represents the initial center of the map
        /// </summary>
        public GeoCoordinate CenterLocation
        {
            get { return _CenterLocation; }
            set
            {
                if (value != _CenterLocation)
                {
                    _CenterLocation = value;
                    NotifyPropertyChanged("CenterLocation");
                }
            }
        }

        private ObservableCollection<CommentModel> _Comments = new ObservableCollection<CommentModel>();
        /// <summary>
        /// The list of comments associated to the SelectedPushpin.
        /// </summary>
        public ObservableCollection<CommentModel> Comments
        {
            get
            {
                return _Comments;
            }
            set
            {
                if (value != _Comments)
                {
                    _Comments = value;
                    NotifyPropertyChanged("Comments");
                }
            }
        }

        private readonly CredentialsProvider _credentialsProvider = new ApplicationIdCredentialsProvider(App.BingMapsId);
        public CredentialsProvider CredentialsProvider
        {
            get { return _credentialsProvider; }
        }

        private ObservableCollection<LayerModel> _Layers = new ObservableCollection<LayerModel>();
        /// <summary>
        /// Represents all the layers that have been identified in the ODAF website PointSources.xaml file.
        /// </summary>
        public ObservableCollection<LayerModel> Layers
        {
            get { return _Layers; }
        }

        private static ObservableCollection<PushpinModel> _Pushpins = new ObservableCollection<PushpinModel>();
        /// <summary>
        /// Represents all the pushpins that are databound to the map.
        /// </summary>
        public static ObservableCollection<PushpinModel> Pushpins
        {
            get { return _Pushpins; }
        }


        private PushpinModel _SelectedPushpin;
        public PushpinModel SelectedPushpin
        {
            get { return _SelectedPushpin; }
            set
            {
                if (value != _SelectedPushpin)
                {
                    _SelectedPushpin = value;
                    if (_SelectedPushpin != null)
                    {
                        //Récupération des commentaires associées au point de données
                        Comments.Clear();
                        CommentsService commentService = new CommentsService();
                        commentService.GetCommentsAsync(_SelectedPushpin.Guid, Comments);
                        IsolatedStorageSettings.ApplicationSettings["SelectedPushpin"] = _SelectedPushpin; 
                        //Résolution de l'adresse civique via l'API Bing Maps
                        CenterLocation = new GeoCoordinate(_SelectedPushpin.Location.Latitude, _SelectedPushpin.Location.Longitude);
                        CivicAddressResolver addressResolver = new CivicAddressResolver();
                        addressResolver.ResolveAddressCompleted += (s, e) => 
                        { 
                            if(!e.Address.IsUnknown) 
                                _SelectedPushpin.CivicAddress = e.Address;
                        };
                        addressResolver.ResolveAddressAsync(_SelectedPushpin.Location);
                    }
                    NotifyPropertyChanged("SelectedPushpin");
                }
            }
        }
        

        private int _ZoomLevel = 12;
        /// <summary>
        /// Represents the initial zoom level of the map.
        /// </summary>
        public int ZoomLevel
        {
            get { return _ZoomLevel; }
            set
            {
                if (value != _ZoomLevel)
                {
                    _ZoomLevel = value;
                    NotifyPropertyChanged("ZoomLevel");
                }
            }
        }
        
        #endregion
   
        public MainViewModel()
        {
            if (DesignerProperties.IsInDesignTool)
            {
                DesignTimeLayerService.LoadLayers(Layers);
                DesignTimePushpinService.LoadPushpinsAsync(Layers[0], Pushpins);

                CommentModel comment1 = new CommentModel
                {
                    Comment = new Comment 
                    {
                        Id = 1,
                        CreatedOn = new DateTime(2011, 07, 28, 12, 35, 0),
                        Text = "Borne très pratique! http://twitpic.com/5ohom5"
                    },
                    CommentAuthor = "Sébastien",
                    ServiceName = "Twitter"                 
                };

                CommentModel comment2 = new CommentModel
                {
                    Comment = new Comment
                    {
                        Id = 2,
                        CreatedOn = new DateTime(2011, 07, 28, 13, 28, 0),
                        Text = "Prise deffectueuse! http://twitpic.com/5ohom5"
                    },
                    CommentAuthor = "Sébastien",
                    ServiceName = "Twitter"
                };


                CommentModel comment3 = new CommentModel
                {
                    Comment = new Comment
                    {
                        Id = 3,
                        CreatedOn = new DateTime(2012, 07, 06, 10, 12, 0),
                        Text = "Prise réparée ! http://twitpic.com/5ohom5"
                    },
                    CommentAuthor = "Rémi",
                    ServiceName = "Twitter"
                };

                Comments.Add(comment1);
                Comments.Add(comment2);
                Comments.Add(comment3);
            }
            else
            {
                Uri OdafBaseUri = new Uri(App.OdafWebsiteUrl);
                LayerService.LoadLayersAsync(new Uri(OdafBaseUri, "PointSources.xml"), Layers);
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
        
    }
}