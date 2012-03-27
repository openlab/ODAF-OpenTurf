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
    partial class NUrl
    {
        public static string Put(this string url)
        {
            byte[] content;
            var request = CreatePutRequest(url, out content);

            return ExecutePut(request, content);
        }

        public static string Put(this string url,
                                 string username,
                                 string password)
        {
            byte[] content;
            var request = CreateAuthPutRequest(username,
                                               password,
                                               url,
                                               out content);

            return ExecutePut(request, content);
        }

        public static IAsyncResult Put(this string url,
                                       Action<WebResponseEventArgs>
                                           callback)
        {
            byte[] content;
            var request = CreatePutRequest(url, out content);

            return ExecutePutAsync(request, content, callback);
        }

        public static IAsyncResult Put(this string url,
                                       string username,
                                       string password,
                                       Action<WebResponseEventArgs>
                                           callback)
        {
            byte[] content;
            var request = CreateAuthPutRequest(username,
                                               password,
                                               url,
                                               out content);

            return ExecutePutAsync(request, content, callback);
        }

        public static string ExecutePut(this WebRequest request, byte[] content)
        {
            Console.WriteLine("PUT: {0}", request.RequestUri);

            ClearLastResponse();

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(content, 0, content.Length);

                    using (var response = request.GetResponse())
                    {
                        using (
                            var reader =
                                new StreamReader(response.GetResponseStream()))
                        {
                            SetLastResponse(response);

                            var result = reader.ReadToEnd();
                            return result;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                return HandleWebException(ex);
            }
        }

        public static IAsyncResult ExecutePutAsync(this WebRequest request,
                                                   byte[] content,
                                                   Action
                                                       <WebResponseEventArgs>
                                                       callback)
        {
            Console.WriteLine("POST: {0}", request.RequestUri);

            ClearLastResponse();

            var state = new object[] {request, content, callback};

            return request.BeginGetRequestStream(
                BeginGetRequestStreamCompleted, state);
        }

        private static WebRequest CreatePutRequest(string url,
                                                   out byte[] content)
        {
            url = ValidateUrl(url);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "PUT";
            request.ContentType = "application/x-www-form-urlencoded";
            content = Encoding.UTF8.GetBytes(url);
            request.ContentLength = content.Length;

            return request;
        }

        private static WebRequest CreateAuthPutRequest(string username,
                                                       string password,
                                                       string url,
                                                       out byte[] content)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreatePutRequest(url, out content);

            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;
            return request;
        }
    }
}