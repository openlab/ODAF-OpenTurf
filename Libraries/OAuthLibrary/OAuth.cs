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
        private const string ALPHANUMERIC = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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

        public static string CreateTimestamp()
        {
            var now = DateTime.UtcNow;
            var then = new DateTime(1970, 1, 1);

            var timespan = (now - then);
            var timestamp = (long) timespan.TotalSeconds;

            return timestamp.ToString();
        }

        public static string NormalizeUrl(string url)
        {
            Uri uri;

            // only work with a valid URL
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                // only include non-standard ports
                string port = string.Empty;
                if ((uri.Scheme.Equals("http") && uri.Port != 80)
                    || (uri.Scheme.Equals("https") && uri.Port != 443)
                    || (uri.Scheme.Equals("ftp") && uri.Port != 20))
                {
                    port = ":" + uri.Port;
                }

                // use only the scheme, host, port, and path
                url = uri.Scheme + "://" + uri.Host + port + uri.AbsolutePath;
            }

            return url;
        }

        public static string CreateSignatureBase(string method, string url, NameValueCollection parameters)
        {
            List<NameValuePair> list = new List<NameValuePair>();
            foreach (string name in parameters.AllKeys)
            {
                NameValuePair item = new NameValuePair
                {
                    Name = name,
                    Value = parameters[name]
                };

                // Ensure duplicates are not included
                if (list.Contains(item))
                {
                    throw new ArgumentException("Cannot add duplicate parameters");
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

            StringBuilder sb = new StringBuilder();
            foreach (var item in list)
            {
                if (list.IndexOf(item) > 0)
                {
                    sb.Append("&");
                }

                sb.Append(string.Format("{0}={1}", item.Name, OAuthExtensions.EscapeUriDataStringRfc3986(item.Value)));
            }

            string signatureBase = string.Format("{0}&{1}&{2}",
                OAuthExtensions.EscapeUriDataStringRfc3986(method),
                OAuthExtensions.EscapeUriDataStringRfc3986(url),
                OAuthExtensions.EscapeUriDataStringRfc3986(sb.ToString()));

            return signatureBase;
        }

        public static string CreateSignature(string signatureBase, string consumerSecret, string tokenSecret)
        {
            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            // initialize the cryptography provider 
            string signingKey = string.Format("{0}&{1}", OAuthExtensions.EscapeUriDataStringRfc3986(consumerSecret), OAuthExtensions.EscapeUriDataStringRfc3986(tokenSecret));
            HMACSHA1 hasher = new HMACSHA1(Encoding.UTF8.GetBytes(signingKey));

            string signature = Convert.ToBase64String(hasher.ComputeHash(Encoding.ASCII.GetBytes(signatureBase)));

            return signature;
        }

        private static string GetAuthorizationHeader(NameValueCollection parameters)
        {
            StringBuilder sb = new StringBuilder("OAuth ");

            bool first = true;
            foreach (string name in parameters.AllKeys)
            {
                if (name.StartsWith("oauth_"))
                {
                    if (!first)
                    {
                        sb.Append(',');
                    }

                    sb.Append(string.Format("{0}={1}", name, OAuthExtensions.EscapeUriDataStringRfc3986(parameters[name])));
                    first = false;
                }
            }

            return sb.ToString();
        }

        #region Nested type: NameValuePair

        private class NameValuePair
        {
            public string Name { get; set; }
            public string Value { get; set; }

            public bool Equals(NameValuePair other)
            {
                return Equals(other.Name, Name) && Equals(other.Value, Value);
            }

            public override bool Equals(object obj)
            {
                return Equals((NameValuePair) obj);
            }
        }

        #endregion
    }
}