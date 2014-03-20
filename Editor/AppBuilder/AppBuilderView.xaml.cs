using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Editor.Messages;
using DeltaEngine.Extensions;
using Microsoft.Win32;

namespace DeltaEngine.Editor.AppBuilder
{
	/// <summary>
	/// Shows all available actions and data provided by the AppBuilderViewModel.
	/// </summary>
	public partial class AppBuilderView : EditorPluginView, IDisposable
	{
		public AppBuilderView()
		{
			InitializeComponent();
		}

		public void Init(Service service)
		{
			ViewModel = new AppBuilderViewModel(service);
			ViewModel.AppBuildFailedRecieved += DispatchAndHandleBuildFailedRecievedEvent;
			ViewModel.BuiltAppRecieved += DispatchAndHandleBuildAppReceivedEvent;
			ViewModel.PropertyChanged += OnPropertyChangedInViewModel;
			ViewModel.Service.DataReceived += DispatchAndHandleOnServiceMessageReceived;
			BuildList.MessagesViewModel = ViewModel.MessagesListViewModel;
			BuildList.AppListViewModel = ViewModel.AppListViewModel;
			BuildList.AppListViewModel.NumberOfBuiltAppsChanged += OnNumberOfBuiltAppsChanged;
			OnNumberOfBuiltAppsChanged();
			SwitchToBuiltApps();
			DataContext = ViewModel;
		}

		private void OnNumberOfBuiltAppsChanged()
		{
			if (BuildList.AppListViewModel.NumberOfBuiltApps > 0)
				BuildAppInfoText.Visibility = Visibility.Hidden;
			else
				BuildAppInfoText.Visibility = Visibility.Visible;
		}

		public void Activate() {}

		public void Deactivate() {}

		public AppBuilderViewModel ViewModel { get; private set; }

		private void DispatchAndHandleBuildFailedRecievedEvent(AppBuildFailed buildFailedMessage)
		{
			Dispatcher.BeginInvoke(new Action(() => OnAppBuildFailedRecieved(buildFailedMessage)));
		}

		private void OnAppBuildFailedRecieved(AppBuildFailed buildFailedMessage)
		{
			var errorMessage = new AppBuildMessage(buildFailedMessage.Reason)
			{
				Project = ViewModel.Service.ProjectName,
				Type = AppBuildMessageType.BuildError,
			};
			ViewModel.MessagesListViewModel.AddMessage(errorMessage);
			SwitchToBuildMessagesList();
		}

		private void SwitchToBuildMessagesList()
		{
			if (BuildList.BuiltAppsList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuildMessagesList));
		}

		private void DispatchAndHandleBuildAppReceivedEvent(AppInfo appInfo, byte[] appData)
		{
			Dispatcher.BeginInvoke(new Action(() => OnBuiltAppRecieved(appInfo, appData)));
		}

		private void DispatchAndHandleOnServiceMessageReceived(object message)
		{
			Dispatcher.BeginInvoke(new Action(() => OnServiceMessageReceived(message)));
		}

		private void OnBuiltAppRecieved(AppInfo appInfo, byte[] appData)
		{
			ViewModel.AppListViewModel.AddApp(appInfo, appData);
			SwitchToBuiltApps();
			InstallAndLaunchNewBuiltApp(appInfo);
		}

		private void SwitchToBuiltApps()
		{
			if (BuildList.BuildMessagesList.IsVisible)
				Dispatcher.BeginInvoke(new Action(BuildList.FocusBuiltAppsList));
		}

		public void InstallAndLaunchNewBuiltApp(AppInfo appInfo)
		{
			if (!appInfo.IsDeviceAvailable)
			{
				AppInfoExtensions.HandleNoDeviceAvailableInView(appInfo);
				UpdateBuildProgressBar("Launching App aborted", 100);
				ViewModel.OpenLocalBuiltAppsDirectory();
				return;
			}
			Device primaryDevice = appInfo.AvailableDevices[0];
			if (primaryDevice.IsAppInstalled(appInfo))
			{
				UpdateBuildProgressBar(appInfo.Name + " was already installed, uninstalling it.", 90);
				primaryDevice.Uninstall(appInfo);
			}
			UpdateBuildProgressBar("Installing " + appInfo.Name + " on " + primaryDevice.Name, 95);
			primaryDevice.Install(appInfo);
			LaunchApp(appInfo, primaryDevice);
		}

		private void LaunchApp(AppInfo appInfo, Device device)
		{
			try
			{
				TryLaunchApp(appInfo, device);
			}
			catch (Device.StartApplicationFailedOnDevice ex)
			{
				AppInfoExtensions.LogStartingAppFailed(appInfo, ex.DeviceName);
			}
		}

		private void TryLaunchApp(AppInfo appInfo, Device device)
		{
			UpdateBuildProgressBar("App launched", 100);
			device.Launch(appInfo);
		}
		
		private void UpdateBuildProgressBar(AppBuildProgress buildProgress)
		{
			UpdateBuildProgressBar(buildProgress.Text, buildProgress.ProgressPercentage);
		}

		private void UpdateBuildProgressBar(string progressText, int progressPercentage)
		{
			BuildProgressText.Text = "Progress: " + progressText;
			BuildProgressBar.Value = progressPercentage;
			ForceRefreshOfBuildProgressControls();
		}

		private void ForceRefreshOfBuildProgressControls()
		{
			BuildProgressText.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
			BuildProgressBar.Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => { }));
		}

		public class NoDeviceAvailable : Exception
		{
			public NoDeviceAvailable(AppInfo appInfo)
				: base(appInfo.ToString()) { }
		}

		private void OnPropertyChangedInViewModel(object sender, PropertyChangedEventArgs e)
		{
			if (!IsVisible)
				return;
			if (e.PropertyName == "selectedCodeProject")
				if (ViewModel.SelectedCodeProject == null)
					ShowProjectNotFoundMessageBox();
		}

		private void ShowProjectNotFoundMessageBox()
		{
			string solutionName = Path.GetFileNameWithoutExtension(ViewModel.UserSolutionPath);
			string text = "The current CodeSolution doesn't contain a project for the selected" +
				" ContentProject " + ViewModel.Service.ProjectName + "." + Environment.NewLine +
				Environment.NewLine +
				"If you want to build a project for " + solutionName + " then please change your" +
				" ContentProject to it";
			MessageBox.Show(text, "Project not found");
		}

		private void OnServiceMessageReceived(object serviceMessage)
		{
			if (serviceMessage is AppBuildProgress)
				OnAppBuildProgressRecieved((AppBuildProgress)serviceMessage);
		}

		private void OnAppBuildProgressRecieved(AppBuildProgress progressMessage)
		{
			UpdateBuildProgressBar(progressMessage);
			if (!ExceptionExtensions.IsDebugMode)
				return;
			string progressText = "Building progress " + progressMessage.ProgressPercentage + "%: " +
				progressMessage.Text;
			Logger.Info(progressText);
		}

		private void OnBrowseUserProjectClicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dialog = CreateUserProjectPathBrowseDialog();
			if (dialog.ShowDialog().Equals(true))
				ViewModel.Service.CurrentContentProjectSolutionFilePath = dialog.FileName;
		}

		private OpenFileDialog CreateUserProjectPathBrowseDialog()
		{
			return new OpenFileDialog
			{
				DefaultExt = ".sln",
				Filter = "C# Solution (.sln)|*.sln",
				InitialDirectory = GetInitialDirectoryForBrowseDialog(),
			};
		}

		private string GetInitialDirectoryForBrowseDialog()
		{
			if (String.IsNullOrWhiteSpace(ViewModel.UserSolutionPath))
				return PathExtensions.GetDeltaEngineInstalledDirectory();
			return Path.GetDirectoryName(ViewModel.UserSolutionPath);
		}

		public string ShortName
		{
			get { return "App Builder"; }
		}

		public string Icon
		{
			get { return @"Images/Plugins/AppBuilder.png"; }
		}

		public bool RequiresLargePane
		{
			get { return true; }
		}

		public void Send(IList<string> arguments) {}

		private void OnGotoLocalBuiltAppsDirectory(object sender, MouseButtonEventArgs e)
		{
			ViewModel.GotoBuiltAppsDirectoryCommand.Execute(null);
		}

		private void OnGotoUserProfilePage(object sender, MouseButtonEventArgs e)
		{
			ViewModel.GotoUserProfilePageCommand.Execute(null);
		}

		private void OnStartBuildClicked(object sender, RoutedEventArgs e)
		{
			UpdateBuildProgressBar("Build started", 1);
			ViewModel.BuildCommand.Execute(null);
		}

		public void Dispose()
		{
			ViewModel.AppBuildFailedRecieved -= DispatchAndHandleBuildFailedRecievedEvent;
			ViewModel.BuiltAppRecieved -= DispatchAndHandleBuildAppReceivedEvent;
			ViewModel.PropertyChanged -= OnPropertyChangedInViewModel;
			ViewModel.Service.DataReceived -= OnServiceMessageReceived;
			BuildList.AppListViewModel.NumberOfBuiltAppsChanged -= OnNumberOfBuiltAppsChanged;
		}
	}
}