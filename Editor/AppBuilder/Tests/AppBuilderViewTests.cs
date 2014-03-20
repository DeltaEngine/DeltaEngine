using System;
using System.Windows.Input;
using ApprovalTests.Reporters;
using ApprovalTests.Wpf;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using NUnit.Framework;
using WpfWindow = System.Windows.Window;

namespace DeltaEngine.Editor.AppBuilder.Tests
{
	[UseReporter(typeof(KDiffReporter))]
	[Category("Slow"), Category("WPF"), Timeout(60000)]
	public class AppBuilderViewTests
	{
		[SetUp]
		public void InitializeService()
		{
			service = new MockBuildService();
		}

		private MockBuildService service;

		[Test, STAThread]
		public void VerifyViewWithMocking()
		{
			WpfApprovals.Verify(CreateTestWindow(CreateViewAndViewModelViaMockService()));
		}

		private static WpfWindow CreateTestWindow(AppBuilderView view)
		{
			return new WpfWindow
			{
				Content = view,
				Width = 1280,
				Height = 720,
				MinWidth = 800,
				MinHeight = 480
			};
		}

		private AppBuilderView CreateViewAndViewModelViaMockService()
		{
			var view = CreateViewWithInitialiedViewModel(service);
			service.ReceiveSomeTestMessages();
			return view;
		}

		private static AppBuilderView CreateViewWithInitialiedViewModel(Service service)
		{
			var view = new AppBuilderView();
			view.Init(service);
			return view;
		}

		[Test, STAThread]
		public void ShowViewWithMockServiceWithEngineContentProjects()
		{
			service.SetAvailableProjects("LogoApp", "GhostWars", "Insight", "DeltaEngine.Tutorials");
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			WpfWindow window = CreateTestWindow(builderView);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowViewWithMockServiceAndDummyApps()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			AppBuilderViewModel viewModel = builderView.ViewModel;
			viewModel.AppListViewModel.AddApp(
				AppBuilderTestExtensions.GetMockAppInfo("My favorite app", PlatformName.Windows));
			viewModel.AppListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo(
				"My mobile app", PlatformName.Android));
			viewModel.AppListViewModel.AddApp(AppBuilderTestExtensions.GetMockAppInfo(
				"My cool web app", PlatformName.Web));
			WpfWindow window = CreateTestWindow(builderView);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowViewWithMockServiceAndLoadedAppStorage()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			WpfWindow window = CreateTestWindow(builderView);
			window.ShowDialog();
		}

		[Test, STAThread]
		public void ShowViewWithIncreasingProgressOnMouseDoubleClick()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			WpfWindow window = CreateTestWindow(builderView);
			window.MouseDoubleClick += (sender, e) => UpdateBuildProgress(builderView, 10);
			window.ShowDialog();
		}

		private void UpdateBuildProgress(AppBuilderView builderView, int percentageIncrease)
		{
			int finalValue = (int)builderView.BuildProgressBar.Value + percentageIncrease;
			if (finalValue > 100)
				finalValue -= 100;
			service.ReceiveData(new AppBuildProgress("Build progress " + finalValue, finalValue));

		}

		[Test, STAThread]
		public void ShowViewWithMockServiceToVisualizeSwitchingBetweenBothLists()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			AppBuilderViewModel viewModel = builderView.ViewModel;
			WpfWindow window = CreateTestWindow(builderView);
			window.MouseDoubleClick += (sender, e) => FireAppBuildMessagesOnMouseDoubleClick(e, viewModel);
			window.ShowDialog();
		}

		private void FireAppBuildMessagesOnMouseDoubleClick(MouseButtonEventArgs e,
			AppBuilderViewModel viewModel)
		{
			if (e.LeftButton != MouseButtonState.Released)
				service.ChangeProject("LogoApp");
			else if (e.RightButton != MouseButtonState.Released)
				service.ChangeProject("GhostWars");
			viewModel.BuildCommand.Execute(null);
		}

		[Test, STAThread]
		public void ShowInfoDialogWhenNoSolutionProjectIsAvailableForContentProject()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			WpfWindow window = CreateTestWindow(builderView);
			window.MouseDoubleClick += (sender, e) => service.ChangeProject("NonExistingProject");
			window.ShowDialog();
		}

		[Test, STAThread, Ignore]
		public void AppsDirectoryWillBeOpenedWhenDeviceIsNotAvailable()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			builderView.InstallAndLaunchNewBuiltApp(new AppInfoOfUndeployablePlatformApp());
		}

		private class AppInfoOfUndeployablePlatformApp : AppInfo
		{
			public AppInfoOfUndeployablePlatformApp()
				: base("UndeployableApp.any", Guid.Empty, (PlatformName)999, DateTime.Now) { }

			protected override Device[] GetAvailableDevices()
			{
				return new Device[0];
			}
		}

		[Test, STAThread]
		public void DownloadAndroidDriversAndOpenInstructions()
		{
			AppBuilderView builderView = CreateViewAndViewModelViaMockService();
			builderView.InstallAndLaunchNewBuiltApp(new AndroidAppInfoWithoutDevices());
		}

		private class AndroidAppInfoWithoutDevices : AndroidAppInfo
		{
			public AndroidAppInfoWithoutDevices()
				: base("AnyAndroidApp.apk", Guid.Empty, DateTime.Now) { }

			protected override Device[] GetAvailableDevices()
			{
				return new Device[0];
			}
		}
	}
}