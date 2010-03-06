﻿using System;
using System.IO;
using System.Text;

namespace Enyim.Caching.Memcached.Operations.Text
{
	internal static class TextSocketHelper
	{
		private const string GenericErrorResponse = "ERROR";
		private const string ClientErrorResponse = "CLIENT_ERROR ";
		private const string ServerErrorResponse = "SERVER_ERROR ";
		private const int ErrorResponseLength = 13;

		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(TextSocketHelper));

		/// <summary>
		/// Reads the response of the server.
		/// </summary>
		/// <returns>The data sent by the memcached server.</returns>
		/// <exception cref="T:System.InvalidOperationException">The server did not sent a response or an empty line was returned.</exception>
		/// <exception cref="T:Enyim.Caching.Memcached.MemcachedException">The server did not specified any reason just returned the string ERROR. - or - The server returned a SERVER_ERROR, in this case the Message of the exception is the message returned by the server.</exception>
		/// <exception cref="T:Enyim.Caching.Memcached.MemcachedClientException">The server did not recognize the request sent by the client. The Message of the exception is the message returned by the server.</exception>
		public static string ReadResponse(PooledSocket socket)
		{
			string response = TextSocketHelper.ReadLine(socket);

			if (log.IsDebugEnabled)
				log.Debug("Received response: " + response);

			if (String.IsNullOrEmpty(response))
				throw new MemcachedClientException("Empty response received.");

			if (String.Compare(response, GenericErrorResponse, StringComparison.Ordinal) == 0)
				throw new NotSupportedException("Operation is not supported by the server or the request was malformed. If the latter please report the bug to the developers.");

			if (response.Length >= ErrorResponseLength)
			{
				if (String.Compare(response, 0, ClientErrorResponse, 0, ErrorResponseLength, StringComparison.Ordinal) == 0)
				{
					throw new MemcachedClientException(response.Remove(0, ErrorResponseLength));
				}
				else if (String.Compare(response, 0, ServerErrorResponse, 0, ErrorResponseLength, StringComparison.Ordinal) == 0)
				{
					throw new MemcachedException(response.Remove(0, ErrorResponseLength));
				}
			}

			return response;
		}


		/// <summary>
		/// Reads a line from the socket. A line is terninated by \r\n.
		/// </summary>
		/// <returns></returns>
		private static string ReadLine(PooledSocket socket)
		{
			MemoryStream ms = new MemoryStream(50);

			bool gotR = false;
			byte[] buffer = new byte[1];

			int data;

			while (true)
			{
				data = socket.ReadByte();

				if (data == 13)
				{
					gotR = true;
					continue;
				}

				if (gotR)
				{
					if (data == 10)
						break;

					ms.WriteByte(13);

					gotR = false;
				}

				ms.WriteByte((byte)data);
			}

			string retval = Encoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length);

			if (log.IsDebugEnabled)
				log.Debug("ReadLine: " + retval);

			return retval;
		}

		/// <summary>
		/// Sends the command to the server. The trailing \r\n is automatically appended.
		/// </summary>
		/// <param name="value">The command to be sent to the server.</param>
		public static void SendCommand(PooledSocket socket, string value)
		{
			if (log.IsDebugEnabled)
				log.Debug("SendCommand: " + value);

			// send the whole command with only one Write
			// since Nagle is disabled on the socket this is more efficient than
			// Write(command), Write("\r\n")
			socket.Write(TextSocketHelper.GetCommandBuffer(value));
		}

		/// <summary>
		/// Gets the bytes representing the specified command. returned buffer can be used to streamline multiple writes into one Write on the Socket
		/// using the <see cref="M:Enyim.Caching.Memcached.PooledSocket.Write(IList&lt;ArraySegment&lt;byte&gt;&gt;)"/>
		/// </summary>
		/// <param name="value">The command to be converted.</param>
		/// <returns>The buffer containing the bytes representing the command. The returned buffer will be terminated with 13, 10 (\r\n)</returns>
		/// <remarks>The Nagle algorithm is disabled on the socket to speed things up, so it's recommended to convert a command into a buffer
		/// and use the <see cref="M:Enyim.Caching.Memcached.PooledSocket.Write(IList&lt;ArraySegment&lt;byte&gt;&gt;)"/> to send the command and the additional buffers in one transaction.</remarks>
		public unsafe static ArraySegment<byte> GetCommandBuffer(string value)
		{
			int valueLength = value.Length;
			byte[] data = new byte[valueLength + 2];

			fixed (byte* buffer = data)
			fixed (char* chars = value)
			{
				Encoding.ASCII.GetBytes(chars, 0, buffer, valueLength);

				buffer[valueLength] = 13;
				buffer[valueLength + 1] = 10;
			}

			return new ArraySegment<byte>(data);
		}

	}
}

#region [ License information          ]
/* ************************************************************
 *
 * Copyright (c) Attila Kiskó, enyim.com
 *
 * This source code is subject to terms and conditions of 
 * Microsoft Permissive License (Ms-PL).
 * 
 * A copy of the license can be found in the License.html
 * file at the root of this distribution. If you can not 
 * locate the License, please send an email to a@enyim.com
 * 
 * By using this source code in any fashion, you are 
 * agreeing to be bound by the terms of the Microsoft 
 * Permissive License.
 *
 * You must not remove this notice, or any other, from this
 * software.
 *
 * ************************************************************/
#endregion