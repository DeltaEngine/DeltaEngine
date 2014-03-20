using System;
using DeltaEngine.Editor.AppBuilder.Windows;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Windows
{
	public class WindowsAppInfoTests
	{
		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			var appInfo = new WindowsAppInfo("MockApp.zip", Guid.Empty, DateTime.Now);
			Assert.AreEqual("MockApp.zip", appInfo.FilePath);
			Assert.AreEqual("MockApp", appInfo.Name);
			Assert.AreEqual(PlatformName.Windows, appInfo.Platform);
			Assert.IsFalse(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			var appInfo = new WindowsAppInfo("MockApp.zip", Guid.Empty, DateTime.Now)
			{
				SolutionFilePath = "App.sln"
			};
			Assert.IsTrue(appInfo.IsSolutionPathAvailable);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestExtensions.GetMockAppInfo("FakeApp", PlatformName.Windows);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchAppOnDevice(null));
		}

		[Test]
		public void ItShouldAlwaysBePossibleToUseWindowsPlatform()
		{
			AppInfo app = AppBuilderTestExtensions.GetMockAppInfo("FakeApp", PlatformName.Windows);
			Assert.IsNotEmpty(app.AvailableDevices);
		}

		// ncrunch: no coverage start
		[Test, Category("Slow")]
		public void LaunchApp()
		{
			var device = new WindowsDevice();
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			app.LaunchAppOnDevice(device);
		}
		// ncrunch: no coverage end
	}
}