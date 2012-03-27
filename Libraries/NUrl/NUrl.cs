#region Copyright

// Professional Twitter Development by Daniel Crenna (ISBN 978-0-470-53132-7)
// Copyright Wiley Publishing Inc, 2009. 
// Please refer to http://www.wrox.com/WileyCDA/Section/id-106010.html for licensing terms.

#endregion

using System;
using System.IO;
using System.Net;
using System.Text;

namespace Wrox.Twitter.NUrl
{
    public static partial class NUrl
    {
        static NUrl()
        {
            // Configure use for the Twitter API
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.UseNagleAlgorithm = false;
        }

        public static HttpStatusCode? LastResponseStatusCode { get; private set; }

        public static string LastResponseStatusDescription { get; private set; }

        public static HttpWebResponse LastResponse { get; private set; }

        public static string ValidateUrl(this string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                throw new ArgumentException("No URL provided", "url");
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                return url;
            }

            Uri uri;
            Uri.TryCreate(url, UriKind.Absolute, out uri);

            if (uri != null)
            {
                return url;
            }

            throw new ArgumentException("Malformed URL provided", "url");
        }

        private static string HandleWebException(WebException ex)
        {
            if (ex.Response is HttpWebResponse &&
                ex.Response != null)
            {
                SetLastResponse(ex.Response);

                using (
                    var reader =
                        new StreamReader(ex.Response.GetResponseStream()))
                {
                    var result = reader.ReadToEnd();
                    return result;
                }
            }

            throw ex;
        }

        public static string Base64Encode(this string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(data);
        }

        public static string Base64Decode(this string input)
        {
            var data = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(data);
        }

        private static void RaiseWebResponse(WebRequest request,
                                             IAsyncResult result,
                                             Action<WebResponseEventArgs>
                                                 callback)
        {
            var args = new WebResponseEventArgs
                           {Uri = request.RequestUri};
            try
            {
                var response = request.EndGetResponse(result);

                SetLastResponse(response);

                using (
                    var reader =
                        new StreamReader(response.GetResponseStream()))
                {
                    args.Response = reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                args.Response = HandleWebException(ex);
            }

            callback.Invoke(args);
        }

        private static void BeginGetRequestStreamCompleted(
            IAsyncResult requestResult)
        {
            var state = (object[]) requestResult.AsyncState;
            var request = (WebRequest) state[0];
            var content = (byte[]) state[1];
            var callback =
                (Action<WebResponseEventArgs>) state[2];

            using (var stream = request.EndGetRequestStream(requestResult))
            {
                stream.Write(content, 0, content.Length);

                request.BeginGetResponse(
                    result => RaiseWebResponse(request, result, callback), null);
            }
        }
    }
}