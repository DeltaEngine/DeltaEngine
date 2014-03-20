using System.Net;

namespace DeltaEngine.Networking.Tcp
{
	/// <summary>
	/// TCP server using raw sockets via TcpServerSocket and a list of TcpSockets for the clients
	/// </summary>
	public sealed class TcpServer : Server
	{
		//ncrunch: no coverage start
		public override void Start(int listenPort)
		{
			socket = new TcpServerSocket(new IPEndPoint(IPAddress.Any, listenPort));
			SetUpSocketBindingAndRegisterCallback();
		}

		private TcpServerSocket socket;

		private void SetUpSocketBindingAndRegisterCallback()
		{
			socket.ClientConnected += OnClientConnected;
			socket.StartListening();
		}

		private void OnClientConnected(Client client)
		{
			client.Disconnected += () => OnClientDisconnected(client);
			client.DataReceived += message => OnClientDataReceived(client, message);
			lock (connectedClients)
				connectedClients.Add(client);
			RaiseClientConnected(client);
		}

		private void OnClientDisconnected(Client client)
		{
			lock (connectedClients)
				connectedClients.Remove(client);
			RaiseClientDisconnected(client);
		}

		public override bool IsRunning
		{
			get { return socket.IsListening; }
		}

		public override int ListenPort
		{
			get { return socket.ListenPort; }
		}

		public override void Dispose()
		{
			socket.Dispose();
			base.Dispose();
		}
	}
}