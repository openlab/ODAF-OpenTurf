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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace BitlyDotNET.Exceptions
{
	/// <summary>
	/// Defines the possible reasons for the library to throw an exception
	/// </summary>
	public enum Reason
	{
		#region Codes Definition

		/// <summary>
		/// The library was unable to initiate a call to the API due to a lack of permissions.
		/// <para>
		///		Usually, this happens when a <see href="http://msdn.microsoft.com/en-us/library/system.net.webpermission.aspx">System.Net.WebPermission</see> is thrown by the framework.
		///		You might want to add the following line in your config files to resolve this issue:
		///		<code><trust level="Medium" originUrl="https?://api\.bit\.ly/.+" /></code>
		/// </para>
		/// </summary>
		CallForbidden,
		/// <summary>
		/// The requested file/method could not be found on the remote server
		/// </summary>
		MethodNotFound,
		/// <summary>
		/// The API returned data in an unexpected format
		/// </summary>
		UnableToParseResponse

		#endregion
	}

	/// <summary>
	/// Represents an exception that can be thrown by the library when a critical
	/// condition prevents a successful communication with the API.
	/// </summary>
	[Serializable]
	public class BitlyDotNETException : Exception, ISerializable
	{
		#region Custom Exception Properties
	
		/// <summary>
		/// Gets the reason behind the creation of this exception
		/// </summary>
		public Reason Reason { get; private set; }

		#endregion

		#region Construction

		private BitlyDotNETException()
		{
		}

		public BitlyDotNETException(Reason reason)
			: base()
		{
			Reason = reason;
		}

		public BitlyDotNETException(Reason reason, string message)
			: base(message)
		{
			Reason = reason;
		}

		public BitlyDotNETException(Reason reason, string message, Exception inner)
			: base(message, inner)
		{
			Reason = reason;
		}

		#endregion

		#region ISerializable Members

		protected BitlyDotNETException(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");

			Reason = (Reason)info.GetValue("Reason", typeof(Reason));
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
				throw new System.ArgumentNullException("info");

			info.AddValue("Reason", Reason, typeof(Reason));
		}

		#endregion
	}
}
