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
using System.Resources;
using System.Collections.Generic;
using ODAF.SilverlightApp.Resources.UserControls;

namespace ODAF.SilverlightApp.Resources
{
    public class LocalizedStrings
    {
        public LocalizedStrings()
        {
        }

        private static SilverlightApp.Resources.MainPageResource mainPageResource = new MainPageResource();
        public SilverlightApp.Resources.MainPageResource MainPageResource { get { return mainPageResource; } }

        private static SilverlightApp.Resources.TwitterUserBadgeResource twitterUserBadgeResource = new TwitterUserBadgeResource();
        public SilverlightApp.Resources.TwitterUserBadgeResource TwitterUserBadgeResource { get { return twitterUserBadgeResource; } }

        private static SilverlightApp.Resources.PointDataViewAndSubViewsResource pointDataViewAndSubViewsResource = new PointDataViewAndSubViewsResource();
        public SilverlightApp.Resources.PointDataViewAndSubViewsResource PointDataViewAndSubViewsResource { get { return pointDataViewAndSubViewsResource; } }

        private static SilverlightApp.Resources.CreatePointViewResource createPointViewResource = new CreatePointViewResource();
        public SilverlightApp.Resources.CreatePointViewResource CreatePointViewResource { get { return createPointViewResource; } }

        private static SilverlightApp.Resources.UserControls.UserControlsViewsResource userControlsViewsResource = new UserControlsViewsResource();
        public SilverlightApp.Resources.UserControls.UserControlsViewsResource UserControlsViewsResource { get { return userControlsViewsResource; } }
    }

}
