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
using ODAF.WindowsPhone.Resources;

namespace ODAF.WindowsPhone.Helpers
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static MainResource mainResource = new MainResource();
        public MainResource MainResource { get { return mainResource; } }

        private static TwitterResource twitterResource = new TwitterResource();
        public TwitterResource TwitterResource { get { return twitterResource; } }

        private static FacebookResource facebookResource = new FacebookResource();
        public FacebookResource FacebookResource { get { return facebookResource; } }
    }
}
