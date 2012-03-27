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
        public static string Post(this string url)
        {
            byte[] content;
            var request = CreatePostRequest(url, out content);

            return ExecutePost(request, content);
        }

        public static string Post(this HttpWebRequest request, byte[] content)
        {
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;

            return ExecutePost(request, content);
        }

        public static string Post(this string url,
                                  string username,
                                  string password)
        {
            byte[] content;
            var request = CreateAuthPostRequest(username,
                                                password,
                                                url,
                                                out content);

            return ExecutePost(request, content);
        }

        // todo new for chapter 8 
        public static string Post(this string url, string data)
        {
            byte[] content;
            var request = CreatePostRequest(url,
                                            out content);

            return ExecutePost(request, content);
        }

        public static IAsyncResult PostAsync(this string url,
                                             Action<WebResponseEventArgs>
                                                 callback)
        {
            byte[] content;
            var request = CreatePostRequest(url, out content);

            return ExecutePostAsync(request, content, callback);
        }

        public static IAsyncResult PostAsync(this string url,
                                             string username,
                                             string password,
                                             Action<WebResponseEventArgs>
                                                 callback)
        {
            byte[] content;
            var request = CreateAuthPostRequest(username,
                                                password,
                                                url,
                                                out content);

            return ExecutePostAsync(request, content, callback);
        }

        public static string ExecutePost(this WebRequest request, byte[] content)
        {
            Console.WriteLine("POST: {0}", request.RequestUri);

            ClearLastResponse();

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(content, 0, content.Length);

                    using (var response = request.GetResponse())
                    {
                        SetLastResponse(response);

                        using (
                            var reader =
                                new StreamReader(response.GetResponseStream()))
                        {
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

        public static IAsyncResult ExecutePostAsync(this WebRequest request,
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

        private static WebRequest CreatePostRequest(string url,
                                                    out byte[] content)
        {
            url = ValidateUrl(url);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            content = Encoding.UTF8.GetBytes(url);
            request.ContentLength = content.Length;

            return request;
        }

        // todo new for chapter 8 
        private static WebRequest CreatePostRequest(string url, string data,
                                                    out byte[] content)
        {
            url = ValidateUrl(url);

            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            content = Encoding.UTF8.GetBytes(data);
            request.ContentLength = content.Length;

            return request;
        }

        private static WebRequest CreateAuthPostRequest(string username,
                                                        string password,
                                                        string url,
                                                        out byte[] content)
        {
            var pair = String.Concat(username, ":", password);
            var token = pair.Base64Encode();

            var request = CreatePostRequest(url, out content);

            var header = String.Format("Basic {0}", token);
            request.Headers["Authorization"] = header;
            return request;
        }
    }
}