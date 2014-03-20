using System;
using System.Windows;
using System.Windows.Controls;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Shows all available apps provided by the BuiltAppsListViewModel.
	/// </summary>
	public partial class BuiltAppsListView
	{
		public BuiltAppsListView()
		{
			InitializeComponent();
		}

		public BuiltAppsListViewModel ViewModel
		{
			get { return viewModel; }
			set
			{
				viewModel = value;
				DataContext = value;
			}
		}
		private BuiltAppsListViewModel viewModel;

		private void OnRebuildAppClicked(object rebuildButton, RoutedEventArgs e)
		{
			AppInfo boundApp = GetBoundApp(rebuildButton);
			viewModel.RequestRebuild(boundApp);
		}

		private static AppInfo GetBoundApp(object clickedButton)
		{
			Panel controlOwner = GetControlOwner(clickedButton);
			return GetBoundApp(controlOwner);
		}

		private static Panel GetControlOwner(object clickedControl)
		{
			return (clickedControl as Control).Parent as Panel;
		}

		private static AppInfo GetBoundApp(FrameworkElement controlOwner)
		{
			return controlOwner.DataContext as AppInfo;
		}

		private void OnLaunchAppClicked(object launchButton, RoutedEventArgs e)
		{
			Panel controlOwner = GetControlOwner(launchButton);
			AppInfo selectedApp = GetBoundApp(controlOwner);
			(launchButton as Control).IsEnabled = false;
			Action enableLaunchButtonAgain =
				() => Dispatcher.BeginInvoke(new Action(() => (launchButton as Control).IsEnabled = true));
			ThreadExtensions.Start(() => LaunchApp(selectedApp, enableLaunchButtonAgain));
		}

		private static void LaunchApp(AppInfo selectedApp, Action enableLaunchButtonAgain)
		{
			try
			{
				TryLaunchApp(selectedApp);
			}
			catch (Device.StartApplicationFailedOnDevice ex)
			{
				AppInfoExtensions.LogStartingAppFailed(selectedApp, ex.DeviceName);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
			enableLaunchButtonAgain();
		}

		private static void TryLaunchApp(AppInfo selectedApp)
		{
			if (!selectedApp.IsDeviceAvailable)
			{
				AppInfoExtensions.HandleNoDeviceAvailableInView(selectedApp);
				return;
			}
			Device primaryDevice = selectedApp.AvailableDevices[0];
			if (!primaryDevice.IsAppInstalled(selectedApp))
			{
				Logger.Info(selectedApp + " wasn't installed on the device '" + primaryDevice +
					"' will install it now.");
				primaryDevice.Install(selectedApp);
			}
			selectedApp.LaunchAppOnPrimaryDevice();
		}
	}
}
