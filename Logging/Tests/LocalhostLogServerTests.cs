using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Category("Slow")]
	class LocalhostLogServerTests
	{
		[SetUp]
		public void InitializeServer()
		{
			mockServer = new MockServer();
			server = new LocalhostLogServer(mockServer);
			mockClient = new MockClient(mockServer);
			mockClient.Connect("localhost", 1);
			server.Start();
		}

		[Test]
		public void SendTextMesssageToServer()
		{
			mockClient.Send(new TextMessage("Hi"));
			server.Dispose();
		}

		[Test]
		public void SendLoginRequestToServer()
		{
			mockClient.Send(new LoginRequest("Key", "ProjectName"));
			server.Dispose();
		}

		[Test]
		public void SendLogInfoMessageToServer()
		{
			mockClient.Send(new LogInfoMessage("Log"));
			server.Dispose();
		}

		private LocalhostLogServer server;
		private MockServer mockServer;
		private MockClient mockClient;
	}
}