using System;
using System.Collections.Generic;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Servers listen on a specific port and accept multiple clients.
	/// </summary>
	public abstract class Server : IDisposable
	{
		public abstract void Start(int listenPort);
		public virtual bool IsRunning { get; protected set; }
		public virtual int ListenPort { get; protected set; }
		public int NumberOfConnectedClients
		{
			get { return connectedClients.Count; }
		}

		internal protected readonly List<Client> connectedClients = new List<Client>();

		protected void RaiseClientDisconnected(Client client)
		{
			if (ClientDisconnected != null)
				ClientDisconnected(client);
		}

		public event Action<Client> ClientDisconnected;

		protected void RaiseClientConnected(Client client)
		{
			if (ClientConnected != null)
				ClientConnected(client);
		}

		public event Action<Client> ClientConnected;

		public void OnClientDataReceived(Client client, object message)
		{
			if (ClientDataReceived != null)
				ClientDataReceived(client, message);
		}

		public event Action<Client, object> ClientDataReceived;

		public virtual void Dispose()
		{
			lock (connectedClients)
			{
				var closingConnections = new List<Client>(connectedClients);
				foreach (var connection in closingConnections)
					connection.Dispose(); //ncrunch: no coverage
			}
		}
	}
}