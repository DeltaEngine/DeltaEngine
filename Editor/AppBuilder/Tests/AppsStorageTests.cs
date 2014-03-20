using DeltaEngine.Editor.Messages;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppsStorageTests
	{
		[SetUp]
		public void CreateEmptySettings()
		{
			settings = new MockSettings();
		}

		private MockSettings settings;

		[Test]
		public void IfSettingsDoesNotContainStorageDataItWillBeCreatedAutomatically()
		{
			var appsStorage = new AppsStorage(settings);
			Assert.IsEmpty(appsStorage.AvailableApps);
		}

		[Test]
		public void LoadStorageFromSettings()
		{
			var appsStorage = new AppsStorage(settings);
			appsStorage.AddApp(GetMockAppInfo("EmptyApp"));
			Assert.AreEqual(1, appsStorage.AvailableApps.Length);
			var loadedAppsStorage = new AppsStorage(settings);
			Assert.AreEqual(appsStorage.AvailableApps.Length, loadedAppsStorage.AvailableApps.Length);
		}

		private static AppInfo GetMockAppInfo(string appName,
			PlatformName platform = PlatformName.Windows)
		{
			return AppBuilderTestExtensions.GetMockAppInfo(appName, platform);
		}

		[Test]
		public void SavingAnAppSecondTimeShouldJustUpdateItInStorage()
		{
			var appsStorage = new AppsStorage(settings);
			const string AppName = "MyCoolApp";
			appsStorage.AddApp(GetMockAppInfo(AppName));
			appsStorage.AddApp(GetMockAppInfo(AppName));
			Assert.AreEqual(1, appsStorage.AvailableApps.Length);
			var loadedAppsStorage = new AppsStorage(settings);
			Assert.AreEqual(appsStorage.AvailableApps.Length, loadedAppsStorage.AvailableApps.Length);
		}

		[Test]
		public void SavingAnAppMustBePlatformDependent()
		{
			var appsStorage = new AppsStorage(settings);
			const string AppName = "MyCoolApp";
			appsStorage.AddApp(GetMockAppInfo(AppName, PlatformName.Android));
			Assert.AreEqual(1, appsStorage.AvailableApps.Length);
			appsStorage.AddApp(GetMockAppInfo(AppName, PlatformName.Web));
			Assert.AreEqual(2, appsStorage.AvailableApps.Length);
		}
	}
}