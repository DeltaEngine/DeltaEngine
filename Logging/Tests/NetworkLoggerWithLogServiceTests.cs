using System;
using System.Threading;
using DeltaEngine.Core;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class NetworkLoggerWithLogServiceTests
	{
		//ncrunch: no coverage start
		[Test, Ignore]
		public static void LogToRealLogServer()
		{
			var ready = false;
			var connection = new OnlineServiceConnection(new MockSettings(), () => {});
			connection.ServerErrorHappened += error => { throw new ServerErrorReceived(error); };
			connection.ContentReady += () => ready = true;
			using (var logClient = new NetworkLogger(connection))
			{
				for (int timeoutMs = 1000; timeoutMs > 0 && !ready; timeoutMs -= 10)
					Thread.Sleep(10);
				logClient.Write(Logger.MessageType.Info, "Hello TestWorld from " + Environment.MachineName);
			}
		}

		public class ServerErrorReceived : Exception
		{
			public ServerErrorReceived(string error)
				: base(error) {}
		}
	}
}