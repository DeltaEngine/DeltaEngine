using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Extensions;
using DeltaEngine.Networking.Messages;
using Ionic.Zlib;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// Socket for a network connection.
	/// </summary>
	public class TcpSocket : Client
	{
		//ncrunch: no coverage start
		public TcpSocket()
			: this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {}

		public TcpSocket(Socket nativeSocket)
		{
			this.nativeSocket = nativeSocket;
			buffer = new byte[ReceiveBufferSize];
			dataCollector = new DataCollector();
			dataCollector.ObjectFinished += OnObjectFinished;
			isDisposed = false;
			Timeout = DefaultTimeout;
		}

		protected readonly Socket nativeSocket;
		private readonly byte[] buffer;
		/// <summary>
		/// The .NET default receive buffer size is between 8kb and 64kb (W8 uses 64kb). Use it all.
		/// </summary>
		internal const int ReceiveBufferSize = 65536;
		private readonly DataCollector dataCollector;
		private bool isDisposed;
		public float Timeout { get; private set; }
		private const float DefaultTimeout = 5.0f;

		private void OnObjectFinished(MessageData dataContainer)
		{
			var dataBytes = dataContainer.IsDataCompressed
				? ZlibStream.UncompressBuffer(dataContainer.Data) : dataContainer.Data;
			using (var dataStream = new MemoryStream(dataBytes))
			using (var dataReader = new BinaryReader(dataStream))
			{
				object receivedMessage;
				try
				{
					receivedMessage = TryReceiveMessage(dataReader);
				}
				catch (Exception ex)
				{
					receivedMessage =
						new ServerError(StackTraceExtensions.FormatExceptionIntoClickableMultilineText(ex));
				}
				if (DataReceived != null)
					DataReceived(receivedMessage);
				else
					throw new NobodyIsUsingTheDataReceivedEvent(receivedMessage);
			}
		}

		private static object TryReceiveMessage(BinaryReader dataReader)
		{
			return dataReader.Create();
		}

		public event Action<object> DataReceived;

		private class NobodyIsUsingTheDataReceivedEvent : Exception
		{
			public NobodyIsUsingTheDataReceivedEvent(object receivedMessage)
				: base(receivedMessage.ToString()) {}
		}

		public void Connect(string serverAddress, int serverPort, Action optionalTimedOut = null)
		{
			if (nativeSocket.Connected || nativeSocket.IsBound || nativeSocket.RemoteEndPoint != null)
				throw new UnableToConnectSocketIsAlreadyInUse();
			if (optionalTimedOut != null)
				TimedOut = optionalTimedOut;
			connectionTargetAddress = serverAddress + ":" + serverPort;
			try
			{
				TryConnect(NetworkExtensions.ToEndPoint(serverAddress, serverPort));
			}
			catch (SocketException)
			{
				Logger.Warning("An error has occurred when trying to request a connection " +
					"to the server (" + connectionTargetAddress + ")");
				if (TimedOut != null)
					TimedOut();
				Dispose();
			}
		}

		public class UnableToConnectSocketIsAlreadyInUse : Exception {}

		protected event Action TimedOut;
		private string connectionTargetAddress;

		private void TryConnect(EndPoint targetAddress)
		{
			var socketArgs = new SocketAsyncEventArgs { RemoteEndPoint = targetAddress };
			socketArgs.Completed += SocketConnectionComplete;
			nativeSocket.ConnectAsync(socketArgs);
			if (TimedOut != null)
				ThreadExtensions.Start(() =>
				{
					if(waitForTimeoutHandle.WaitOne((int)(Timeout * 1000)))
						return;
					if (IsConnected || isDisposed)
						return;
					TimedOut();
					Dispose();
				});
		}

		private readonly AutoResetEvent waitForTimeoutHandle = new AutoResetEvent(false);

		private void SocketConnectionComplete(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
		{
			lock (syncObject)
			{
				if (socketAsyncEventArgs.SocketError != SocketError.Success)
					return;
				WaitForData();
				if (Connected != null)
					Connected();
				if (IsConnected)
					TrySendAllMessagesInTheQueue();
			}
		}

		public event Action Connected;
		
		/// <summary>
		/// Send any object to the receiving side. When byte data is big enough and compression is
		/// allowed (not already compressed file data) the data will be send compressed via Zip.
		/// </summary>
		public void Send(object data, bool allowToCompressMessage = true)
		{
			try
			{
				TrySendOrEnqueueData(data, allowToCompressMessage);
			}
			catch (SocketException)
			{
				Dispose();
			}
		}

		private void TrySendOrEnqueueData(object data, bool allowToCompressMessage)
		{
			lock (syncObject)
			{
				if (IsConnected)
					SendDataThroughNativeSocket(data, allowToCompressMessage);
				else
					messages.Enqueue(data);
			}
		}

		private readonly Queue<object> messages = new Queue<object>();
		private readonly Object syncObject = new Object();

		private void SendDataThroughNativeSocket(object message, bool allowToCompressMessage)
		{
			if (nativeSocket == null || isDisposed)
				throw new SocketException();
			var byteData = CreateByteDataWithCompressionIfPossible(message, allowToCompressMessage);
			int numberOfSendBytes = nativeSocket.Send(byteData);
			if (numberOfSendBytes == 0)
				throw new SocketException();
			if (numberOfSendBytes != byteData.Length)
				Logger.Warning("Failed to send message " + message + ", numberOfSendBytes=" +
					numberOfSendBytes + ", messageLength=" + byteData.Length);
		}

		private static byte[] CreateByteDataWithCompressionIfPossible(object message,
			bool allowToCompressMessage)
		{
			var messageData = BinaryDataExtensions.ToByteArrayWithTypeInformation(message);
			if (allowToCompressMessage && messageData.Length > MinimumByteDataLengthToZip)
			{
				byte[] compressedData = ZlibStream.CompressBuffer(messageData);
				if (compressedData.Length < messageData.Length)
					return ToByteArrayWithLengthHeader(compressedData, true);
			}
			return ToByteArrayWithLengthHeader(messageData, false);
		}

		public static byte[] ToByteArrayWithLengthHeader(byte[] messageData, bool dataIsCompressed)
		{
			using (var total = new MemoryStream())
			using (var writer = new BinaryWriter(total))
			{
				writer.WriteNumberMostlyBelow255(messageData.Length);
				writer.Write(dataIsCompressed);
				writer.Write(messageData);
				return total.ToArray();
			}
		}

		/// <summary>
		/// The amount of bytes where the packed data start to be smaller than the original one is
		/// around 500 bytes, but there is also overhead in compressing and decompressing data, so it
		/// only makes sense for at least 200kb of data (e.g. sending big network files).
		/// </summary>
		public const int MinimumByteDataLengthToZip = 1024 * 200;

		private void TrySendAllMessagesInTheQueue()
		{
			try
			{
				TrySendDataThroughNativeSocket();
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to send all messages in queue: " + ex.Message);
			}
		}

		private void TrySendDataThroughNativeSocket()
		{
			while (messages.Count > 0)
				SendDataThroughNativeSocket(messages.Dequeue(), true);
		}

		public void WaitForData()
		{
			if (isDisposed)
				return;
			try
			{
				TryBeginReceive();
			}
			catch (SocketException)
			{
				Logger.Warning("Server Error: occurred when setting the socket to receive data");
			}
		}

		private void TryBeginReceive()
		{
			nativeSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivingBytes, null);
		}

		private void ReceivingBytes(IAsyncResult asyncResult)
		{
			if (isDisposed)
				return;
			try
			{
				TryReceiveBytes(asyncResult);
			}
			catch (SocketException)
			{
				Dispose();
			}
			catch (ObjectDisposedException)
			{
				Dispose();
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		private void TryReceiveBytes(IAsyncResult asyncResult)
		{
			int numberOfReceivedBytes = nativeSocket.EndReceive(asyncResult);
			if (numberOfReceivedBytes == 0)
				throw new SocketException();
			dataCollector.ReadBytes(buffer, 0, numberOfReceivedBytes);
			WaitForData();
		}

		public string TargetAddress
		{
			get { return IsConnected ? nativeSocket.RemoteEndPoint.ToString() : connectionTargetAddress; }
		}

		public bool IsConnected
		{
			get { return nativeSocket != null && nativeSocket.Connected; }
		}

		public virtual void Dispose()
		{
			if (isDisposed)
				return;
			isDisposed = true;
			nativeSocket.Close();
			waitForTimeoutHandle.Set();
			if (Disconnected != null)
				Disconnected();
		}

		public event Action Disconnected;

		public int UniqueID { get; set; }
	}
}