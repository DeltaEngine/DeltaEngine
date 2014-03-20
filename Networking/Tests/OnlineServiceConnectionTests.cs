using System.Threading;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests
{
	[Ignore]
	public class OnlineServiceConnectionTests
	{
		//ncrunch: no coverage start
		[Test]
		public void ClientGetsConnectedAndSendsLoginRequest()
		{
			string errorReceived = "";
			bool readyReceived = false;
			var connection = new OnlineServiceConnection(new MockSettings(), () => { });
			connection.ServerErrorHappened += text => errorReceived = text;
			connection.ContentReady += () => readyReceived = true;
			connection.ConnectToService();
			Thread.Sleep(1000);
			Assert.AreEqual("", errorReceived);
			Assert.IsTrue(connection.IsLoggedIn);
			Assert.IsTrue(readyReceived);
		}

		[Test]
		public void ConnectToInvalidServerShouldTimeOut()
		{
			bool timedOut = false;
			var connection = new OnlineServiceConnection();
			connection.Connect("localhost", 12345, () => timedOut = true);
			Thread.Sleep((int)((connection.Timeout + 0.5f) * 1000));
			Assert.IsTrue(timedOut);
			Assert.IsFalse(connection.IsLoggedIn);
		}

		[Test]
		public void ReceiveResultFromServer()
		{
			var connection = new OnlineServiceConnection(new MockSettings(), () => { });
			object lastMessageReceived = null;
			string errorReceived = "";
			connection.ServerErrorHappened += text => errorReceived = text;
			connection.DataReceived += message => lastMessageReceived = message;
			Assert.IsNull(lastMessageReceived);
			connection.ConnectToService();
			Thread.Sleep(1000);
			Assert.AreEqual("", errorReceived);
			Assert.IsNotNull(lastMessageReceived);
		}
	}
}