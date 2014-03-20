using System;
using System.Collections.Generic;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Set up or join a server, and send and receive messages across a network
	/// </summary>
	public abstract class MessagingSession : IDisposable
	{
		public List<Message> GetMessages()
		{
			if (incomingMessages.Count == 0)
				return new List<Message>();
			List<Message> messages;
			lock (incomingMessages)
			{
				messages = new List<Message>(incomingMessages);
				incomingMessages.Clear();
			}
			return messages;
		}

		private readonly List<Message> incomingMessages = new List<Message>();

		public class Message
		{
			protected Message() {} //ncrunch: no coverage

			public Message(int senderUniqueID, object data)
			{
				SenderUniqueID = senderUniqueID;
				Data = data;
			}

			public int SenderUniqueID { get; private set; }
			public object Data { get; private set; }
		}

		protected class UniqueIDAssignment
		{
			protected UniqueIDAssignment() {} //ncrunch: no coverage

			public UniqueIDAssignment(int uniqueID)
			{
				UniqueID = uniqueID;
			}

			public readonly int UniqueID;
		}

		public abstract int UniqueID { get; }
		public const int UniqueIDUnassigned = -1;

		public abstract int NumberOfParticipants { get; }

		public abstract void SendMessage(object message);

		protected void AddToIncomingMessages(Message message)
		{
			lock (incomingMessages)
				incomingMessages.Add(message);
		}

		public void Disconnect()
		{
			Dispose();
		}

		public abstract void Dispose();
	}
}