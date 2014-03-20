using System;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	[Category("Slow")]
	public class NetworkLoggerTests
	{
		//ncrunch: no coverage start
		[TestFixtureSetUp]
		public void StartLogServer()
		{
			server = new LocalhostLogServer(new TcpServer());
			server.Start();
			new MockSettings();
			var ready = false;
			var connection = new OnlineServiceConnection();
			connection.DataReceived += o => ready = true;
			connection.Connect("localhost", LocalhostLogServer.Port);
			connection.Send(new LoginRequest("", "DeltaEngine.Logging.Tests"));
			logger = new NetworkLogger(connection);
			for (int timeoutMs = 1000; timeoutMs > 0 && !ready; timeoutMs -= 10)
				Thread.Sleep(10);
			Assert.IsTrue(ready);
		}

		private LocalhostLogServer server;
		private NetworkLogger logger;

		[TestFixtureTearDown]
		public void ShutdownLogServer()
		{
			logger.Dispose();
			server.Dispose();
		}

		[Test]
		public void LogInfoMessage()
		{
			logger.Write(Logger.MessageType.Info, "Hello");
			ExpectThatServerHasReceivedMessage("Hello");
		}

		private void ExpectThatServerHasReceivedMessage(string messageText)
		{
			Assert.That(() => server.LastMessage.Text, Is.EqualTo(messageText).After(100, 5));
		}

		[Test]
		public void LogWarning()
		{
			logger.Write(Logger.MessageType.Warning, "Ohoh");
			ExpectThatServerHasReceivedMessage("Ohoh");
			logger.Write(Logger.MessageType.Warning, new NullReferenceException().ToString());
			ExpectThatServerLastMessageContains("NullReferenceException");
		}

		private void ExpectThatServerLastMessageContains(string messageText)
		{
			Assert.That(() => server.LastMessage.Text.Contains(messageText),
				Is.EqualTo(true).After(100, 5));
		}

		[Test]
		public void LogError()
		{
			logger.Write(Logger.MessageType.Error, new ArgumentException().ToString());
			Thread.Sleep(100);
			ExpectThatServerLastMessageContains("ArgumentException");
		}

		[Test]
		public void NotLoggingIfSettingFalse()
		{
			var logRecieved = false;
			Settings.Current.UseOnlineLogging = false;
			server.Server.ClientDataReceived += (client, o) => logRecieved = true;
			logger.Write(Logger.MessageType.Error, new ArgumentException().ToString());
			Assert.IsFalse(logRecieved);
			Settings.Current.UseOnlineLogging = true;
		}
	}
}