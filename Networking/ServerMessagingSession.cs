using System.Linq;

namespace DeltaEngine.Networking
{
	internal class ServerMessagingSession : MessagingSession
	{
		internal ServerMessagingSession(Server server, int port)
		{
			this.server = server;
			server.Start(port);
			server.ClientConnected += ClientConnected;
			server.ClientDataReceived += DataReceived;
		}

		private Server server;

		private void ClientConnected(Client client)
		{
			client.UniqueID = ++lastAssignedUniqueID;
			client.Send(new UniqueIDAssignment(client.UniqueID));
		}

		private int lastAssignedUniqueID = ServerUniqueID;
		private const int ServerUniqueID = 0;

		private void DataReceived(Client client, object data)
		{
			var message = data as Message;
			AddToIncomingMessages(message);
			EchoToOtherClients(client, message);
		}

		private void EchoToOtherClients(Client sender, Message message)
		{
			lock (server.connectedClients)
				foreach (Client client in server.connectedClients.Where(c => c != sender))
					client.Send(message);
		}

		public override void SendMessage(object message)
		{
			lock (server.connectedClients)
				foreach (Client client in server.connectedClients)
					client.Send(new Message(ServerUniqueID, message));
		}

		public override int UniqueID
		{
			get { return ServerUniqueID; }
		}

		public override int NumberOfParticipants
		{
			get { return server.connectedClients.Count; }
		}

		public override void Dispose()
		{
			if (server != null)
				server.Dispose();
			server = null;
		}
	}
}