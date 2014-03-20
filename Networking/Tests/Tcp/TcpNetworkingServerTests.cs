using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests.Tcp
{
	[Ignore]
	public class TcpNetworkingServerTests
	{
		//ncrunch: no coverage start
		[Test]
		public void IsRunning()
		{
			var tcpServer = new TcpServer();
			tcpServer.Start(Port);
			Assert.IsTrue(tcpServer.IsRunning);
			tcpServer.Dispose();
		}

		private const int Port = 1;
	}
}
