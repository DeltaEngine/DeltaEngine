using DeltaEngine.Content;
using DeltaEngine.Content.Disk;
using DeltaEngine.Content.Online;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class AppRunnerTests
	{
		[SetUp]
		public void CreateResolver()
		{
			resolver = new SimpleResolver();
		}

		private SimpleResolver resolver;

		[TearDown]
		public void DisposeResolver()
		{
			resolver.Dispose();
		}

		[Test]
		public void CheckDefaultContentLoader()
		{
			CreateResolver();
			Assert.AreEqual(typeof(DeveloperOnlineContentLoader), ContentLoader.Type);
		}

		private class SimpleResolver : AppRunner
		{
			public SimpleResolver()
			{
				RegisterCommonEngineSingletons();
			}

			protected override void RegisterPhysics() {}
		}

		[Test]
		public void CheckExplicitlySetContentLoader()
		{
			ContentLoader.Use<DiskContentLoader>();
			CreateResolver();
			Assert.AreEqual(typeof(DiskContentLoader), ContentLoader.Type);
			ContentLoader.DisposeIfInitialized();
		}
	}
}