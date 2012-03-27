#region Copyright

// Professional Twitter Development by Daniel Crenna (ISBN 978-0-470-53132-7)
// Copyright Wiley Publishing Inc, 2009. 
// Please refer to http://www.wrox.com/WileyCDA/Section/id-106010.html for licensing terms.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace OAuthLibrary
{
    public static partial class OAuth
    {
        private const string ALPHANUMERIC =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string CreateNonce()
        {
            var sb = new StringBuilder();
            var random = new Random();

            for (var i = 0; i <= 16; i++)
            {
                var index = random.Next(ALPHANUMERIC.Length);
                sb.Append(ALPHANUMERIC[index]);
            }

            return sb.ToString();
        }

        public static long CreateTimestamp()
        {
            var now = DateTime.UtcNow;
            var then = new DateTime(1970, 1, 1);

            var timespan = (now - then);
            var timestamp = (long) timespan.TotalSeconds;

            return timestamp;
        }

        public static string NormalizeUrl(string url)
        {
            Uri uri;

            // only work with a valid URL
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                // only include non-standard ports
                var port = "";
                if (uri.Scheme.Equals("http") && uri.Port != 80 ||
                    uri.Scheme.Equals("https") && uri.Port != 443 ||
                    uri.Scheme.Equals("ftp") && uri.Port != 20)
                {
                    port = ":" + uri.Port;
                }

                // use only the scheme, host, port, and path
                url = uri.Scheme + "://" + uri.Host + port + uri.AbsolutePath;
            }

            return url;
        }


        public static string NormalizeRequestParameters
            (NameValueCollection parameters)
        {
            var sb = new StringBuilder();

            var list = new List<NameValuePair>();
            foreach (var name in parameters.AllKeys)
            {
                var value = OAuthExtensions.EscapeUriDataStringRfc3986(parameters[name]);
                var item = new NameValuePair {Name = name, Value = value};

                // Ensure duplicates are not included
                if (list.Contains(item))
                {
                    throw new ArgumentException(
                        "Cannot add duplicate parameters");
                }

                list.Add(item);
            }

            list.Sort((left, right) =>
                          {
                              if (left.Name.Equals(right.Name))
                              {
                                  return left.Value.CompareTo(right.Value);
                              }

                              return left.Name.CompareTo(right.Name);
                          });

            foreach (var item in list)
            {
                sb.Append(item.Name + "=" + item.Value);
                if (list.IndexOf(item) < list.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        public static string ConcatenateRequestElements(string method,
                                                        string url,
                                                        string parameters)
        {
            // URL encode base elements
            url = OAuthExtensions.EscapeUriDataStringRfc3986(url);
            parameters = OAuthExtensions.EscapeUriDataStringRfc3986(parameters);

            // build signature base according to spec
            var sb = new StringBuilder();
            sb.Append(method.ToUpper()).Append("&");
            sb.Append(url).Append("&");
            sb.Append(parameters);

            return sb.ToString();
        }

        public static string CreateSignature(string signatureBase,
                                             string consumerSecret,
                                             string tokenSecret)
        {
            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            // URL encode key elements TODO why?
            consumerSecret = OAuthExtensions.EscapeUriDataStringRfc3986(consumerSecret);
            tokenSecret = OAuthExtensions.EscapeUriDataStringRfc3986(tokenSecret);

            // initialize the cryptography provider 
            var key = String.Concat(consumerSecret, "&", tokenSecret);
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var signatureMethod = new HMACSHA1(keyBytes);

            // create a signature with the base and provider
            var data = Encoding.ASCII.GetBytes(signatureBase);
            var hash = signatureMethod.ComputeHash(data);
            var signature = Convert.ToBase64String(hash);

            // You must encode the URI for safe net travel
            signature = OAuthExtensions.EscapeUriDataStringRfc3986(signature);
            return signature;
        }

        #region Nested type: NameValuePair

        private class NameValuePair
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public bool Equals(NameValuePair other)
            {
                return Equals(other.Name, Name) &&
                       Equals(other.Value, Value);
            }

            public override bool Equals(object obj)
            {
                return Equals((NameValuePair) obj);
            }
        }

        #endregion
    }
}