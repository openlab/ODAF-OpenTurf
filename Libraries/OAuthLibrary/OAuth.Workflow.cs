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
        public static string GetRequestToken(string url,
                                             string consumerKey,
                                             string consumerSecret)
        {
            // get any parameters in the request body 
            // to use in the OAuth signature
            var uri = new Uri(url);
            var parameters = HttpUtility.ParseQueryString(uri.Query);

            // collect the required OAuth signature data to make a request
            var oauthParameters = GetOAuthParameters(parameters,
                                                     url,
                                                     "GET",
                                                     consumerKey,
                                                     consumerSecret);

            // create a new request with OAuth authorization set
            var request = BuildOAuthWebRequest(oauthParameters, url, null);

            // send the request to get back a request token
            var token = request.Get();
            return token;
        }

        private static HttpWebRequest BuildOAuthWebRequest(NameValueCollection oauthParameters, string url, string realm)
        {
            var header = new StringBuilder();
            header.Append("OAuth ");

            if (!string.IsNullOrEmpty(realm))
            {
                // add realm info if provided
                header.Append("realm=\"" + realm + "\" ");
            }

            for (var i = 0; i < oauthParameters.Count; i++)
            {
                var key = oauthParameters.GetKey(i);
                var pair = key + "=\"" + oauthParameters[key] + "\"";

                header.Append(pair);
                if (i < oauthParameters.Count - 1)
                {
                    header.Append(",");
                }
            }

            // create a new request and set the OAuth header
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Headers["Authorization"] = header.ToString();

            return request;
        }

        private static NameValueCollection GetOAuthParameters(
            NameValueCollection requestParameters,
            string url,
            string httpMethod,
            string consumerKey,
            string consumerSecret,
            string requestToken,
            string requestTokenSecret)
        {
            if (requestParameters == null)
            {
                requestParameters = new NameValueCollection();
            }

            var timestamp = CreateTimestamp().ToString();
            var nonce = CreateNonce();

            // create oauth requestParameters
            var oauthParameters = new NameValueCollection
                                      {
                                          {"oauth_timestamp", timestamp},
                                          {"oauth_nonce", nonce},
                                          {"oauth_version", "1.0"},
                                          {"oauth_signature_method", "HMAC-SHA1"},
                                          {"oauth_consumer_key", consumerKey}
                                      };

            // add the request token if found
            if (!String.IsNullOrEmpty(requestToken))
            {
                oauthParameters.Add("oauth_token", requestToken);
            }

            // fold oauth into any existing request request parameters
            foreach (var oauthKey in oauthParameters.AllKeys)
            {
                requestParameters.Add(oauthKey, oauthParameters[oauthKey]);
            }

            // prepare a signature base
            url = NormalizeUrl(url);
            var normalizedParameters =
                NormalizeRequestParameters(requestParameters);
            var signatureBase = ConcatenateRequestElements(httpMethod,
                                                           url,
                                                           normalizedParameters);

            // obtain a signature and add it to header requestParameters
            var signature = CreateSignature(signatureBase, consumerSecret, requestTokenSecret);
            oauthParameters.Add("oauth_signature", signature);

            return oauthParameters;
        }

        private static NameValueCollection GetOAuthParameters(
            NameValueCollection requestParameters,
            string url,
            string httpMethod,
            string consumerKey,
            string consumerSecret)
        {
            // the original request is now a method overload
            return GetOAuthParameters(requestParameters,
                                      url,
                                      httpMethod,
                                      consumerKey,
                                      consumerSecret,
                                      null,
                                      null);
        }

        public static string GetAccessToken(string url,
                                            string consumerKey,
                                            string consumerSecret,
                                            string requestToken,
                                            string requestTokenSecret)
        {
            // get any parameters in the request body 
            // to use in the OAuth signature
            var uri = new Uri(url);
            var parameters = HttpUtility.ParseQueryString(uri.Query);

            // collect the required OAuth signature data to make a request
            var oauthParameters = GetOAuthParameters(parameters,
                                                     url,
                                                     "GET",
                                                     consumerKey,
                                                     consumerSecret,
                                                     requestToken,
                                                     requestTokenSecret);

            // create a new request with OAuth authorization set
            var request = BuildOAuthWebRequest(oauthParameters, url, null);

            // send the request to get back a request token
            var token = request.Get();
            return token;
        }

        public static string GetProtectedResource(string url,
                                                  string httpMethod,
                                                  string consumerKey,
                                                  string consumerSecret,
                                                  string accesssToken,
                                                  string accessTokenSecret)
        {
            // get any parameters in the request body 
            // to use in the OAuth signature
            var uri = new Uri(url);
            var parameters = HttpUtility.ParseQueryString(uri.Query);

            // keep a copy of the non-OAuth parameters
            var queryParameters = new NameValueCollection(parameters);

            // collect the required OAuth signature data to make a request
            var oauthParameters = GetOAuthParameters(parameters,
                                                     url,
                                                     httpMethod,
                                                     consumerKey,
                                                     consumerSecret,
                                                     accesssToken,
                                                     accessTokenSecret);

            // if posting, rebuild the URI without the query 
            if (httpMethod.ToUpper().Equals("POST"))
            {
                url = String.Concat(uri.Scheme,
                                    "://",
                                    uri.Authority,
                                    uri.AbsolutePath);
            }

            // create a new request with OAuth authorization set
            var request = BuildOAuthWebRequest(oauthParameters, url, null);

            // send the request to get back a request token
            string response = null;
            switch (httpMethod.ToUpper())
            {
                case "GET":
                    response = request.Get();
                    break;
                case "POST":
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";

                    // collect non-OAuth parameters for the post body
                    var sb = new StringBuilder();
                    for (var i = 0;
                         i < queryParameters.AllKeys.Length;
                         i++)
                    {
                        var key = queryParameters.AllKeys[i];
                        sb.AppendFormat("{0}={1}",
                                        OAuthExtensions.EscapeUriDataStringRfc3986(key),
                                        OAuthExtensions.EscapeUriDataStringRfc3986(
                                            queryParameters[key]));

                        if (i < queryParameters.Count - 1)
                        {
                            sb.Append("&");
                        }
                    }

                    var body = sb.ToString();
                    var content = Encoding.ASCII.GetBytes(body);
                    response = request.Post(content);
                    break;
            }

            return response;
        }
    }
}