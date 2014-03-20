using System;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.AppBuilder.Web;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests.Web
{
	public class WebAppInfoTests
	{
		[SetUp]
		public void CreateMockApp()
		{
			mockApp = new WebAppInfo("MockApp.html", Guid.Empty, DateTime.Now);
		}

		private WebAppInfo mockApp;

		[Test]
		public void CheckValuesOfSimpleAppInfo()
		{
			Assert.AreEqual("MockApp.html", mockApp.FilePath);
			Assert.AreEqual("MockApp", mockApp.Name);
			Assert.AreEqual(PlatformName.Web, mockApp.Platform);
			Assert.IsFalse(mockApp.IsSolutionPathAvailable);
		}

		[Test]
		public void CheckRebuildableApp()
		{
			mockApp.SolutionFilePath = "App.sln";
			Assert.IsTrue(mockApp.IsSolutionPathAvailable);
		}

		[Test]
		public void ItShouldAlwaysBePossibleToUseWeb()
		{
			Assert.IsNotEmpty(mockApp.AvailableDevices);
		}

		[Test]
		public void LaunchAppWithoutDeviceThrowsExcetpion()
		{
			AppInfo app = AppBuilderTestExtensions.GetMockAppInfo("FakeApp", PlatformName.Web);
			Assert.Throws<AppInfo.NoDeviceSpecified>(() => app.LaunchAppOnDevice(null));
		}
	}
}
