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
        public static string Delete(this string url)
        {
            var request = CreateDeleteRequest(url);

            return ExecuteDelete(request);
        }

        public static string Delete(this string url,
                                    string username,
                                    string password)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreateDeleteRequest(url);
            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;

            return ExecuteDelete(request);
        }

        public static IAsyncResult DeleteAsync(this string url,
                                               Action<WebResponseEventArgs>
                                                   callback)
        {
            var request = CreateDeleteRequest(url);

            return ExecuteDeleteAsync(request, callback);
        }

        public static IAsyncResult DeleteAsync(this string url,
                                               string username,
                                               string password,
                                               Action<WebResponseEventArgs>
                                                   callback)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreateDeleteRequest(url);
            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;

            return ExecuteDeleteAsync(request, callback);
        }

        public static string ExecuteDelete(this WebRequest request)
        {
            Console.WriteLine("DELETE: {0}", request.RequestUri);

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

        public static IAsyncResult ExecuteDeleteAsync(this WebRequest request,
                                                      Action<WebResponseEventArgs>
                                                          callback)
        {
            Console.WriteLine("DELETE: {0}", request.RequestUri);

            ClearLastResponse();

            return
                request.BeginGetResponse(
                    result => RaiseWebResponse(request, result, callback), null);
        }

        private static WebRequest CreateDeleteRequest(string url)
        {
            url = ValidateUrl(url);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "DELETE";

            return request;
        }
    }
}