using System.Net;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Networking.Tests.Tcp
{
	public class NetworkExtensionsTests
	{
		[Test]
		public void TestToEndPointWithExternalAddress()
		{
			const string DeltaEngineExternalIp = "217.91.31.182";
			IPEndPoint endpointFromDomain = NetworkExtensions.ToEndPoint("deltaengine.net", ServicesPort);
			IPEndPoint endpointFromIp = NetworkExtensions.ToEndPoint(DeltaEngineExternalIp, ServicesPort);
#if DEBUG			
			const string DeltaEngineInternalIp = "192.168.0.5";
#else
			const string DeltaEngineInternalIp = "127.0.0.1";
#endif

			var validEndpoints = new[]
			{ DeltaEngineExternalIp + ":" + ServicesPort, DeltaEngineInternalIp + ":" + ServicesPort };
			Assert.Contains(endpointFromDomain.ToString(), validEndpoints);
			Assert.Contains(endpointFromIp.ToString(), validEndpoints);
		}

		private const int ServicesPort = 800;

		[Test]
		public void TestToEndPointWithLoopbackAddress()
		{
			IPEndPoint endpointFromHostname = NetworkExtensions.ToEndPoint("localhost", ServicesPort);
			IPEndPoint endpointFromIp = NetworkExtensions.ToEndPoint("127.0.0.1", ServicesPort);
			string expectedEndpoint = "127.0.0.1:" + ServicesPort;
			Assert.AreEqual(expectedEndpoint, endpointFromHostname.ToString());
			Assert.AreEqual(expectedEndpoint, endpointFromIp.ToString());
		}
	}
}