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
using ODAF.SilverlightApp.VO;
using System.Collections.Generic;
using System.Net.Browser;
using System.IO;
using System.Windows.Browser;


namespace ODAF.SilverlightApp.CloudService
{

    public class SummaryDeletedArgs : EventArgs
    {
        public string guid;
        public SummaryDeletedArgs(string res)
        {
            this.guid = res;
        }
    }

    public delegate void SummaryDeletedEventHandler(object sender, SummaryDeletedArgs e);
    
    public class SummaryUpdateArgs : EventArgs
    {
        public PointDataSummary result;

        public SummaryUpdateArgs(PointDataSummary res)
        {
            this.result = res;
        }
    }

    public delegate void SummaryUpdateEventHandler(object sender, SummaryUpdateArgs e);

    public delegate void UserSummariesUpdateEventHandler(SummariesController sender);

    public delegate void CommunitySummariesUpdateEventHandler(SummariesController sender);

    public class SummariesController
    {
        public static int kMaxSummaryAgeSeconds = 30; // Max age in seconds, before we refresh

        public event SummaryDeletedEventHandler SummaryDeleted;
        public event SummaryUpdateEventHandler SummaryUpdate;
        public event UserSummariesUpdateEventHandler UserSummaryUpdate;
        public event CommunitySummariesUpdateEventHandler CommunitySummaryUpdate;

        public Dictionary<string, PointDataSummary> PDSummaries;

        public Dictionary<string, PointDataSummary> UserSummaries;

        public Dictionary<string, PointDataSummary> CommunitySummaries;

        public string BaseURL { get; set; }

        public ServiceController baseController { get; set; }

        public SummariesController()
        {
            PDSummaries = new Dictionary<string, PointDataSummary>();
            UserSummaries = new Dictionary<string, PointDataSummary>();
            CommunitySummaries = new Dictionary<string, PointDataSummary>();
        }

        #region "GetLayersForUserId"

        public void GetLayersForUserId(long id)
        {
            string jsonUrl = BaseURL + "Summaries/ShowLayersByUserId.json/" + id;

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
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(String[]));
                
                String[] res = (String[])serializer.ReadObject(e.Result);

                // TODO: dispatch event

                //GetByLayerId(res[0]);
            }
        }

        #endregion


        #region "GetSummaryForId"

        public void GetSummaryForId(string id)
        {
            string jsonUrl = BaseURL + "Summaries/ShowById.json/" + id;
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(onGetSummariesForId);
            webClient.OpenReadAsync(new Uri(jsonUrl));
        }

        public void onGetSummariesForId(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary[]));
                PointDataSummary[] res = (PointDataSummary[])serializer.ReadObject(e.Result);
                for (int n = 0; n < res.Length; n++)
                {
                    string guid = res[n].Guid;
                    CommunitySummaries[guid] = res[n];
                }

            }

            CommunitySummaryUpdate(this);
        }

        #endregion

        #region "GetByLayerId"

        public void GetByLayerId(string layerId)
        {
            string jsonUrl = BaseURL + "Summaries/ShowByLayerId.json?layerId=" + layerId;
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(onGetByLayerId);
            webClient.OpenReadAsync(new Uri(jsonUrl));

        }

        void onGetByLayerId(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary[]));
                PointDataSummary[] res = (PointDataSummary[])serializer.ReadObject(e.Result);

                DateTime dt = DateTime.Now;
                for (int n = 0; n < res.Length; n++)
                {
                    string guid = res[n].Guid;
                    //if (this.UserSummaries.ContainsKey(guid))
                    //{
                    //    //PointDataSummary pds = UserSummaries[guid];
                    //    UserSummaries[guid].
                    //}
                    UserSummaries[guid] = res[n];
                }
                UserSummaryUpdate(this);
            }
        }


        #endregion



        public void GetSummariesForCommunity()
        {
            string jsonUrl = BaseURL + "Summaries/ShowForCommunityByActivity.json?page=0&page_size=100";
            WebClient webClient = new WebClient();
            webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(onGetSummariesForCommunity);
            webClient.OpenReadAsync(new Uri(jsonUrl));
        }

        public void onGetSummariesForCommunity(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary[]));
                PointDataSummary[] res = (PointDataSummary[])serializer.ReadObject(e.Result);
                for (int n = 0; n < res.Length; n++)
                {
                    string guid = res[n].Guid;
                    CommunitySummaries[guid] = res[n];
                }

            }

            CommunitySummaryUpdate(this);
        }


        public PointDataSummary GetForPlaceMark(PlaceMark pm)
        {
            PointDataSummary ret = null;
            string guid = pm.Id;

            if (PDSummaries.ContainsKey(guid))
            {

                ret = PDSummaries[guid];
                
            }
            else
            {
                ret = new PointDataSummary();
                ret.Guid = guid;
                ret.Name = pm.name; // this is temp until data is retrieved
                ret.Latitude = pm.Location.Latitude;
                ret.Longitude = pm.Location.Longitude;
                ret.LayerId = pm.FeedId;

                PDSummaries[guid] = ret;
            }

            if (!IsCurrent(guid))
            {
                UpdateSummaryByGuid(guid);
            }

            return ret;
        }


        public bool IsCurrent(string guid)
        {
            if (PDSummaries.ContainsKey(guid))
            {
                return (DateTime.Now - PDSummaries[guid].LastRefreshed).TotalSeconds < kMaxSummaryAgeSeconds;
            }
            return false;
        }

        public PointDataSummary ShowByGuid(string guid)
        {
            PointDataSummary ret = null;
            if (PDSummaries.ContainsKey(guid))
            {
                ret = PDSummaries[guid];
            }
            else
            {
                ret = new PointDataSummary();
                ret.Guid = guid;
                PDSummaries[guid] = ret;
            }

            if (!IsCurrent(guid))
            {
                UpdateSummaryByGuid(guid);
            }

            return ret;

        }

        public void UpdateSummaryByGuid(string guid)
        {
            string jsonUrl = BaseURL + "Summaries/ShowByGuid.json?guid=" + guid;
            WebClient placemarkClient = new WebClient();
            placemarkClient.Headers["RequestGuid"] = guid;
            placemarkClient.OpenReadCompleted += new OpenReadCompletedEventHandler(ShowByGuid_OpenReadCompleted);
            placemarkClient.OpenReadAsync(new Uri(jsonUrl));
        }

        private void ShowByGuid_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                WebClient client = (WebClient)sender;
                string guid = client.Headers["RequestGuid"];
                if (PDSummaries.ContainsKey(guid))
                {
                    // TODO: Create a new summary on the server based on the info we have
                    if(this.baseController.User.IsAuthenticated)
                    {
                        CreatePointDataSummary(PDSummaries[guid]);
                    }
                }
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary));
                PointDataSummary res = (PointDataSummary)serializer.ReadObject(e.Result);
                res.LastRefreshed = DateTime.Now;
                if (res != null && res.Id != 0 )
                {
                    if (PDSummaries.ContainsKey(res.Guid))
                    {
                        res.CurrentUserRating = PDSummaries[res.Guid].CurrentUserRating;
                        PDSummaries[res.Guid] = res;
                    }
                    if (res.CommentCount > 0)
                    {
                        GetCommentsForSummary(res.Guid);
                    }

                }
                else // does not exist, let's create it
                {
                    
                }
                SummaryUpdateArgs args = new SummaryUpdateArgs(res);
                SummaryUpdate(this, args);
            }
        }

        #region "RemovePlacemark"

        public void RemovePointDataSummary(string guid)
        {
            PointDataSummary summ = PDSummaries[guid];
            if (summ != null)
            {
                string jsonUrl = BaseURL + "Summaries/Remove.json/" + summ.Id;

                WebClient webClient = new WebClient();
                webClient.Headers["RequestGuid"] = guid;
                webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(onRemovePointDataSummary);
                webClient.OpenReadAsync(new Uri(jsonUrl));
            }
        }

        void onRemovePointDataSummary(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
            else
            {
                WebClient webClient = sender as WebClient;
                if (webClient != null)
                {
                    string guid = webClient.Headers["RequestGuid"];

                    // Remove it from our cache :
                    PDSummaries.Remove(guid);
                    CommunitySummaries.Remove(guid);
                    UserSummaries.Remove(guid);
                    SummaryDeletedArgs args = new SummaryDeletedArgs(guid);
                    SummaryDeleted(this, args);
                    
                }

            }
        }

        #endregion

        #region "EditPlacemark"

        public void EditPointDataSummary(PointDataSummary val)
        {
            PDSummaries[val.Guid] = val;
            string jsonUrl = BaseURL + "Summaries/Edit.json/" + val.Id;
            WebRequest req = WebRequest.Create(jsonUrl);
            req.Headers["RequestGuid"] = val.Guid;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.BeginGetRequestStream(EditPlaceMarkSummaryPostRequest, req);
        }

        void EditPlaceMarkSummaryPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            // "Description", "LayerId", "Latitude", "Longitude", "Tag", "Guid"
            string guid = req.Headers["RequestGuid"];
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary currentSummary = PDSummaries[guid];

                writer.Write("Description=" + HttpUtility.UrlEncode(currentSummary.Description));
                writer.Write("&LayerId=" + currentSummary.LayerId);
                writer.Write("&Latitude=" + HttpUtility.UrlEncode(currentSummary.Latitude.ToString()));
                writer.Write("&Longitude=" + HttpUtility.UrlEncode(currentSummary.Longitude.ToString()));
                writer.Write("&Guid=" + currentSummary.Guid);
                writer.Write("&Name=" + HttpUtility.UrlEncode(currentSummary.Name));
                writer.Write("&Tag=" + currentSummary.Tag);

                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(onEditPlaceMarkSummary, req);
            }
            else
            {
                req.Abort(); // ??
            }



        }

        void onEditPlaceMarkSummary(IAsyncResult asyncResult)
        {
            string result;
            WebRequest request = (WebRequest)asyncResult.AsyncState;

            // Get the response stream.
            WebResponse response = null;

            try
            {
                response = request.EndGetResponse(asyncResult);
                Stream responseStream = response.GetResponseStream();
                // Read the returned text.
                // StreamReader reader = new StreamReader(responseStream);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary));
                PointDataSummary summary = (PointDataSummary)serializer.ReadObject(responseStream);
                if (PDSummaries.ContainsKey(summary.Guid))
                {
                    summary.CurrentUserRating = PDSummaries[summary.Guid].CurrentUserRating;
                }
                PDSummaries[summary.Guid] = summary;
                SummaryUpdateArgs args = new SummaryUpdateArgs(summary);
                SummaryUpdate(this, args);
            }
            catch (Exception e)
            {
                result = "Error contacting service. " + e.Message;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
        }


        #endregion

        #region "CreatePlacemark"

        public void CreatePointDataSummary(PointDataSummary val)
        {

            PDSummaries[val.Guid] = val;
            string jsonUrl = BaseURL + "Summaries/Add.json";
            WebRequest req = WebRequest.Create(jsonUrl);
            req.Headers["RequestGuid"] = val.Guid;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.BeginGetRequestStream(CreatePlaceMarkSummaryPostRequest, req);
        }

        void CreatePlaceMarkSummaryPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            // "Description", "LayerId", "Latitude", "Longitude", "Tag", "Guid"
            string guid = req.Headers["RequestGuid"];
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary currentSummary = PDSummaries[guid];

                writer.Write("Description=" + HttpUtility.UrlEncode(currentSummary.Description));
                writer.Write("&LayerId=" + currentSummary.LayerId);
                writer.Write("&Latitude=" + HttpUtility.UrlEncode(currentSummary.Latitude.ToString()));
                writer.Write("&Longitude=" + HttpUtility.UrlEncode(currentSummary.Longitude.ToString()));
                writer.Write("&Guid=" + currentSummary.Guid);
                writer.Write("&Name=" + currentSummary.Name);
                writer.Write("&Tag=" + currentSummary.Tag);

                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(onCreatedPlaceMarkSummary, req);
            }
            else
            {
                req.Abort(); // ??
            }
            


        }

        void onCreatedPlaceMarkSummary(IAsyncResult asyncResult)
        {
            string result;
            WebRequest request = (WebRequest)asyncResult.AsyncState;

            // Get the response stream.
            WebResponse response = null;
            
            try
            {
                response = request.EndGetResponse(asyncResult);
                Stream responseStream = response.GetResponseStream();
                // Read the returned text.
                // StreamReader reader = new StreamReader(responseStream);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary));
                PointDataSummary summary = (PointDataSummary)serializer.ReadObject(responseStream);
                if(PDSummaries.ContainsKey(summary.Guid))
                {
                    summary.CurrentUserRating = PDSummaries[summary.Guid].CurrentUserRating;
                }
                PDSummaries[summary.Guid] = summary;
                SummaryUpdateArgs args = new SummaryUpdateArgs(summary);
                SummaryUpdate(this, args);
            }
            catch(Exception e)
            {
                result = "Error contacting service. " + e.Message;
            }
            finally
            {
                if(response != null)
                    response.Close();
            }
        }

        #endregion

        #region "AddTag"

        public void AddTag(string guid, string tag)
        {
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary tempSummary = PDSummaries[guid];

                string jsonUrl = BaseURL + "Summaries/AddTag.json/" + tempSummary.Id;

                WebRequest req = WebRequest.Create(jsonUrl);
                req.Headers["RequestGuid"] = guid;
                req.Headers["UserTag"] = tag;
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.BeginGetRequestStream(AddTagPostRequest, req);
            }
            else
            {
                // wtf?
            }
        }

        void AddTagPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            string guid = req.Headers["RequestGuid"];
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary tempSummary = PDSummaries[guid];

                writer.Write("&tag=" + req.Headers["UserTag"]);

                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(OnAddTag, req);
            }
            else
            {
                req.Abort(); // ??
            }
        }

        void OnAddTag(IAsyncResult asyncResult)
        {
            string result = "";

            WebRequest request = (WebRequest)asyncResult.AsyncState;

            // Get the response stream.
            WebResponse response = request.EndGetResponse(asyncResult);
            Stream responseStream = response.GetResponseStream();
            try
            {
                // Read the returned text.
                // StreamReader reader = new StreamReader(responseStream);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary));
                PointDataSummary addTagResult = (PointDataSummary)serializer.ReadObject(responseStream);
                string guid = request.Headers["RequestGuid"];
                if (PDSummaries.ContainsKey(guid))
                {
                    PointDataSummary tempSummary = PDSummaries[guid];
                    tempSummary.Tag = addTagResult.Tag;
                    tempSummary.CurrentUserRating = PDSummaries[guid].CurrentUserRating;
                    PDSummaries[guid] = tempSummary;

                    SummaryUpdateArgs args = new SummaryUpdateArgs(tempSummary);
                    SummaryUpdate(this, args);

                }
            }
            catch (Exception e)
            {
                result = "Error contacting service." + e.Message;
            }
            finally
            {
                response.Close();
            }
        }

        #endregion

        #region "AddRating"
        // Add Rating

        public void AddRating(string guid, int rating)
        {
            if (PDSummaries.ContainsKey(guid) && PDSummaries[guid].Id > 0)
            {
                if (baseController.User.IsAuthenticated)
                {
                    PointDataSummary tempSummary = PDSummaries[guid];

                    PDSummaries[guid].CurrentUserRating = rating;

                    string jsonUrl = BaseURL + "Summaries/AddRating.json/" + tempSummary.Id;

                    WebRequest req = WebRequest.Create(jsonUrl);
                    req.Headers["RequestGuid"] = guid;
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.BeginGetRequestStream(AddRatingPostRequest, req);
                }
                else
                {
                   // TODO: Else WHAT?
                }
            }
            else
            {

            }
        }



        void AddRatingPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            string guid = req.Headers["RequestGuid"];
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary tempSummary = PDSummaries[guid];

                writer.Write("&rating=" + tempSummary.CurrentUserRating);
   
                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(OnAddRating, req);
            }
            else
            {
                req.Abort(); // ??
            }
        }

        void OnAddRating(IAsyncResult asyncResult)
        {
            string result = "";
            
            WebRequest request = (WebRequest)asyncResult.AsyncState;
            
            // Get the response stream.
            WebResponse response = request.EndGetResponse(asyncResult);
            Stream responseStream = response.GetResponseStream();
            try
            {
                // Read the returned text.
                // StreamReader reader = new StreamReader(responseStream);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PointDataSummary));
                PointDataSummary addRatingResult = (PointDataSummary)serializer.ReadObject(responseStream);
                string guid = request.Headers["RequestGuid"];
                if (PDSummaries.ContainsKey(guid))
                {
                    PointDataSummary tempSummary = PDSummaries[guid];
                    tempSummary.RatingCount = addRatingResult.RatingCount;
                    tempSummary.RatingTotal = addRatingResult.RatingTotal;
                    tempSummary.CurrentUserRating = PDSummaries[guid].CurrentUserRating;
                    PDSummaries[guid] = tempSummary;

                    SummaryUpdateArgs args = new SummaryUpdateArgs(tempSummary);
                    SummaryUpdate(this, args);

                }
            }
            catch(Exception e)
            {
                result = "Error contacting service." + e.Message;
            }
            finally
            {
                response.Close();
            }
        }

        #endregion

        #region "Comments"

        // List(long id, string format, int? page, int? page_size)
        public void GetCommentsForSummary(string guid)
        {
            GetCommentsForSummary(guid, null, null);
        }
        public void GetCommentsForSummary(string guid, int? page)
        {
            GetCommentsForSummary(guid, page, null);
        }
        public void GetCommentsForSummary(string guid, int? page, int? pageSize)
        {
            if (PDSummaries.ContainsKey(guid))
            {
                PointDataSummary tempSummary = PDSummaries[guid];

                string jsonUrl = BaseURL + "Comments/List.json/" + tempSummary.Id;
                if (page != null)
                {
                    jsonUrl += "?page=" + page;

                    if (pageSize != null)
                    {
                        jsonUrl += "&pageSize=" + pageSize;
                    }
                }

                WebClient placemarkClient = new WebClient();
                placemarkClient.Headers["RequestGuid"] = guid;
                placemarkClient.OpenReadCompleted += new OpenReadCompletedEventHandler(GetComments_OpenReadCompleted);
                placemarkClient.OpenReadAsync(new Uri(jsonUrl));
            }
            else
            {
                // wtf?
            }
        }

        private void GetComments_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            WebClient client = (WebClient)sender;
            string guid = client.Headers["RequestGuid"];

            if (e.Error != null)
            {
                if (PDSummaries.ContainsKey(guid))
                {
                    // ??
                }

            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UserComment[]));
                UserComment[] res = (UserComment[])serializer.ReadObject(e.Result);
                if (res != null && res.Length != 0)
                {
                    if (PDSummaries.ContainsKey(guid))
                    {
                        PDSummaries[guid].UserComments = res;
                        SummaryUpdateArgs args = new SummaryUpdateArgs(PDSummaries[guid]);
                        SummaryUpdate(this, args);
                    }
                }
                else 
                {
                    // failed, hmm, it should at least be an empty array ...
                }
            }
        }



        #endregion

        public void AddComment(string guid, string text, bool bAutoTweet)
        {
            if (PDSummaries.ContainsKey(guid))
            {
                string jsonUrl = BaseURL + "Comments/Add.json/" + PDSummaries[guid].Id;

                WebRequest req = WebRequest.Create(jsonUrl);
                req.Headers["RequestGuid"] = guid;
                req.Headers["AutoTweet"] = bAutoTweet.ToString();
                req.Headers["RequestCommentBody"] = HttpUtility.UrlEncode(text); 
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.BeginGetRequestStream(AddCommentPostRequest, req);
            }
            else
            {
                // wtf?
            }
        }

        private void AddCommentPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            string guid = req.Headers["RequestGuid"];
            if (PDSummaries.ContainsKey(guid))
            {
                writer.Write("Text=" + req.Headers["RequestCommentBody"]);
   
                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(OnAddComment, req);
            }
            else
            {
                req.Abort(); // ??
            }
        }

        private void OnAddComment(IAsyncResult asyncResult)
        {
            string result = "";
            
            WebRequest request = (WebRequest)asyncResult.AsyncState;
            
            // Get the response stream.
            WebResponse response = null;
           
            try
            {
                response = request.EndGetResponse(asyncResult);
                Stream responseStream = response.GetResponseStream();
                // Read the returned text.
                // StreamReader reader = new StreamReader(responseStream);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UserComment));
                UserComment addedComment = (UserComment)serializer.ReadObject(responseStream);
                string guid = request.Headers["RequestGuid"];
                bool bAutoTweet = (request.Headers["AutoTweet"] == bool.TrueString);
                if (PDSummaries.ContainsKey(guid))
                {
                    PointDataSummary tempSummary = PDSummaries[guid];
                   // PDSummaries.Remove(guid);
                   //this.ShowByGuid(guid);
                    tempSummary.CommentCount++;
                    GetCommentsForSummary(guid);

                    if (bAutoTweet)
                    {
                        string tweetText = request.Headers["RequestCommentBody"];
                        FormatAndPostCommentToTwitter(tweetText, tempSummary, addedComment.Comment.Id);
                        
                        
                    }
                }

            }
            catch(Exception e)
            {
                result = "Error contacting service." + e.Message;
                
            }
            finally
            {
                if(response!=null)
                    response.Close();
            }
        }

        public void FormatAndPostCommentToTwitter(string tweetText, PointDataSummary pds, int commentId)
        {
            string reqUrl = this.BaseURL + "Home/GetLinkForComment.json/" + commentId;
           
            WebClient shurlClient = new WebClient();
            shurlClient.OpenReadCompleted += new OpenReadCompletedEventHandler(shurlClient_OpenReadCompleted);
            shurlClient.Headers["RequestGuid"] = pds.Guid;
            shurlClient.Headers["TweetText"] = tweetText;
            shurlClient.OpenReadAsync(new Uri(reqUrl));
        }

        void shurlClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // wtf ?
                return;
            }

            WebClient client = (WebClient)sender;
            string guid = client.Headers["RequestGuid"];
            string tweetText = client.Headers["TweetText"];
            PointDataSummary tempSummary = PDSummaries[guid];
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SummaryUrl));
            SummaryUrl res = (SummaryUrl)serializer.ReadObject(e.Result);

            string elipsis = " ... ";
            int len = res.short_url.Length + elipsis.Length;
            int maxLen = 140 - len;

            string truncTweet = tweetText;

            if (tweetText.Length > maxLen)
            {
                truncTweet = tweetText.Substring(0, maxLen);
            }

            
            truncTweet += elipsis + res.short_url;
            this.baseController.User.UpdateTwitterStatus(truncTweet, tempSummary.Latitude, tempSummary.Longitude);
        }



    }

    
}
