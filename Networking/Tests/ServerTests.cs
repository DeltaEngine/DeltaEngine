using DeltaEngine.Networking.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	public class ServerTests
	{
		[SetUp]
		public void SetUp()
		{
			server = new MockServer();
		}

		private MockServer server;

		[Test]
		public void ListenForClients()
		{
			server.Start(800);
			Assert.IsTrue(server.IsRunning);
		}

		[Test]
		public void InitiallyNoClients()
		{
			Assert.AreEqual(0, server.ListenPort);
			Assert.AreEqual(0, server.NumberOfConnectedClients);
		}

		[Test]
		public void ConnectClient()
		{
			bool didClientConnect = false;
			server.ClientConnected += client => didClientConnect = true;
			CreateConnectedClient();
			Assert.AreEqual(1, server.NumberOfConnectedClients);
			Assert.IsTrue(didClientConnect);
		}

		private Client CreateConnectedClient()
		{
			var client = new MockClient(server);
			client.Connect("Target", 0);
			return client;
		}

		[Test]
		public void DisconnectClient()
		{
			bool didClientDisconnect = false;
			server.ClientDisconnected += c => didClientDisconnect = true;
			var client = CreateConnectedClient();
			client.Dispose();
			Assert.IsTrue(didClientDisconnect);
		}

		[Test]
		public void DisconnectServer()
		{
			var client = CreateConnectedClient();
			server.Dispose();
			Assert.IsFalse(client.IsConnected);
		}
	}
}