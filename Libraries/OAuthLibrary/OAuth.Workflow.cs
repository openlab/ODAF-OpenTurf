#region Copyright

// Professional Twitter Development by Daniel Crenna (ISBN 978-0-470-53132-7)
// Copyright Wiley Publishing Inc, 2009. 
// Please refer to http://www.wrox.com/WileyCDA/Section/id-106010.html for licensing terms.

#endregion

using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Wrox.Twitter.NUrl;

namespace OAuthLibrary
{
    partial class OAuth
    {
        public static string GetRequestToken(
            string consumerKey,
            string consumerSecret,
            string callbackUrl)
        {
            const string method = "POST";
            const string url = "https://api.twitter.com/oauth/request_token";

            NameValueCollection parameters = new NameValueCollection
            {
                {"oauth_callback", callbackUrl},
                {"oauth_consumer_key", consumerKey},
                {"oauth_nonce", CreateNonce()},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_timestamp", CreateTimestamp()},
                {"oauth_version", "1.0"}
            };

            string signatureBase = CreateSignatureBase(method, url, parameters);
            string signature = CreateSignature(signatureBase, consumerSecret, null);

            parameters.Add("oauth_signature", signature);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Headers["Authorization"] = GetAuthorizationHeader(parameters);

            return request.Post(Encoding.ASCII.GetBytes(string.Empty));
        }

        public static string GetAccessToken(
            string consumerKey,
            string consumerSecret,
            string oauth_token,
            string oauth_verifier)
        {
            const string method = "POST";
            const string url = "https://api.twitter.com/oauth/access_token";

            NameValueCollection parameters = new NameValueCollection
            {
                {"oauth_consumer_key", consumerKey},
                {"oauth_nonce", CreateNonce()},
                {"oauth_signature_method", "HMAC-SHA1"},
                {"oauth_token", oauth_token},
                {"oauth_timestamp", CreateTimestamp()},
                {"oauth_version", "1.0"}
            };

            string signatureBase = CreateSignatureBase(method, url, parameters);
            string signature = CreateSignature(signatureBase, consumerSecret, null);

            parameters.Add("oauth_signature", signature);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Headers["Authorization"] = GetAuthorizationHeader(parameters);

            return request.Post(Encoding.ASCII.GetBytes("oauth_verifier=" + oauth_verifier));
        }

        public static string GetProtectedResource(
            string method,
            string url,
            string consumerKey,
            string consumerSecret,
            string accessToken,
            string accessTokenSecret)
        {
            Uri uri = new Uri(url);
            NameValueCollection parameters = HttpUtility.ParseQueryString(uri.Query);

            // Normalize URL
            url = NormalizeUrl(url);

            // keep a copy of the non-OAuth parameters
            NameValueCollection queryParameters = new NameValueCollection(parameters);

            parameters.Add("oauth_consumer_key", consumerKey);
            parameters.Add("oauth_nonce", CreateNonce());
            parameters.Add("oauth_signature_method", "HMAC-SHA1");
            parameters.Add("oauth_token", accessToken);
            parameters.Add("oauth_timestamp", CreateTimestamp());
            parameters.Add("oauth_version", "1.0");

            string signatureBase = CreateSignatureBase(method, url, parameters);
            string signature = CreateSignature(signatureBase, consumerSecret, accessTokenSecret);

            parameters.Add("oauth_signature", signature);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Headers["Authorization"] = GetAuthorizationHeader(parameters);

            // send the request to get back a request token
            string response = null;
            switch (method.ToUpper())
            {
                case "GET":
                    response = request.Get();
                    break;
                case "POST":
                    // collect non-OAuth parameters for the post body
                    StringBuilder sb = new StringBuilder();

                    bool first = true;
                    foreach (string name in queryParameters)
                    {
                        if (!first)
                        {
                            sb.Append("&");
                        }

                        sb.AppendFormat("{0}={1}", name, OAuthExtensions.EscapeUriDataStringRfc3986(queryParameters[name]));
                        first = false;
                    }

                    response = request.Post(Encoding.ASCII.GetBytes(sb.ToString()));
                    break;
            }

            return response;
        }
    }
}