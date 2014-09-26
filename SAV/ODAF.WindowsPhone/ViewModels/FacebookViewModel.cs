using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Facebook;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using ODAF.WindowsPhone.Commands;

namespace ODAF.WindowsPhone.ViewModels
{
    public class FacebookViewModel : INotifyPropertyChanged
    {
        private byte[] bytes;

        #region Commands

        private ICommand _TakePhotoCommand;
        public ICommand TakePhotoCommand
        {
            get { return _TakePhotoCommand; }
        }

        private ICommand _UploadPhotoToFacebookCommand;
        public ICommand UploadPhotoToFacebookCommand
        {
            get { return _UploadPhotoToFacebookCommand; }
        }

        #endregion

        #region Properties


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
        

        private BitmapImage _FacebookImage;
        /// <summary>
        /// Represents the image that will be uploaded to Facebook. This property is set after the image has been uploaded.
        /// </summary>
        public BitmapImage FacebookImage
        {
            get { return _FacebookImage; }
            set
            {
                if (value != _FacebookImage)
                {
                    _FacebookImage = value;
                    NotifyPropertyChanged("FacebookImage");
                }
            }
        }

        private string _PhotoLegend;
        /// <summary>
        /// Represents the description of the photo
        /// </summary>
        public string PhotoLegend
        {
            get { return _PhotoLegend; }
            set
            {
                if (value != _PhotoLegend)
                {
                    _PhotoLegend = value;
                    NotifyPropertyChanged("PhotoLegend");
                    (UploadPhotoToFacebookCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            }
        } 

        #endregion

        public FacebookViewModel()
        {
            _TakePhotoCommand = new DelegateCommand(TakePhoto, (obj) => true);
            _UploadPhotoToFacebookCommand = new DelegateCommand(UploadPhotoToFacebook, (obj) => (FacebookImage != null && !string.IsNullOrEmpty(PhotoLegend)));
        }

        /// <summary>
        /// Launches the camera cand save the captured photo for future upload.
        /// </summary>
        /// <param name="obj">Useless </param>
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
                        FacebookImage = bmp;
                    }
                    (UploadPhotoToFacebookCommand as DelegateCommand).RaiseCanExecuteChanged();
                }
            };
            task.Show();
        }

        /// <summary>
        /// The first method to launch the upload process on Facebook.
        /// </summary>
        /// <param name="obj">Useless (Compatibility with ICommand)</param>
        private void UploadPhotoToFacebook(object obj)
        {
            IsUploading = true;
            string facebookAccessToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"].ToString();
            string facebookUserId;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("FacebookUserId", out facebookUserId) || (facebookUserId == null))
                CreateAlbum(facebookAccessToken);
            else
            {
                GetAlbum(facebookAccessToken, facebookUserId);
            }
        }

        /// <summary>
        /// A method which executes a FQL query to get the album Id to which the photo will be uploaded.
        /// </summary>
        /// <param name="accessToken">the OAuth access token</param>
        /// <param name="albumId">Id of the album's owner (current user)</param>
        private void GetAlbum(string accessToken, string userId)
        {
            FacebookApp facebookClient = new FacebookApp(accessToken);

            facebookClient.QueryAsync("SELECT object_id, name FROM album WHERE owner=\"" + userId + "\"", GetAlbumAsyncCallback);
        }

        /// <summary>
        /// A method to create an ODAF album on Facebook.
        /// </summary>
        /// <param name="accessToken">the OAuth access token</param>
        private void CreateAlbum(string accessToken)
        {
            FacebookApp facebookClient = new FacebookApp(accessToken);
            Dictionary<string, object> albumParameters = new Dictionary<string, object>();
            albumParameters.Add("message", App.FacebookAlbumDescription);
            albumParameters.Add("name", App.FacebookAlbumName);
            facebookClient.PostAsync("/me/albums", albumParameters, CreateAlbumAsyncCallback);
        }

        /// <summary>
        /// This method uploads the stored photo to Facebook on the given album Id.
        /// </summary>
        /// <param name="accessToken">the OAuth access token</param>
        /// <param name="AlbumId">The album Id to which the photo will belong.</param>
        private void UploadPhoto(string accessToken, string AlbumId)
        {
            FacebookApp facebookClient = new FacebookApp(accessToken);
            FacebookMediaObject mediaObject = new FacebookMediaObject
            {
                FileName = "image",
                ContentType = "image/png"
            };
            byte[] fileBytes = bytes;
            mediaObject.SetValue(fileBytes);
            IDictionary<string, object> upload = new Dictionary<string, object>();
            upload.Add("name", "photo name");
            upload.Add("message", PhotoLegend);
            upload.Add("@file.jpg", mediaObject);
            facebookClient.PostAsync("/" + AlbumId + "/photos", upload, UploadPhotoAsyncCallback);
        }

        /// <summary>
        /// Asynchronous callback of the GetAlbum method.
        /// </summary>
        /// <param name="result">Async result.</param>
        private void GetAlbumAsyncCallback(FacebookAsyncResult result)
        {
            //If the user has suppressed the album, we need to re-create it in the catch block.
            try
            {
                JsonArray array = result.Result as JsonArray;
                foreach (JsonObject obj in array)
                {
                    string name = obj["name"].ToString();
                    if (name == App.FacebookAlbumName)
                    {
                        string albumId = obj["object_id"].ToString();
                        string facebookAccessToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"].ToString();
                        UploadPhoto(facebookAccessToken, albumId);
                        return;
                    }
                }
                throw new Exception();
            }
            catch
            {
                string facebookAccessToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"].ToString();
                CreateAlbum(facebookAccessToken);
            }
        }

        /// <summary>
        /// Asynchronous callback of the CreateAlbum method.
        /// </summary>
        /// <param name="result">Async result.</param>
        private void CreateAlbumAsyncCallback(FacebookAsyncResult result)
        {
            JsonObject obj = result.Result as JsonObject;
            string albumId = obj["id"].ToString();
            string facebookAccessToken = IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"].ToString();
            UploadPhoto(facebookAccessToken, albumId);
        }

        /// <summary>
        /// This event is raised when the photo has been successfully uploaded to Facebook.
        /// </summary>
        public event EventHandler PhotoSentToFacebook;

        /// <summary>
        /// Asynchronous callback of the UploadPhoto method.
        /// </summary>
        /// <param name="result">Async result.</param>
        private void UploadPhotoAsyncCallback(FacebookAsyncResult result)
        {
            JsonObject obj = result.Result as JsonObject;
            if (PhotoSentToFacebook != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => 
                {
                    IsUploading = false;
                    PhotoSentToFacebook(this, new EventArgs());
                });
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
