using System.Collections.Generic;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Mocks;
using NUnit.Framework;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class BuiltAppsListViewModelTests
	{
		[SetUp]
		public void CreateAppsListViewModel()
		{
			editorSettings = new MockSettings();
			appListViewModel = new BuiltAppsListViewModel(editorSettings);
		}

		private MockSettings editorSettings;
		private BuiltAppsListViewModel appListViewModel;

		[Test]
		public void CheckFormattedTextOfBuiltApps()
		{
			string formattedText = appListViewModel.TextOfBuiltApps;
			Assert.IsTrue(formattedText.Contains(appListViewModel.NumberOfBuiltApps.ToString()));
		}

		[Test]
		public void AddInvalidBuiltAppShouldThrowException()
		{
			Assert.Throws<BuiltAppsListViewModel.NoAppInfoSpecified>(() => appListViewModel.AddApp(null));
		}

		[Test]
		public void AddValidBuiltApp()
		{
			int builtApps = 0;
			appListViewModel.NumberOfBuiltAppsChanged +=
				() => builtApps = appListViewModel.NumberOfBuiltApps;
			appListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("AppNotSavedOnDisk",
				PlatformName.Windows));
			Assert.AreEqual(builtApps, appListViewModel.NumberOfBuiltApps);
		}

		[Test]
		public void SaveAndLoadBuiltApps()
		{
			AddMockAppInfoAndSaveWithDummyData("TestApp");
			Assert.AreNotEqual(0, appListViewModel.NumberOfBuiltApps);
			var loadedAppList = new BuiltAppsListViewModel(editorSettings);
			Assert.AreEqual(loadedAppList.NumberOfBuiltApps, appListViewModel.NumberOfBuiltApps);
			AssertBuiltApps(loadedAppList.BuiltApps, appListViewModel.BuiltApps);
		}

		private void AddMockAppInfoAndSaveWithDummyData(string appName)
		{
			string folder = appListViewModel.AppStorageDirectory;
			appListViewModel.AddApp(
				AppBuilderTestExtensions.GetMockAppInfo(appName, PlatformName.Windows, folder),
				new byte[] { 1 });
		}

		private static void AssertBuiltApps(IList<AppInfo> savedApps, IList<AppInfo> loadedApps)
		{
			Assert.AreEqual(savedApps.Count, loadedApps.Count);
			for (int i = 0; i < loadedApps.Count; i++)
				AssertBuiltApp(savedApps[i], loadedApps[i]);
		}

		private static void AssertBuiltApp(AppInfo expectedApp, AppInfo actualApp)
		{
			Assert.AreEqual(expectedApp.Name, actualApp.Name);
			Assert.AreEqual(expectedApp.Platform, actualApp.Platform);
			Assert.AreEqual(expectedApp.AppGuid, actualApp.AppGuid);
			Assert.AreEqual(expectedApp.FilePath, actualApp.FilePath);
		}

		[Test]
		public void RebuildEventShouldGetRaisedWhenRebuildWasRequested()
		{
			AppInfo anyApp = AppBuilderTestExtensions.GetMockAppInfo("TestApp", PlatformName.Windows);
			appListViewModel.RebuildRequest += (appInfo) => AssertBuiltApp(anyApp, appInfo);
			appListViewModel.RequestRebuild(anyApp);
		}
	}
}