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

namespace ODAF.SilverlightApp
{
    public class GeoTweet
    {
        public string profile_image_url;
        public DateTime created_at;
        public string from_user;
        public string text;
        public string location;
    }

    public class GeoTweetList
    {
        public GeoTweet[] results;
    }
}
