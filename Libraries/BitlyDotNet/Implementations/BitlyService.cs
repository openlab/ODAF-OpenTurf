/*
 * BitlyDotNet
 * 
 * Copyright (c) 2009 Mike Gleason jr Couturier
 * (http://blog.mikecouturier.com/search/label/bitly-dot-net)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to
 * permit persons to whom the Software is furnished to do so, subject to
 * the following conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */
using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using BitlyDotNET.Exceptions;
using BitlyDotNET.Interfaces;

namespace BitlyDotNET.Implementations
{
	/// <summary>
	/// Implements the IBitlyService interface
	/// </summary>
	public class BitlyService : IBitlyService
	{
		#region Properties

		/// <summary>
		/// Gets the Login name used to communicate with the bit.y API.
		/// </summary>
		public string Login { get; private set; }

		/// <summary>
		/// Gets the API Key used to communicate with the bit.y API.
		/// </summary>
		public string ApiKey { get; private set; }

		/// <summary>
		/// Gets the API Version used by the library to communicate with the bit.y API.
		/// </summary>
		public string ApiVersion { get { return "2.0.1"; } }

		/// <summary>
		/// Gets the base URL of the bit.ly REST API
		/// </summary>
        //private string ApiRestUrl { get { return "http://api.bit.ly"; } }
        private string ApiRestUrl { get { return "http://api.j.mp"; } }

		#endregion

		#region Construction

		/// <summary>
		/// Hides default constructor to force the use of the overload
		/// </summary>
		private BitlyService()
		{
		}

		/// <summary>
		/// Constructs the service specifying the necessary credentials to
		/// communicate with the bit.ly API.
		/// </summary>
		/// <exception cref="ArgumentNullException">
		///		<paramref name="login"/> is <see langword="null">null</see>.
		///		<para>-or-</para>
		///		<paramref name="apikey"/> is <see langword="null">null</see>.
		///	</exception>
		/// <param name="login">Your bit.ly account name</param>
		/// <param name="apikey">Your bit.ly API key</param>
		public BitlyService(string login, string apikey)
		{
			if (login == null)
				throw new ArgumentNullException("login");

			if (apikey == null)
				throw new ArgumentNullException("apikey");

			Login = login;
			ApiKey = apikey;
		}

		#endregion

		#region IBitlyService Members

		/// <summary>
		/// Encodes a long URL as a shorter one, put it in <paramref name="shortened"/> and returns the <see cref="StatusCode">StatusCode</see> of the request.
		/// </summary>
		/// <param name="url">A long URL to shorten</param>
		/// <param name="shortened">Contains a long URL if successful, <see langword="null">null</see> otherwise.</param>
		/// <returns>The <see cref="StatusCode">status code</see> of the request</returns>
		/// <exception cref="ArgumentNullException"><paramref name="url"/> is <see langword="null">null</see>.</exception>
		/// <exception cref="ArgumentException"><paramref name="url"/> is not a well formed URL (see remarks).</exception>
		/// <exception cref="BitlyDotNET.Exceptions.BitlyDotNETException">A critical error occured that prevented the function to succesfully contact the Bitly API (see remarks).</exception>
		/// <remarks>
		/// <para>
		///		When an exception of type <see cref="BitlyDotNET.Exceptions.BitlyDotNETException">BitlyDotNETException</see> is thrown, you can examine the <see cref="BitlyDotNET.Exceptions.Reason">Reason</see> member of the exception to further diagnose the problem.
		/// </para>
		/// <para>
		///		The parameter <paramref name="url"/> shouldn't be URL-escaped as a whole, but any parameters in the query string should be.
		/// </para>
		/// </remarks>
		public StatusCode Shorten(string url, out string shortened)
		{
			if (url == null)
				throw new ArgumentNullException("url");

			if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
				throw new ArgumentException("Invalid absolute URL", "url");

			string method = "/shorten?longUrl={0}&login={1}&apiKey={2}&version={3}&format=xml&history=1";
			XDocument response = null;
			XElement errorCode, shortUrl = null;
			shortened = null;

			try
			{
				// Load the XML document from the REST URL
				response = XDocument.Load(ApiRestUrl + String.Format(method, HttpUtility.UrlEncode(url), HttpUtility.UrlEncode(Login), HttpUtility.UrlEncode(ApiKey), ApiVersion));
			}
			catch (SecurityException e)
			{
				throw new BitlyDotNETException(Reason.CallForbidden, "The local reader does not have sufficient permissions to access the location of the data.", e);
			}
			catch (FileNotFoundException e)
			{
				throw new BitlyDotNETException(Reason.MethodNotFound, "The file identified by the url does not exist.", e);
			}
			catch (XmlException e)
			{
				throw new BitlyDotNETException(Reason.UnableToParseResponse, "An error occurred while parsing the response.", e);
			}

			// Try to retreive the error code from the XML response
			errorCode = response.Descendants("errorCode").SingleOrDefault();
			if (errorCode == null || !Enum.IsDefined(typeof(StatusCode), (int)errorCode))
				throw new BitlyDotNETException(Reason.UnableToParseResponse, "Unable to extract \"errorCode\"");

			// If the call was unsuccessful, look no further
			if ((StatusCode)(int)errorCode != StatusCode.OK)
				return (StatusCode)(int)errorCode;

			// Try to retreive the short URL generated for us
			shortUrl = response.Descendants("shortUrl").SingleOrDefault();
			if (shortUrl == null || String.IsNullOrEmpty((string)shortUrl))
				throw new BitlyDotNETException(Reason.UnableToParseResponse, "Unable to extract \"shortUrl\"");

			// Return the URL and OK
			shortened = shortUrl.Value;
			return StatusCode.OK;
		}

		/// <summary>
		/// Encodes a long URL as a shorter one and returns it.
		/// </summary>
		/// <param name="url">A long URL to shorten</param>
		/// <returns>The short URL corresponding to <paramref name="url"/> if successful, <see langword="null">null</see> otherwise.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="url"/> is <see langword="null">null</see>.</exception>
		/// <exception cref="ArgumentException"><paramref name="url"/> is not a well formed URL (see remarks).</exception>
		/// <exception cref="BitlyDotNET.Exceptions.BitlyDotNETException">A critical error occured that prevented the function to succesfully contact the Bitly API (see remarks).</exception>
		/// <remarks>
		/// <para>
		///		When an exception of type <see cref="BitlyDotNET.Exceptions.BitlyDotNETException">BitlyDotNETException</see> is thrown, you can examine the <see cref="BitlyDotNET.Exceptions.Reason">Reason</see> member of the exception to further diagnose the problem.
		/// </para>
		/// <para>
		///		The parameter <paramref name="url"/> shouldn't be URL-escaped as a whole, but any parameters in the query string should be.
		/// </para>
		/// </remarks>
		public string Shorten(string url)
		{ 
			string shortened;

			if (Shorten(url, out shortened) == StatusCode.OK)
				return shortened;

			return null;
		}
	
		#endregion
	}
}
