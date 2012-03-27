#region Copyright

// Professional Twitter Development by Daniel Crenna (ISBN 978-0-470-53132-7)
// Copyright Wiley Publishing Inc, 2009. 
// Please refer to http://www.wrox.com/WileyCDA/Section/id-106010.html for licensing terms.

#endregion

using System;
using System.Net;

namespace Wrox.Twitter.NUrl
{
    partial class NUrl
    {
        public static void Main(string[] args)
        {
            var input = false;
            if (args.Length == 0)
            {
                input = true;
                PrintUsage();
                Console.Write("Enter a new query: ");
                var line = Console.ReadLine();
                if (line != null && line.Trim().Length > 0)
                {
                    args = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                }
            }

            var valid = true;
            if (args.Length == 0 || args.Length > 3)
            {
                valid = false;
            }

            if (!valid)
            {
                PrintUsage();
            }
            else
            {
                var method = args[0].ToUpper().Trim();

                switch (method)
                {
                    case "GET":
                        ExecuteMethod(args, input, "GET");
                        break;
                    case "POST":
                        ExecuteMethod(args, input, "POST");
                        break;
                    case "PUT":
                        ExecuteMethod(args, input, "PUT");
                        break;
                    case "DELETE":
                        ExecuteMethod(args, input, "DELETE");
                        break;
                    default:
                        Console.WriteLine("Invalid HTTP method provided.");
                        break;
                }
            }
        }

        private static void ExecuteMethod(string[] args, bool input, string method)
        {
            if (args.Length == 3)
            {
                var uri = args[2];
                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                {
                    Console.WriteLine("Malformed URI provided. URIs must be valid and absolute.");
                }

                var pair = args[1].Split(':');

                switch (method)
                {
                    case "GET":
                        Console.WriteLine(uri.Get(pair[0], pair[1]));
                        break;
                    case "POST":
                        Console.WriteLine(uri.Post(pair[0], pair[1]));
                        break;
                    case "PUT":
                        Console.WriteLine(uri.Put(pair[0], pair[1]));
                        break;
                    case "DELETE":
                        Console.WriteLine(uri.Delete(pair[0], pair[1]));
                        break;
                }

                if (input)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to quit.");
                    Console.ReadLine();
                }
            }
            else
            {
                var uri = args[1];
                if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                {
                    Console.WriteLine("Malformed URI provided. URIs must be valid and absolute.");
                }
                else
                {
                    switch (method)
                    {
                        case "GET":
                            Console.WriteLine(uri.Get());
                            break;
                        case "POST":
                            Console.WriteLine(uri.Post());
                            break;
                        case "PUT":
                            Console.WriteLine(uri.Put());
                            break;
                        case "DELETE":
                            Console.WriteLine(uri.Delete());
                            break;
                    }
                }

                if (input)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to quit.");
                    Console.ReadLine();
                }
            }
        }

        public static void PrintUsage()
        {
            Console.WriteLine("NUrl - (c) 2009 Wrox");
            Console.WriteLine("Usage: [http-method] (username:password) [uri]");
        }

        private static void SetLastResponse(WebResponse response)
        {
            if (response is HttpWebResponse)
            {
                var httpResponse = (HttpWebResponse) response;
                LastResponseStatusCode = httpResponse.StatusCode;
                LastResponseStatusDescription = httpResponse.StatusDescription;
                LastResponse = httpResponse;
            }
        }

        private static void ClearLastResponse()
        {
            LastResponseStatusCode = null;
            LastResponseStatusDescription = null;
        }
    }
}