using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	public class AppsStorage
	{
		public AppsStorage(Settings settings)
		{
			this.settings = settings;
			availableApps = new List<AppInfo>();
			LoadAlreadyBuiltApps();
		}

		private readonly Settings settings;
		private readonly List<AppInfo> availableApps;

		private void LoadAlreadyBuiltApps()
		{
			storageData = settings.GetValue(XmlNodeNameOfStorageData,
				new XmlData(XmlNodeNameOfStorageData));
			if (String.IsNullOrEmpty(storageData.GetAttributeValue("StoragePath")))
				CreateNewBuiltAppsListData();
			StorageDirectory = storageData.GetAttributeValue("StoragePath");
			foreach (XmlData appInfoData in storageData.Children)
				LoadAppFromStorageData(appInfoData);
		}

		private const string XmlNodeNameOfStorageData = "BuiltApps";
		private XmlData storageData;

		private void CreateNewBuiltAppsListData()
		{
			storageData.AddAttribute("StoragePath",
				Path.Combine(Settings.GetMyDocumentsAppFolder(), storageData.Name));
			UpdateStorageDataInSettings();
		}

		private void UpdateStorageDataInSettings()
		{
			settings.SetValue(storageData.Name, storageData);
		}

		public string StorageDirectory { get; private set; }

		private void LoadAppFromStorageData(XmlData appInfoData)
		{
			try
			{
				TryLoadAppFromStorageData(appInfoData);
			}
			// ncrunch: no coverage start
			catch (Exception ex)
			{
				Logger.Warning("Unable to load '" + appInfoData + "' as app :" + Environment.NewLine + ex);
			}
			// ncrunch: no coverage end
		}

		private void TryLoadAppFromStorageData(XmlData appInfoData)
		{
			AppInfo app = AppInfoExtensions.CreateAppInfo(GetAppPackageFilePath(appInfoData),
				GetAppPlatform(appInfoData), GetAppGuid(appInfoData), GetAppBuildData(appInfoData));
			app.SolutionFilePath = GetAppSolutionFilePath(appInfoData);
			availableApps.Add(app);
		}

		private string GetAppPackageFilePath(XmlData appInfoData)
		{
			string fileName = appInfoData.GetAttributeValue(XmlAttributeNameOfFileName);
			if (String.IsNullOrEmpty(fileName))
				throw new AppInfoDataMissing(XmlAttributeNameOfFileName); // ncrunch: no coverage
			return Path.Combine(StorageDirectory, fileName);
		}

		private const string XmlAttributeNameOfFileName = "FileName";

		// ncrunch: no coverage start
		private class AppInfoDataMissing : Exception
		{
			public AppInfoDataMissing(string xmlAttributeName)
				: base(xmlAttributeName) { }
		}
		// ncrunch: no coverage end

		private static PlatformName GetAppPlatform(XmlData appInfoData)
		{
			const PlatformName NoPlatform = (PlatformName)0;
			PlatformName platform = appInfoData.GetAttributeValue(XmlAttributeNameOfPlatform, NoPlatform);
			if (platform == NoPlatform)
				throw new AppInfoDataMissing(XmlAttributeNameOfPlatform); // ncrunch: no coverage
			return platform;
		}

		private const string XmlAttributeNameOfPlatform = "Platform";

		private static Guid GetAppGuid(XmlData appInfoData)
		{
			string appGuidString = appInfoData.GetAttributeValue(XmlAttributeNameOfAppGuid);
			if (String.IsNullOrEmpty(appGuidString))
				throw new AppInfoDataMissing(XmlAttributeNameOfAppGuid); // ncrunch: no coverage
			return new Guid(appGuidString);
		}

		private const string XmlAttributeNameOfAppGuid = "AppGuid";

		private static DateTime GetAppBuildData(XmlData appInfoData)
		{
			var noBuildDate = DateTime.MinValue;
			DateTime buildDate = appInfoData.GetAttributeValue(XmlAttributeNameOfBuildDate, noBuildDate);
			if (buildDate == noBuildDate)
				throw new AppInfoDataMissing(XmlAttributeNameOfBuildDate); // ncrunch: no coverage
			return buildDate;
		}

		private const string XmlAttributeNameOfBuildDate = "BuildDate";

		private static string GetAppSolutionFilePath(XmlData appInfoData)
		{
			string solutionFilePath = appInfoData.GetAttributeValue(XmlAttributeNameOfSolutionFilePath);
			if (String.IsNullOrEmpty(solutionFilePath))
				throw new AppInfoDataMissing(XmlAttributeNameOfSolutionFilePath); // ncrunch: no coverage
			return solutionFilePath;
		}

		private const string XmlAttributeNameOfSolutionFilePath = "SolutionFilePath";

		public AppInfo[] AvailableApps
		{
			get { return availableApps.ToArray(); }
		}

		public void AddApp(AppInfo app)
		{
			int indexOfApp = GetIndexOfAppInAvailableAppsList(app);
			if (indexOfApp != InvalidIndex)
			{
				availableApps[indexOfApp] = app;
				UpdateStorageDataInSettings();
			}
			else
			{
				availableApps.Add(app);
				AddAppToStorageData(app);
			}
		}

		private int GetIndexOfAppInAvailableAppsList(AppInfo app)
		{
			return availableApps.FindIndex(a => a.Name == app.Name && a.Platform == app.Platform);
		}

		private const int InvalidIndex = -1;

		private void AddAppToStorageData(AppInfo app)
		{
			var appDataNode = new XmlData("App");
			appDataNode.AddAttribute(XmlAttributeNameOfFileName, Path.GetFileName(app.FilePath));
			appDataNode.AddAttribute(XmlAttributeNameOfPlatform, app.Platform);
			appDataNode.AddAttribute(XmlAttributeNameOfAppGuid, app.AppGuid);
			appDataNode.AddAttribute(XmlAttributeNameOfBuildDate, app.BuildDate);
			appDataNode.AddAttribute(XmlAttributeNameOfSolutionFilePath, app.SolutionFilePath);
			storageData.AddChild(appDataNode);
			UpdateStorageDataInSettings();
		}
	}
}