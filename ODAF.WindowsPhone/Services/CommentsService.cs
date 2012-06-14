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
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using ODAF.WindowsPhone.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace ODAF.WindowsPhone.Services
{
    /// <summary>
    /// This helper class proposes methods to interact with the CommentController of the ODAF website.
    /// </summary>
    public class CommentsService
    {
        private const string _commentsAddControllerRelativeUrl = "Comments/Add.json/";
        private const string _commentsListControllerRelativeUrl = "Comments/List.json/";
        private const string _summariesAddControllerRelativeUrl = "Summaries/Add.json";
        private const string _userAuthenticateControllerRelativeUrl = "User/Authenticate.json/";
 
        //This CookieContainer object is used to stock the Forms Authentication cookie. It is used by each service call which necessitate authentication.
        private CookieContainer container = new CookieContainer();

        public event EventHandler Authenticated;
        public event EventHandler SummaryCreated;

        /// <summary>
        /// Method to add a comment to the appropriate summary asynchronously.
        /// </summary>
        /// <param name="guid">GUID of the summary to which the comment will be linked.</param>
        public void AddCommentAsync(string guid, string text)
        {
            Uri uri = new Uri(App.OdafWebsiteUrl
            + _commentsAddControllerRelativeUrl
            + "0?guid="
            + guid);

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Headers["Text"] = text;
            request.Method = "POST";
            request.CookieContainer = container;
            request.ContentType = "application/x-www-form-urlencoded";
            request.BeginGetRequestStream(AddCommentPostRequest, request);
        }

        private void AddCommentPostRequest(IAsyncResult asyncResult)
        {
            WebRequest request = (WebRequest)asyncResult.AsyncState;
            Stream requestStream = request.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(requestStream);
            writer.Write("Text=" + request.Headers["Text"]);
            writer.Close();
            requestStream.Close();
            request.BeginGetResponse(AddCommentRespCallback, request);
        }

        private void AddCommentRespCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                }
            }
        }

        /// <summary>
        /// ODAF website authentication.
        /// </summary>
        /// <param name="oauthToken">OAuth access token.</param>
        /// <param name="oauthTokenSecret">OAuth access token secret.</param>
        public void AuthenticateAsync(string oauthToken, string oauthTokenSecret)
        {
            Uri uri = new Uri(App.OdafWebsiteUrl 
                + _userAuthenticateControllerRelativeUrl 
                + "?oauth_token="
                + oauthToken
                + "&oauth_token_secret="
                + oauthTokenSecret
                + "&appId="
                + App.AppId);

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.CookieContainer = container;
            request.BeginGetResponse(AuthenticationRespCallback, request);
        }

        private void AuthenticationRespCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //Successfull authentication
                    if (Authenticated != null)
                    {
                        Authenticated(this, new EventArgs());
                    }
                }     
            }
        }

        /// <summary>
        /// Create a PointDataSummary in the ODAF database asynchronously.
        /// The PointDataSummary properties will be mapped to the PushpinModel properties passed as a parameter.
        /// </summary>
        /// <param name="pushpinModel">the PushpinModel which serves as a base to create the PointDataSummary.</param>
        public void CreateSummaryAsync(PushpinModel pushpinModel)
        {
            Uri uri = new Uri(App.OdafWebsiteUrl
                + _summariesAddControllerRelativeUrl);

            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.CookieContainer = container;

            CreateSummaryUserState userState = new CreateSummaryUserState
            {
                Request = request,
                PushpinModel = pushpinModel
            };

            request.BeginGetRequestStream(CreateSummaryPostRequest, userState);
        }

        private void CreateSummaryPostRequest(IAsyncResult result)
        {
            CreateSummaryUserState userState = result.AsyncState as CreateSummaryUserState;
            PushpinModel pushpinModel = userState.PushpinModel;
            HttpWebRequest request = userState.Request;

            Stream requestStream = request.EndGetRequestStream(result);
            StreamWriter writer = new StreamWriter(requestStream);
            writer.Write("Description=" + " ");
            writer.Write("&LayerId=" + pushpinModel.Layer.Id);
            writer.Write("&Latitude=" + pushpinModel.Location.Latitude.ToString(CultureInfo.InvariantCulture));
            writer.Write("&Longitude=" + pushpinModel.Location.Longitude.ToString(CultureInfo.InvariantCulture));
            writer.Write("&Guid=" + pushpinModel.Guid);
            writer.Write("&Name=" + pushpinModel.Description);
            writer.Write("&Tag=" + " ");
            writer.Close();
            requestStream.Close();
            request.BeginGetResponse(CreateSummaryRespCallback, request);
        }

        private void CreateSummaryRespCallback(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (SummaryCreated != null)
            {
                SummaryCreated(this, new EventArgs());
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(result))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {

                    }
                }
            }
            catch (WebException)
            {
                //The summary already exists.                
            }
        }

        internal class CreateSummaryUserState
        {
            public HttpWebRequest Request { get; set; }
            public PushpinModel PushpinModel { get; set; }
        }

        /// <summary>
        /// A methods to get the comments associated to a pushpin. This comments are consumed from the ODAF service.
        /// </summary>
        /// <param name="guid">The Guid of the pushpin.</param>
        /// <param name="comments">The list of comments to be populated.</param>
        public void GetCommentsAsync(string guid, ObservableCollection<CommentModel> comments)
        {
            if (comments == null)
            {
                throw new ArgumentNullException("comments", "'comments' argument is null in the GetComments call.");
            }
            WebClient client = new WebClient();
            client.DownloadStringCompleted += GetCommentsAsync_DownloadStringCompleted;
            Uri uri = new Uri(App.OdafWebsiteUrl + _commentsListControllerRelativeUrl + "0?guid=" + guid, UriKind.Absolute);
            client.DownloadStringAsync(uri, comments);
        }

        private void GetCommentsAsync_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            ObservableCollection<CommentModel> comments = e.UserState as ObservableCollection<CommentModel>;
            if (e.Error == null)
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(e.Result);
                MemoryStream stream = new MemoryStream(byteArray);
                DataContractJsonSerializer serial = new DataContractJsonSerializer(typeof(List<CommentModel>), new[] { typeof(Comment) });
                List<CommentModel> list = serial.ReadObject(stream) as List<CommentModel>;
                stream.Close();
                list.ForEach(i => comments.Add(i));
            }
        }
    }
}
