using System;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Mocks;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	public class AppBuilderInfoListViewTests
	{
		[Test, STAThread, Category("Slow"), Category("WPF")]
		public void ShowViewWithMockService()
		{
			var infoListView = new AppBuilderInfoListView();
			infoListView.MessagesViewModel = CreateViewModelWithDummyMessages();
			infoListView.AppListViewModel = CreateAppsListViewModelWithDummyEntries();
			infoListView.FocusBuiltAppsList();
			var window = CreateVerifiableWindowFromViewModel(infoListView);
			window.ShowDialog();
		}

		private static AppBuildMessagesListViewModel CreateViewModelWithDummyMessages()
		{
			var listViewModel = new AppBuildMessagesListViewModel();
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestWarning("A simple build warning"));
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("A simple build error"));
			listViewModel.AddMessage(
				AppBuilderTestExtensions.AsBuildTestError("A second simple build error"));
			return listViewModel;
		}

		private static BuiltAppsListViewModel CreateAppsListViewModelWithDummyEntries()
		{
			var appListViewModel = new BuiltAppsListViewModel(new MockSettings());
			appListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("A Windows app",
				PlatformName.Windows));
			appListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo("A Windows Phone 7 app",
				PlatformName.WindowsPhone7));
			return appListViewModel;
		}

		private static WpfWindow CreateVerifiableWindowFromViewModel(AppBuilderInfoListView view)
		{
			return new WpfWindow { Content = view, Width = 800, Height = 480 };
		}
	}
}
