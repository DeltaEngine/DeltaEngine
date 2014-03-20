using System;
using System.IO;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppInfoExtensionsTests
	{
		[Test]
		public void CreateAppInfoForUnsupportedPlatformWillStillReturnAnAppInfo()
		{
			Assert.IsNotNull(AppInfoExtensions.CreateAppInfo("UnknownApp", (PlatformName)99, Guid.Empty,
				DateTime.Now));
		}

		[Test]
		public void CreateAppInfoForWindows()
		{
			const string AppName = "WindowsApp";
			AppInfo appInfo = AppInfoExtensions.CreateAppInfo(AppName, PlatformName.Windows,
				Guid.NewGuid(), DateTime.Now);
			Assert.AreEqual(AppName, appInfo.Name);
			Assert.AreEqual(PlatformName.Windows, appInfo.Platform);
			Assert.AreNotEqual(Guid.Empty, appInfo.AppGuid);
		}

		[Test]
		public void CreateAppInfoFromBuildResult()
		{
			const string AppName = "MockApp";
			var buildResult = new AppBuildResult(AppName, PlatformName.Windows)
			{
				PackageFileName = AppName + ".app",
				PackageGuid = Guid.NewGuid(),
			};
			const string AppDirectory = "DirectoryForApps";
			AppInfo appInfo = AppInfoExtensions.CreateAppInfo(Path.Combine(AppDirectory, buildResult.PackageFileName),
				buildResult.Platform, buildResult.PackageGuid, DateTime.Now);
			Assert.AreEqual(buildResult.ProjectName, appInfo.Name);
			Assert.AreEqual(buildResult.Platform, appInfo.Platform);
			Assert.AreEqual(Path.Combine(AppDirectory, buildResult.PackageFileName), appInfo.FilePath);
			Assert.AreEqual(buildResult.PackageGuid, appInfo.AppGuid);
		}

		[Test]
		public void GetFullAppNameForAndroidApp()
		{
			var androidApp = new AndroidAppInfo(@"C:\Fake\MockApp.apk", Guid.Empty, DateTime.Now);
			string fullAppName = androidApp.GetFullAppNameForEngineApp();
			Assert.IsTrue(fullAppName.Contains("DeltaEngine"), fullAppName);
		}

		// ncrunch: no coverage start
		[Test, Ignore]
		public void GetFullAppNameForNonAndroidApp()
		{
			var otherPlatfromApp = new WP7AppInfo(@"C:\Fake\MockApp.xap", Guid.Empty, DateTime.Now);
			string fullAppName = otherPlatfromApp.GetFullAppNameForEngineApp();
			Assert.IsFalse(fullAppName.Contains("DeltaEngine"), fullAppName);
		}
	}
}