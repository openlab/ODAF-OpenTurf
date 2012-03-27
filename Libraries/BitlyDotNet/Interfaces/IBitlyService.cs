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

namespace BitlyDotNET.Interfaces
{
	/// <summary>
	/// Defines the possible return codes when calling the various methods in the library
	/// </summary>
	public enum StatusCode
	{
		#region Codes Definition

		/// <summary>
		/// The request was successful.
		/// </summary>
		OK = 0,
		/// <summary>
		/// %s.
		/// </summary>
		Unspecified = 1,
		/// <summary>
		/// An unknown error occured.
		/// </summary>
		UnknownError = 101,
		/// <summary>
		/// Missing parameter %s.
		/// </summary>
		MissingParameter = 201,
		/// <summary>
		/// Undefined method %s.
		/// </summary>
		UndefinedMethod = 202,
		/// <summary>
		/// You must be authenticated %s.
		/// </summary>
		NotAuthenticated = 203,
		/// <summary>
		/// You are already logged in.
		/// </summary>
		AlreadyLoggedIn = 204,
		/// <summary>
		/// You tried to login with an invalid username/password.
		/// </summary>
		InvalidCredentials = 205,
		/// <summary>
		/// You specified an invalid version number. Valid versions are [2.0.0, 2.0.1].
		/// </summary>
		InvalidVersion = 206,
		/// <summary>
		/// There was a problem posting your request. Please try again.
		/// </summary>
		PostError = 207,
		/// <summary>
		/// That page does not exist.
		/// </summary>
		PageNotFound = 404,
		/// <summary>
		/// Service unavailable.
		/// </summary>
		ServiceUnavailable = 503,
		/// <summary>
		/// Invalid email address.
		/// </summary>
		InvalidEmail = 1101,
		/// <summary>
		/// Invalid username.
		/// </summary>
		InvalidUsername = 1102,
		/// <summary>
		/// Username not available.
		/// </summary>
		UsernameNotAvailable = 1103,
		/// <summary>
		/// Email not available.
		/// </summary>
		EmailNotAvailable = 1104,
		/// <summary>
		/// Password must be 6-20 characters long.
		/// </summary>
		InvalidPasswordLength = 1105,
		/// <summary>
		/// Could not fetch bitly json doc from s3.
		/// </summary>
		CouldNotFetchJSON = 1201,
		/// <summary>
		/// No info available for requested document.
		/// </summary>
		InfoNotFoundForDocument = 1202,
		/// <summary>
		/// No info available for requested document.
		/// </summary>
		InfoNotFoundForDocument2 = 1203,
		/// <summary>
		/// Not a valid bitly hash.
		/// </summary>
		InvalidBitlyHash = 1204,
		/// <summary>
		/// Traffic lookup for that hash failed.
		/// </summary>
		TrafficLookupHashFailed = 1205,
		/// <summary>
		/// URL you tried to shorten was already a short bit.ly URL or was invalid.
		/// </summary>
		InvalidURLOrAlreadyShort = 1206,
		/// <summary>
		/// That CNAME is already associated with another account.
		/// </summary>
		CNAMEAlreadyAssociated = 1207,
		/// <summary>
		/// That CNAME is invalid.
		/// </summary>
		CNAMEIsInvalid = 1208,
		/// <summary>
		/// You do not have access to this CNAME version of bitly.
		/// </summary>
		NoAccessToCNAMEVersion = 1209,
		/// <summary>
		/// You are trying to access a CNAMED bit.ly site. Please login.
		/// </summary>
		CNAMENotAuthenticated = 1210,
		/// <summary>
		/// You made a batch request and an error occurred with one part of your request.
		/// </summary>
		ErrorInBatch = 1211,
		/// <summary>
		/// The custom keyword you tried to use was already used by someone else.
		/// </summary>
		KeywordInUse = 1212,
		/// <summary>
		/// We could not find a long URL for that short URL.
		/// </summary>
		NoMatchingLongURL = 1213,
		/// <summary>
		/// Invalid Twitter credentials.
		/// </summary>
		InvalidTwitterCredentials = 1301,
		/// <summary>
		/// Missing Twitter credentials.
		/// </summary>
		MissingTwitterCredentials = 1302,
		/// <summary>
		/// Error updating Twitter status.
		/// </summary>
		ErrorUpdatingTwitterStatus = 1303,
		/// <summary>
		/// Text must be less than or equal to 140 characters.
		/// </summary>
		TextToLong = 1304,
		/// <summary>
		/// The Twitter password you saved on bitly is no longer valid for your Twitter account.
		/// </summary>
		TwitterPasswordMismatchOnBitly = 1305,
		/// <summary>
		/// Twitter is currently unavailable.
		/// </summary>
		TwitterUnavailable = 1306,
		/// <summary>
		/// Duplicate linked accounts.
		/// </summary>
		DuplicateLinkedAccounts = 1401,
		/// <summary>
		/// Invalid account type.
		/// </summary>
		InvalidAccountType = 1402
		
		#endregion
	}

	/// <summary>
	/// Defines the interface for communicating with the bit.ly API
	/// </summary>
	public interface IBitlyService
	{
		#region Interface Definition

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
		StatusCode Shorten(string url, out string shortened);

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
		string Shorten(string url);

		#endregion
	}
}
