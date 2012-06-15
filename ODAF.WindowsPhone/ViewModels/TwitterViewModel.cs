using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using ODAF.WindowsPhone.Commands;
using ODAF.WindowsPhone.Services;
using TweetSharp;
using System.IO;
using System.Globalization;
using System.Threading;
using ODAF.WindowsPhone.Models;

namespace ODAF.WindowsPhone.ViewModels
{
    /// <summary>
    /// This class represents the logical model of the TwitterPage.xaml view.
    /// </summary>
    public class TwitterViewModel : INotifyPropertyChanged
    {
        private CommentsService CommentService = new CommentsService();
        //To ensure that a summary exists before creating comments in ODAF database.
        private bool SummaryHasBeenCreated;
        private PushpinModel SelectedPushpin;
        private TwitterService Twitter = new TwitterService(App.TwitterConsumerKey, App.TwitterConsumerSecret);
        private OAuthRequestToken RequestToken;
        private OAuthAccessToken AccessToken;

        #region Commands

        private ICommand _AuthenticateUserCommand;
        public ICommand AuthenticateUserCommand
        {
            get { return _AuthenticateUserCommand; }
        }

        private ICommand _SendTweetCommand;
        public ICommand SendTweetCommand
        {
            get { return _SendTweetCommand; }
        }

        private ICommand _ResetTwitterCredentialsCommand;
        public ICommand ResetTwitterCredentialsCommand
        {
            get { return _ResetTwitterCredentialsCommand; }
        }


        private ICommand _TakePhotoCommand;
        public  ICommand TakePhotoCommand
        {
            get { return _TakePhotoCommand; }
        }      

        #endregion

        #region Properties

        private BitmapImage _TwitpicImage;
        /// <summary>
        /// Represents the image that will be uploaded to Twitpic. This property is set after the image has been uploaded.
        /// </summary>
        public BitmapImage TwitpicImage
        {
            get { return _TwitpicImage; }
            set
            {
                if (value != _TwitpicImage)
                {
                    _TwitpicImage = value;
                    NotifyPropertyChanged("TwitpicImage");
                }
            }
        }
        
        private bool _HasUserAlreadyAuthorizedApp;
        /// <summary>
        /// Indicates if the user has already authorized the application to access his Twitter account.
        /// If so, we don't need to re-authenticate at each launching.
        /// </summary>
        public bool HasUserAlreadyAuthorizedApp
        {
            get { return _HasUserAlreadyAuthorizedApp; }
            set
            {
                if (value != _HasUserAlreadyAuthorizedApp)
                {
                    _HasUserAlreadyAuthorizedApp = value;
                    NotifyPropertyChanged("HasUserAlreadyAuthorizedApp");
                }
            }
        }


        private bool _IsUploading;
        /// <summary>
        /// Used to notify the state of the uploading process.
        /// </summary>
        public bool IsUploading
        {
            get { return _IsUploading; }
            set
            {
                if (value != _IsUploading)
                {
                    _IsUploading = value;
                    NotifyPropertyChanged("IsUploading");
                }
            }
        }
        

        private string _Tweet;
        /// <summary>
        /// Represents a message to be sent to Twitter
        /// </summary>
        public string Tweet
        {
            get { return _Tweet; }
            set
            {
                if (value != _Tweet)
                {
                    _Tweet = value;
                    NotifyPropertyChanged("Tweet");
                }
            }
        }   

        private string _TwitpicPhotoUri;
        /// <summary>
        /// Represents a Uri to a photo posted on Twitpic.
        /// </summary>
        public string TwitpicPhotoUri
        {
            get { return _TwitpicPhotoUri; }
            set
            {
                if (value != _TwitpicPhotoUri)
                {
                    _TwitpicPhotoUri = value;
                    NotifyPropertyChanged("TwitpicPhotoUri");
                }
            }
        }
        
        private Uri _TwitterAuthorizationUri;
        /// <summary>
        /// Represents the Twitter authorization URI which will be needed ine the authorization process to allow the application to post on user's wall.
        /// </summary>
        public Uri TwitterAuthorizationUri
        {
            get { return _TwitterAuthorizationUri; }
            set
            {
                if (value != _TwitterAuthorizationUri)
                {
                    _TwitterAuthorizationUri = value;
                    NotifyPropertyChanged("TwitterAuthorizationUri");
                }
            }
        }
           
        private string _TwitterCodePin;
        /// <summary>
        /// Represents the Pin code which will be displayed after the user has entered his credentials in the WebBrowser control.
        /// </summary>
        public string TwitterCodePin
        {
            get { return _TwitterCodePin; }
            set
            {
                if (value != _TwitterCodePin)
                {
                    _TwitterCodePin = value;
                    NotifyPropertyChanged("TwitterCodePin");
                }
            }
        }

        private string _TwitterPassword;
        /// <summary>
        /// Represents the Twitter account password of the user.
        /// </summary>
        public string TwitterPassword
        {
            get { return _TwitterPassword; }
            set
            {
                if (value != _TwitterPassword)
                {
                    _TwitterPassword = value;
                    NotifyPropertyChanged("TwitterPassword");
                }
            }
        }

        private string _TwitterUsername;
        /// <summary>
        /// Represents the Twitter account name of the user.
        /// </summary>
        public string TwitterUsername
        {
            get { return _TwitterUsername; }
            set
            {
                if (value != _TwitterUsername)
                {
                    _TwitterUsername = value;
                    NotifyPropertyChanged("TwitterUsername");
                }
            }
        }

        #endregion

        public TwitterViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                try
                {
                    AccessToken = IsolatedStorageSettings.ApplicationSettings["AccessToken"] as OAuthAccessToken;
                    TwitterUsername = IsolatedStorageSettings.ApplicationSettings["TwitterUsername"] as string;
                    TwitterPassword = IsolatedStorageSettings.ApplicationSettings["TwitterPassword"] as string;

                    SelectedPushpin = IsolatedStorageSettings.ApplicationSettings["SelectedPushpin"] as PushpinModel;
                    //We create the summary if it doesn't exist only when when authentication against ODAF website has been established
                    CommentService.Authenticated += (sender, e) =>
                        {
                            CommentService.CreateSummaryAsync(SelectedPushpin);
                        };
                    CommentService.SummaryCreated += (sender, e) =>
                        {
                            SummaryHasBeenCreated = true;
                        };
                    CommentService.AuthenticateAsync(AccessToken.Token, AccessToken.TokenSecret);
                    
                    if (AccessToken == null || TwitterUsername == null || TwitterPassword == null)
                    {
                        throw new KeyNotFoundException();
                    }
                    Twitter.AuthenticateWith(AccessToken.Token, AccessToken.TokenSecret);
                    HasUserAlreadyAuthorizedApp = true;
                }
                catch (KeyNotFoundException)
                {
                    HasUserAlreadyAuthorizedApp = false;
                    GetRequestToken();
                }
            }

            _AuthenticateUserCommand = new DelegateCommand(AuthenticateUser, (obj) => true);
            _SendTweetCommand = new DelegateCommand(SendTweet, CanSendTweet);
            _ResetTwitterCredentialsCommand = new DelegateCommand(ResetTwitterCredentials, (obj) => true);
            _TakePhotoCommand = new DelegateCommand(TakePhoto, (obj) => HasUserAlreadyAuthorizedApp);
        }

        /// <summary>
        /// This method permits to store the OAuthRequestToken
        /// </summary>
        private void GetRequestToken()
        {
            RequestToken = new OAuthRequestToken();
            AccessToken = new OAuthAccessToken();

            Twitter.GetRequestToken((request, Response) =>
            {
                if (Response.StatusCode == HttpStatusCode.OK)
                {
                    RequestToken = request;
                    Uri uri = Twitter.GetAuthorizationUri(RequestToken);
                    TwitterAuthorizationUri = uri;
                }
            });
        }

        /// <summary>
        /// This method permits to store the user OAuthAccessToken, and also his username and password to access Twitpic API.
        /// </summary>
        private void AuthenticateUser(object obj)
        {
            Twitter.GetAccessToken(RequestToken, TwitterCodePin, (access, Response) =>
            {
                if (Response.StatusCode == HttpStatusCode.OK)
                {
                    AccessToken = access; // Store it for reuse
                    Twitter.AuthenticateWith(access.Token, access.TokenSecret);
                    IsolatedStorageSettings.ApplicationSettings["AccessToken"] = AccessToken;
                    IsolatedStorageSettings.ApplicationSettings["TwitterUsername"] = TwitterUsername;
                    IsolatedStorageSettings.ApplicationSettings["TwitterPassword"] = TwitterPassword;
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        HasUserAlreadyAuthorizedApp = true;
                    });
                }
            });
        }

        /// <summary>
        /// This event serves only to indicate to the view that the tweet has been successfully posted.
        /// </summary>
        public event EventHandler TweetSent;

        private byte[] bytes;
        /// <summary>
        /// Method to send a tweet asynchronously.
        /// </summary>
        private void SendTweet(object obj)
        {
            IsUploading = true;
            TwitpicService service = new TwitpicService(bytes, TwitterUsername, TwitterPassword);
            service.TwitpicImageUploaded += (sender, e) =>
                {
                    if ((Tweet.Length + e.TwitpicImageUrl.Length) > 140)
                        Tweet = Tweet.Substring(0, 139 - e.TwitpicImageUrl.Length);

                    double latitude = (double)IsolatedStorageSettings.ApplicationSettings["SelectedPushpinLatitude"];
                    double longitude = (double)IsolatedStorageSettings.ApplicationSettings["SelectedPushpinLongitude"];
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                    Twitter.SendTweet(Tweet + " " + e.TwitpicImageUrl, latitude, longitude, (Status, Response) =>
                    {
                        if (Response.StatusCode == HttpStatusCode.OK)
                        {
                            if (SummaryHasBeenCreated == true)
                            {
                                CommentService.AddCommentAsync(SelectedPushpin.Guid, Tweet + " " + e.TwitpicImageUrl);
                            }
                            
                            if (TweetSent != null)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() =>
                                {
                                    IsUploading = false;
                                    TweetSent(this, new EventArgs());
                                });
                            }
                        }
                        else
                        {
                            Deployment.Current.Dispatcher.BeginInvoke(() =>
                            {
                                MessageBox.Show(Response.StatusDescription);
                            });
                        }
                    });
                };
            service.UploadPhoto();

       
        }

        /// <summary>
        /// Method to indicate if the SendTweetCommand can be invoked.
        /// </summary>
        /// <returns></returns>
        private bool CanSendTweet(object obj)
        {
            return HasUserAlreadyAuthorizedApp && (TwitpicImage != null);
        }

        /// <summary>
        /// Method to reset all the user-supplied credntials and identification informations
        /// </summary>
        private void ResetTwitterCredentials(object obj)
        {
            IsolatedStorageSettings.ApplicationSettings["AccessToken"] = null;
            IsolatedStorageSettings.ApplicationSettings["TwitterUsername"] = null;
            IsolatedStorageSettings.ApplicationSettings["TwitterPassword"] = null;

            HasUserAlreadyAuthorizedApp = false;
        }

        private void TakePhoto(object obj)
        {
            CameraCaptureTask task = new CameraCaptureTask();
            task.Completed += (sender, e) =>
                {
                    if (e.TaskResult == TaskResult.OK)
                    {
                        using (BinaryReader reader = new BinaryReader(e.ChosenPhoto))
                        {
                            bytes = reader.ReadBytes(Convert.ToInt32(e.ChosenPhoto.Length));
                            BitmapImage bmp = new BitmapImage();
                            bmp.SetSource(e.ChosenPhoto);
                            TwitpicImage = bmp;
                        }
                        (SendTweetCommand as DelegateCommand).RaiseCanExecuteChanged();
                    }
                };
            task.Show();
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
