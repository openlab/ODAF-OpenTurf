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

using System.Json;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Net.Browser;
using System.IO;
using System.Windows.Browser;
using ODAF.SilverlightApp.VO;




namespace ODAF.SilverlightApp.CloudService
{


    /*
    public class SummaryUpdateArgs : EventArgs
    {
        public PointDataSummary result;

        public SummaryUpdateArgs(PointDataSummary res)
        {
            this.result = res;
        }
    }

    

    public delegate void UserSummariesUpdateEventHandler(SummariesController sender);

    public delegate void CommunitySummariesUpdateEventHandler(SummariesController sender);
     * */

    public delegate void FeedUpdateEventHandler(FeedController sender);

    public class FeedController
    {
        public event FeedUpdateEventHandler FeedUpdate;
        //public event UserSummariesUpdateEventHandler UserSummaryUpdate;
        //public event CommunitySummariesUpdateEventHandler CommunitySummaryUpdate;

        public string BaseURL { get; set; }

        public CityFeed[] CityFeeds { get; set; }

        public ServiceController baseController { get; set; }

        public FeedController()
        {
            //PDSummaries = new Dictionary<string, PointDataSummary>();
            //UserSummaries = new Dictionary<string, PointDataSummary>();
            //CommunitySummaries = new Dictionary<string, PointDataSummary>();
        }

        #region "GetFeedList"

        public void GetFeedList()
        {
            string jsonUrl = BaseURL + "Feeds/List";

            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(onGetLayersForUserId);
            webClient.OpenReadAsync(new Uri(jsonUrl));
        }

        void onGetLayersForUserId(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(CityFeed[]));
                CityFeeds = (CityFeed[])serializer.ReadObject(e.Result);

                FeedUpdate(this);
            }
        }

        #endregion
    }
}



