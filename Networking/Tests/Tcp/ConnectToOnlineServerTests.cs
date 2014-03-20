using System;
using System.Threading;
using DeltaEngine.Datatypes;
using DeltaEngine.Networking.Messages;
using DeltaEngine.Networking.Tcp;
using Microsoft.Win32;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests.Tcp
{
	[Ignore]
	public class ConnectToOnlineServerTests
	{
		//ncrunch: no coverage start
		[SetUp]
		public void CreateConnectionAndWaitUntilConnected()
		{
			ConnectToServer();
			WaitUntilConnected();
			connection.DataReceived += delegate(object response)
			{
				serverResponse = response;
			};
		}

		private void ConnectToServer()
		{
			connection = new TcpSocket();
			connection.Connect(ServerAddress, ServerPort);
		}

		private Client connection;
		private const string ServerAddress = "deltaengine.net";
		private const int ServerPort = 800;

		private void WaitUntilConnected()
		{
			int timeoutMs = 2000;
			while (!connection.IsConnected && timeoutMs > 0)
			{
				Thread.Sleep(10);
				timeoutMs -= 10;
			}
			if (timeoutMs <= 0)
				throw new Exception("Unable to connect to " + connection.TargetAddress);
		}

		private object serverResponse;

		[TearDown]
		public void DisposeConnection()
		{
			connection.Dispose();
		}

		[Test]
		public void ShouldBeConnected()
		{
			Assert.IsTrue(connection.IsConnected);
		}

		[Test]
		public void SendingNullMessagesIsNotAllowed()
		{
			Assert.Throws<ArgumentNullException>(() => SendMessageAndWaitForServerResponse(null));
		}

		[Test]
		public void SendMessageWithoutLoginCausesServerError()
		{
			SendMessageAndWaitForServerResponse(new LogInfoMessage("Hi"));
			Assert.IsInstanceOf<ServerError>(serverResponse);
		}

		private void SendMessageAndWaitForServerResponse(object message)
		{
			connection.Send(message);
			int timeoutMs = 2000;
			while (serverResponse == null && timeoutMs > 0)
			{
				Thread.Sleep(10);
				timeoutMs -= 10;
			}
			if (timeoutMs <= 0)
				throw new Exception("Server is not responding to "+message);
		}

		[Test]
		public void LoginWithoutApiKeyShouldFail()
		{
			SendMessageAndWaitForServerResponse(new LoginRequest("", ""));
			Assert.IsInstanceOf<ServerError>(serverResponse);
		}

		[Test]
		public void SendInvalidMessageShouldBeRejectedByServer()
		{
			Login();
			SendMessageAndWaitForServerResponse(new Vector2D(0, 1));
			Thread.Sleep(50);
			Assert.IsInstanceOf<SetProject>(serverResponse);
		}

		private void Login()
		{
			SendMessageAndWaitForServerResponse(new LoginRequest(GetDeveloperApiKeyFromRegistry(),
				"LogoApp"));
			Assert.IsInstanceOf<LoginSuccessful>(serverResponse);
		}

		private static string GetDeveloperApiKeyFromRegistry()
		{
			var registryKey = Registry.CurrentUser.OpenSubKey(RegistryPathForApiKey, false);
			if (registryKey != null)
				return (string)registryKey.GetValue("ApiKey");
			return "";
		}

		private const string RegistryPathForApiKey = @"Software\DeltaEngine\Editor";

		[Test]
		public void SendLogMessageAfterLoginDoesNotCauseServerError()
		{
			Login();
			connection.Send(new LogInfoMessage("Hi Server"));
			Assert.IsNotInstanceOf<ServerError>(serverResponse);
		}
	}
}