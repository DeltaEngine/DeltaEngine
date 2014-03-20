using System;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.WindowsPhone7
{
	[Ignore]
	public class WP7AppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new WP7AppInfo("MockApp.zip", Guid.Empty, DateTime.Now);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.WindowsPhone7, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new WP7AppInfo("MockApp.zip", Guid.Empty, DateTime.Now)
			{
				SolutionFilePath = "App.sln"
			};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestExtensions.GetMockAppInfo("FakeApp",
				PlatformName.WindowsPhone7);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchAppOnDevice(null));
		}

		[Test, Category("Slow"), Timeout(16000)]
		public void LaunchApp()
		{
			AppBuilderTestExtensions.LaunchLogoAppOnEmulatorDevice(PlatformName.WindowsPhone7);
		}
	}
}