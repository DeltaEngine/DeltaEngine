namespace DeltaEngine.Networking
{
	internal class ClientMessagingSession : MessagingSession
	{
		internal ClientMessagingSession(Client connection, string host, int port)
		{
			this.connection = connection;
			connection.UniqueID = UniqueIDUnassigned;
			connection.DataReceived += DataReceived;
			connection.Connect(host, port);
		}

		private Client connection;

		private void DataReceived(object data)
		{
			var uniqueIDAssignment = data as UniqueIDAssignment;
			if (uniqueIDAssignment != null)
				connection.UniqueID = uniqueIDAssignment.UniqueID;
			else
				AddToIncomingMessages(data as Message);
		}

		public override void SendMessage(object message)
		{
			connection.Send(new Message(connection.UniqueID, message));
		}

		public override int UniqueID
		{
			get { return connection.UniqueID; }
		}

		public override int NumberOfParticipants
		{
			get { return NumberOfParticipantsNotCalculated; }
		}

		internal const int NumberOfParticipantsNotCalculated = -1;

		public override void Dispose()
		{
			if (connection != null)
				connection.Dispose();
			connection = null;
		}
	}
}