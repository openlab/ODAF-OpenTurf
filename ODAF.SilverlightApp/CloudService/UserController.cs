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
using System.Windows.Messaging;
using System.Windows.Browser;
using System.IO;
using System.Globalization;

namespace ODAF.SilverlightApp.CloudService
{
    public class TwitterAuthTokenResult
    {
        public string link { get; set; }
        public string oauth_token { get; set; }
        public string oauth_token_secret { get; set; }
    }

    public class TwitterUser
    {
        // url to twitter image
        public string profile_image_url { get; set; }
        // twitter screen name ! NOT unique, and can change
        public string screen_name { get; set; }
        // unique within twitter
        public int user_id { get; set; }
        // Id within our system
        public long Id { get; set; }
    }

    public delegate void AuthUpdateEventHandler(UserController sender);

    public class UserController
    {
        public event AuthUpdateEventHandler AuthUpdate;

        public string BaseURL { get; set; }

        //private LocalMessageReceiver receiver;

        private TwitterAuthTokenResult oauthTokenResult;

        public TwitterUser currentUser;

        private string AppId = ((App)Application.Current).twitterAppId;

        public UserController()
        { }

        public void GetUser()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted +=
                new OpenReadCompletedEventHandler(Current_Result);

            client.OpenReadAsync(new Uri(BaseURL + "User/Current.json"));
        }

        private void Current_Result(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterUser));
                currentUser = (TwitterUser)serializer.ReadObject(e.Result);
                AuthUpdate(this);
            }
        }

        public void OnTwitterCallbackMessageReceived(string oauth_token, string oauth_verifier)
        {
            // if we get here, we should have a token!
            GetAccessToken(oauth_token, oauth_verifier);
        }

        public void RequestAuthToken()
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += RequestAuthToken_Result;
            client.OpenReadAsync(new Uri(BaseURL + "User/RequestAuthToken.json?appId=" + AppId));
        }

        private void RequestAuthToken_Result(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            { }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterAuthTokenResult));
                oauthTokenResult = (TwitterAuthTokenResult)serializer.ReadObject(e.Result);
                // TODO: Can we already be logged in?
                //Authenticate(oauthTokenResult.oauth_token, oauthTokenResult.oauth_token_secret);
                ScriptObject script = (ScriptObject)HtmlPage.Window.GetProperty("connectTW");
                script.InvokeSelf(oauthTokenResult.link);

            }
        }

        public void GetAccessToken(string oauth_token, string oauth_verifier)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(GetAccessToken_Result);
            client.OpenReadAsync(new Uri(BaseURL + "User/GetAccessToken.json?appId=" + AppId + "&oauth_token=" + oauth_token + "&oauth_verifier=" + oauth_verifier));
        }

        // Result	"{\"oauth_token\":\"15623120-KrsY3eOekxUe7nfIJe5B1YnEj9Y7WaMIWjFnQc6ej\",
        // \"oauth_token_secret\":\"ZcwHFnovmS6MeMPglSXMcqQ33qE9k2zojUidn86TU\"}"	string

        private void GetAccessToken_Result(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: Handle this somehow ...
                MessageBox.Show("GetAccessToken_Result Error: " + e.Error.Message);
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterAuthTokenResult));
                this.oauthTokenResult = (TwitterAuthTokenResult)serializer.ReadObject(e.Result);
                this.Authenticate(oauthTokenResult.oauth_token, oauthTokenResult.oauth_token_secret);
            }
        }

        // Authenticate
        public void Authenticate(string oauth_token, string oauth_token_secret)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(Authenticate_Result);
            client.OpenReadAsync(new Uri(BaseURL + "User/Authenticate.json?appId=" + AppId + "&oauth_token=" + oauth_token + "&oauth_token_secret=" + oauth_token_secret));
        }

        private void Authenticate_Result(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // TODO: 
            }
            else
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TwitterUser));
                currentUser = (TwitterUser)serializer.ReadObject(e.Result);
                AuthUpdate(this);
            }
        }

        // UpdateTwitterStatus :: TODO: 
        public void UpdateTwitterStatus(string status,double lat, double lon)
        {
            WebRequest req = WebRequest.Create(BaseURL + "User/UpdateTwitterStatus.json");
            req.Headers["TWStatus"] = status; // note, status is already encoded
            req.Headers["TWLat"] = lat.ToString(CultureInfo.InvariantCulture);
            req.Headers["TWLon"] = lon.ToString(CultureInfo.InvariantCulture);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.BeginGetRequestStream(UpdateTwitterStatusPostRequest, req);
        }

        void UpdateTwitterStatusPostRequest(IAsyncResult asyncResult)
        {
            WebRequest req = (WebRequest)asyncResult.AsyncState;
            Stream reqStream = req.EndGetRequestStream(asyncResult);
            StreamWriter writer = new StreamWriter(reqStream);

            if(req.Headers["TWStatus"] != null)
            {
                writer.Write("lat=" + req.Headers["TWLat"]);
                writer.Write("&lng=" + req.Headers["TWLon"]);
                writer.Write("&status=" + req.Headers["TWStatus"]);
                
                writer.Close();
                reqStream.Close();
                req.BeginGetResponse(OnUpdateTwitterStatusResult, req);
            }
            else
            {
                req.Abort(); // ??
            }
        }

        void OnUpdateTwitterStatusResult(IAsyncResult asyncResult)
        {
            string result = "";
            WebRequest request = (WebRequest)asyncResult.AsyncState;
            // Get the response stream.
            WebResponse response = null;

            try
            {
                response = request.EndGetResponse(asyncResult);
                Stream responseStream = response.GetResponseStream();
            }
            catch (Exception e)
            {
                result = "Error contacting service." + e.Message;
            }
            finally
            {
                if(response != null)
                    response.Close();
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return ( currentUser != null && currentUser.Id > 0 );
            }   
        }
    }
}
