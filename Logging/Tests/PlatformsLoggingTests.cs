using System;
using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Logging.Tests
{
	public class NetworkLoggingTests
	{
		[Test]
		public void NetworkLoggingTest()
		{
			using (new ConsoleLogger())
			{
				Logger.Info("Testing network logging info");
				Logger.Warning("Testing network logging warning");
				Assert.Throws<TestingNetworkError>(() => { throw new TestingNetworkError(); });
			}
		}

		private class TestingNetworkError : Exception
		{
			public TestingNetworkError()
				: base("Testing network logging error")
			{
				Logger.Error(this);
			}
		}
	}
}