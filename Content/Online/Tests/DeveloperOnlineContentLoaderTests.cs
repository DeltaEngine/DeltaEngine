using System;
using System.IO;
using DeltaEngine.Mocks;
using DeltaEngine.Networking.Tcp;
using NUnit.Framework;

namespace DeltaEngine.Content.Online.Tests
{
	[Ignore]
	public class DeveloperOnlineContentLoaderTests
	{
		//ncrunch: no coverage start
		[Test]
		public void ConnectToOnlineContentServiceWithoutExistingContent()
		{
			if (Directory.Exists("Content"))
				Directory.Delete("Content", true);
			bool ready = false;
			var connection = new OnlineServiceConnection(new MockSettings(),
				() => { throw new ConnectionTimedOut(); });
			connection.ServerErrorHappened += error => { throw new ServerErrorReceived(error); };
			connection.ContentReady += () => ready = true;
			ContentLoaderResolver.CreationParameterForContentLoader = connection;
			ContentLoader.Use<DeveloperOnlineContentLoader>();
			Assert.IsTrue(ContentLoader.Exists("DeltaEngineLogo"));
			Assert.IsTrue(Directory.Exists("Content"));
			Assert.IsTrue(ready);
			ContentLoader.DisposeIfInitialized();
		}

		public class ConnectionTimedOut : Exception {}

		public class ServerErrorReceived : Exception
		{
			public ServerErrorReceived(string error)
				: base(error) {}
		}
	}
}