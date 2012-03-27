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
using ODAF.SilverlightApp.CloudService;
using System.Windows.Media.Imaging;

namespace ODAF.SilverlightApp
{
    public partial class TwitterUserBadge : UserControl
    {
        private TwitterUser _currentUser;
        public TwitterUser CurrentUser 
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
                if (_currentUser != null)
                {
                    screenNameText.Text = "Welcome \n" + _currentUser.screen_name;
                    userImage.Source = new BitmapImage(new Uri(_currentUser.profile_image_url, UriKind.Absolute));
                    twitterImage.Visibility = Visibility.Collapsed;
                    screenNameText.Visibility = Visibility.Visible;
                    userImage.Visibility = Visibility.Visible;
                }
                else
                {
                    twitterImage.Visibility = Visibility.Visible;
                    screenNameText.Visibility = Visibility.Collapsed;
                    userImage.Visibility = Visibility.Collapsed;
                }

            }
        }
        public TwitterUserBadge()
        {
            InitializeComponent();
        }
    }
}
