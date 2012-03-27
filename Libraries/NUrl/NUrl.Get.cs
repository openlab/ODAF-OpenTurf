#region Copyright

// Professional Twitter Development by Daniel Crenna (ISBN 978-0-470-53132-7)
// Copyright Wiley Publishing Inc, 2009. 
// Please refer to http://www.wrox.com/WileyCDA/Section/id-106010.html for licensing terms.

#endregion

using System;
using System.IO;
using System.Net;

namespace Wrox.Twitter.NUrl
{
    partial class NUrl
    {
        public static string Get(this string url)
        {
            var request = CreateGetRequest(url);

            return ExecuteGet(request);
        }

        public static string Get(this WebRequest request)
        {
            return ExecuteGet(request);
        }

        public static string Get(this string url,
                                 string username,
                                 string password)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreateGetRequest(url);
            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;

            return ExecuteGet(request);
        }

        public static IAsyncResult GetAsync(this string url,
                                            Action<WebResponseEventArgs>
                                                callback)
        {
            var request = CreateGetRequest(url);

            return ExecuteGetAsync(request, callback);
        }

        public static IAsyncResult GetAsync(this string url,
                                            string username,
                                            string password,
                                            Action<WebResponseEventArgs>
                                                callback)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreateGetRequest(url);
            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;

            return ExecuteGetAsync(request, callback);
        }

        public static string ExecuteGet(this WebRequest request)
        {
            Console.WriteLine("GET: {0}", request.RequestUri);

            ClearLastResponse();

            try
            {
                using (var response = request.GetResponse())
                {
                    var stream = response.GetResponseStream();

                    SetLastResponse(response);

                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                return HandleWebException(ex);
            }
        }

        public static IAsyncResult ExecuteGetAsync(this WebRequest request,
                                                   Action<WebResponseEventArgs>
                                                       callback)
        {
            Console.WriteLine("GET: {0}", request.RequestUri);

            ClearLastResponse();

            return
                request.BeginGetResponse(
                    result => RaiseWebResponse(request, result, callback), null);
        }

        private static WebRequest CreateGetRequest(string url)
        {
            url = ValidateUrl(url);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";

            return request;
        }
    }
}