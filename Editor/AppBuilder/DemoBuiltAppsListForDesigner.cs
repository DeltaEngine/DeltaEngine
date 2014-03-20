using System;
using DeltaEngine.Content.Xml;
using DeltaEngine.Core;
using DeltaEngine.Editor.AppBuilder.Windows;
using DeltaEngine.Mocks;

namespace DeltaEngine.Editor.AppBuilder
{
	internal class DemoBuiltAppsListForDesigner : BuiltAppsListViewModel
	{
		public DemoBuiltAppsListForDesigner()
			: base(GetMockSettingsWithAlreadyBuiltApps())
		{
		}

		private static Settings GetMockSettingsWithAlreadyBuiltApps()
		{
			var settings = new MockSettings();
			var storageData = new XmlData("BuiltApps");
			storageData.AddAttribute("StoragePath", storageData.Name);
			settings.SetValue(storageData.Name, storageData);
			var appsStorage = new AppsStorage(settings);
			appsStorage.AddApp(new WindowsAppInfo("Rebuildable app", Guid.NewGuid(), DateTime.Now)
			{
				SolutionFilePath = "A.sln"
			});
			appsStorage.AddApp(new WindowsAppInfo("Non-Rebuildable app ", Guid.NewGuid(), DateTime.Now));
			return settings;
		}
	}
}