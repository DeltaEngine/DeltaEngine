using System;
using System.Collections.Generic;

namespace DeltaEngine.Networking
{
	/// <summary>
	/// Set up or join a server, and send and receive messages across a network.
	/// Multiple sessions can be run concurrently with any combination of servers and clients
	/// </summary>
	public class Messaging : IDisposable
	{
		internal Messaging(NetworkResolver resolver)
		{
			this.resolver = resolver;
			Current = this;
		}

		private readonly NetworkResolver resolver;

		internal static Messaging Current { get; private set; }

		public static MessagingSession StartSession(int port)
		{
			var session = new ServerMessagingSession(Current.resolver.ResolveServer(), port);
			Current.sessions.Add(session);
			return session;
		}

		private readonly List<MessagingSession> sessions = new List<MessagingSession>();

		public static MessagingSession JoinSession(string host, int port)
		{
			var session = new ClientMessagingSession(Current.resolver.ResolveClient(), host, port);
			Current.sessions.Add(session);
			return session;
		}

		public void Dispose()
		{
			foreach (MessagingSession session in sessions)
				session.Dispose();
			Current = null;
		}
	}
}