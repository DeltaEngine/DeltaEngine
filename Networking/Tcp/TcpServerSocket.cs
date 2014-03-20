using System;
using System.Net;
using System.Net.Sockets;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// Networking over a TCP connection.
	/// </summary>
	public class TcpServerSocket : IDisposable
	{
		//ncrunch: no coverage start
		public TcpServerSocket(IPEndPoint endPoint)
		{
			this.endPoint = endPoint;
			nativeSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		}

		private readonly IPEndPoint endPoint;
		private readonly Socket nativeSocket;

		public void StartListening()
		{
			nativeSocket.Bind(endPoint);
			nativeSocket.Listen(MaxNumberOfSimultaneouslyAcceptedClients);
			IsListening = true;
			nativeSocket.BeginAccept(AcceptCallback, null);
		}

		public bool IsListening { get; private set; }

		private const int MaxNumberOfSimultaneouslyAcceptedClients = 10;

		private void AcceptCallback(IAsyncResult asyncResult)
		{
			lock (this)
			{
				if (!IsListening)
					return;
				Socket clientHandle = nativeSocket.EndAccept(asyncResult);
				var newConnectedClient = new TcpSocket(clientHandle);
				TriggerClientConnectedEvent(newConnectedClient);
				newConnectedClient.WaitForData();
				nativeSocket.BeginAccept(AcceptCallback, null);
			}
		}

		private void TriggerClientConnectedEvent(TcpSocket newConnectedClient)
		{
			if (ClientConnected != null)
				ClientConnected(newConnectedClient);
		}

		public event Action<TcpSocket> ClientConnected;

		public int ListenPort
		{
			get { return endPoint.Port; }
		}

		public void Dispose()
		{
			lock (this)
			{
				IsListening = false;
				nativeSocket.Close();
			}
		}
	}
}