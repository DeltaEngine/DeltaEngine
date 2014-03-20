using System;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Android
{
	public class AndroidAppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new AndroidAppInfo("MockApp.apk", Guid.Empty, DateTime.Now);
			Assert.AreEqual("MockApp.apk", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.Android, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new AndroidAppInfo("MockApp.zip", Guid.Empty, DateTime.Now)
			{
				SolutionFilePath = "App.sln"
			};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestExtensions.GetMockAppInfo("FakeApp", PlatformName.Android);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchAppOnDevice(null));
		}

		[Test, Category("Slow"), Timeout(16000)]
		public void LaunchApp()
		{
			AppBuilderTestExtensions.LaunchLogoAppOnPrimaryDevice(PlatformName.Android);
		}
	}
}