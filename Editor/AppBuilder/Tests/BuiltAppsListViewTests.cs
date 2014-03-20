using System;
using System.Collections.Generic;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[Category("Slow"), Category("WPF")]
	public class BuiltAppsListViewTests
	{
		[Test, STAThread]
		public void ShowViewWithOneBuiltApp()
		{
			var listViewModel = GetBuiltAppsListWithDummyEntry();
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}

		private static BuiltAppsListViewModel GetBuiltAppsListWithDummyEntry()
		{
			var listViewModel = new BuiltAppsListViewModel(new MockSettings());
			listViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Windows app",
				PlatformName.Windows));
			return listViewModel;
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(
			BuiltAppsListViewModel listViewModel)
		{
			var appsListView = new BuiltAppsListView();
			appsListView.ViewModel = listViewModel;
			return new WpfWindow { Content = appsListView, Width = 800, Height = 480 };
		}

		[Test, STAThread]
		public void ShowViewWithSeveralAppEntries()
		{
			BuiltAppsListViewModel list = GetBuiltAppsListWithDummyEntry();
			list.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Windows app", PlatformName.Windows));
			list.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Android app", PlatformName.Android));
			list.AddApp(AppBuilderTestExtensions.GetMockAppInfo("Web app", PlatformName.Web));
			var window = CreateVerifiableWindowFromViewModel(list);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowIconOfOfficialSupportedPlatforms()
		{
			BuiltAppsListViewModel list = GetBuiltAppsListWithDummyEntry();
			IEnumerable<PlatformName> allPlatforms = EnumExtensions.GetEnumValues<PlatformName>();
			foreach (PlatformName platform in allPlatforms)
				if (platform != PlatformName.WindowsPhone7)
					list.AddApp(AppBuilderTestExtensions.GetMockAppInfo(platform + " app", platform));
			var window = CreateVerifiableWindowFromViewModel(list);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowViewWithLogoAppForWindows()
		{
			var listViewModel = new BuiltAppsListViewModel(new MockSettings());
			AppInfo app = AppBuilderTestExtensions.TryGetAlreadyBuiltApp("LogoApp", PlatformName.Windows);
			listViewModel.AddApp(app);
			var window = CreateVerifiableWindowFromViewModel(listViewModel);
			window.ShowDialog();
		}
	}
}