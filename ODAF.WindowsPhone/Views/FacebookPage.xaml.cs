using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;
using Facebook;
using System.Text;
using ODAF.WindowsPhone.ViewModels;
using ODAF.WindowsPhone.Resources;

namespace ODAF.WindowsPhone.Views
{
    public partial class FacebookPage : PhoneApplicationPage
    {

        public FacebookPage()
        {
            InitializeComponent();
            var vm = this.Resources["FacebookViewModel"] as FacebookViewModel;
            vm.PhotoSentToFacebook += vm_PhotoSentToFacebook;
        }

        void vm_PhotoSentToFacebook(object sender, EventArgs e)
        {
            MessageBox.Show(FacebookResource.PhotoUploadedOnFacebook);
            NavigationService.GoBack();
        }

        private Uri GetFBLoginURL()
        {
            FacebookOAuthClient oauth = new FacebookOAuthClient { AppId = App.FacebookAppId };
            Dictionary<string, object> parameters = new Dictionary<string, object> 
            { 
                { "response_type", "token" }, 
                { "display", "touch" } 
            };

            if (App.FacebookExtendedPermissions != null && App.FacebookExtendedPermissions.Length > 0)
            {
                StringBuilder scope = new StringBuilder();
                scope.Append(string.Join(",", App.FacebookExtendedPermissions));
                parameters["scope"] = scope.ToString();
            }
            var url = oauth.GetLoginUrl(parameters);
            return url;
        }

        private void LoginToFacebook()
        {
            FacebookAuthBrowser.Navigate(GetFBLoginURL());
        }

        //The first time the application is launched (or if the app has been reseted), the user need to authorize the application to access his Facebook account.
        private void FacebookAuthBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            string facebookAccessToken;
            if (!IsolatedStorageSettings.ApplicationSettings.TryGetValue("FacebookAccessToken", out facebookAccessToken) || (facebookAccessToken == null))
            {
                LoginToFacebook();
            }
            else
            {
                FacebookAuthBrowser.Visibility = Visibility.Collapsed;
            }
        }

        private void FacebookAuthBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult result;
            if (FacebookOAuthResult.TryParse(e.Uri, out result))
            {
                if (result.IsSuccess)
                {
                    //user logged in successfully and access token is returned from facebook.
                    //Save that Access token for future use.
                    FacebookApp client = new FacebookApp(result.AccessToken);
                    client.GetAsync("me", (facebookResult) => 
                    {
                        IsolatedStorageSettings.ApplicationSettings["FacebookUserId"] = (facebookResult.Result as JsonObject)["id"].ToString();
                    });
                    
                    IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"] = result.AccessToken;
                    FacebookAuthBrowser.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MessageBox.Show(result.ErrorDescription + " \n" + result.ErrorReason);
                }
            } 
        }


        private void BackIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        private void PhotoIconButton_Click(object sender, EventArgs e)
        {
            var vm = this.Resources["FacebookViewModel"] as FacebookViewModel;
            vm.TakePhotoCommand.Execute(null);
        }

        private void ResetIconButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(FacebookResource.AreYouSureToResetCredentials, FacebookResource.Reset, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings["FacebookAccessToken"] = null;
                NavigationService.GoBack();
            }
        }

        #region AppBar

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            UpdateApplicationBar();
        }

        private void UpdateApplicationBar()
        {
            UpdateButton(0, FacebookResource.Back);
            UpdateButton(1, FacebookResource.Photo);
            UpdateButton(2, FacebookResource.Reset);
        }

        protected void UpdateButton(int index, string text)
        {
            var button = this.ApplicationBar.Buttons[index] as Microsoft.Phone.Shell.ApplicationBarIconButton;
            if (null != button)
            {
                button.Text = text.ToLower();
            }
        }

        #endregion
    }
}