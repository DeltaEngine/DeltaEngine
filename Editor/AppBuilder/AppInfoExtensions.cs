using System;
using System.Diagnostics;
using System.Windows;
using DeltaEngine.Core;
using DeltaEngine.Editor.AppBuilder.Android;
using DeltaEngine.Editor.AppBuilder.Web;
using DeltaEngine.Editor.AppBuilder.Windows;
using DeltaEngine.Editor.AppBuilder.WindowsPhone7;
using DeltaEngine.Editor.Messages;

namespace DeltaEngine.Editor.AppBuilder
{
	// ncrunch: no coverage start
	public static class AppInfoExtensions
	{
		//this should be a factory with a plugin system
		public static AppInfo CreateAppInfo(string appFilePath, PlatformName platform, Guid appGuid,
			DateTime buildDate)
		{
			switch (platform)
			{
				case PlatformName.Windows:
					return new WindowsAppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.WindowsPhone7:
					return new WP7AppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.Android:
					return new AndroidAppInfo(appFilePath, appGuid, buildDate);
				case PlatformName.Web:
					return new WebAppInfo(appFilePath, appGuid, buildDate);
				default:
					return new UnknownDeviceAppInfo(appFilePath, appGuid, platform, buildDate);
			}
		}

		public static void HandleNoDeviceAvailableInView(AppInfo appInfo)
		{
			LogNoDeviceAvailable(appInfo);
			if (appInfo is Android.AndroidAppInfo)
				if (ShowDownloadAndroidDriversQuestionAndGetUserDecision() == MessageBoxResult.Yes)
					DownloadAndroidDriverAndShowInstructions();
		}

		private static void LogNoDeviceAvailable(AppInfo appToStart)
		{
			Logger.Warning("No " + appToStart.Platform + " device found. Please make sure your" +
				" device is connected and you have the correct driver installed.");
		}

		private static MessageBoxResult ShowDownloadAndroidDriversQuestionAndGetUserDecision()
		{
			const string MessageTitle = "Android Driver";
			string newLine = Environment.NewLine;
			string messageText = "There was currently no Android device found." + newLine +
				"Please enable USB-Debugging on your Android device and approve the fingerprint to your PC" +
				newLine +
				newLine +
				"If the driver for your Android device is missing, click yes to download it " +
				" automatically.The folder with the driver and an instruction manual (pdf) will open to" +
				" help you with the installation.";
			return MessageBox.Show(messageText, MessageTitle, MessageBoxButton.YesNo);
		}

		private static void DownloadAndroidDriverAndShowInstructions()
		{
			try
			{
				TryDownloadAndroidDriverAndShowInstructions();
			}
			catch (Exception ex)
			{
				Logger.Warning("Failed to download the Android drivers because: " + ex.Message);
			}
		}

		private static void TryDownloadAndroidDriverAndShowInstructions()
		{
			var driverDownloader = new AndroidDriverDownloader();
			driverDownloader.DownloadAndroidDriver();
			Process.Start(driverDownloader.DownloadDirectory);
			OpenAndroidDriverInstallationManual(driverDownloader);
		}

		private static void OpenAndroidDriverInstallationManual(AndroidDriverDownloader downloader)
		{
			try
			{
				TryOpenAndroidDriverInstallationManual(downloader);
			}
			catch (AndroidDriverDownloader.ShowingInstructionsFailed ex)
			{
				Logger.Warning("Could not open the manual of 'How to install Android drivers' because: " +
					ex.Message);
			}
		}

		private static void TryOpenAndroidDriverInstallationManual(AndroidDriverDownloader downloader)
		{
			downloader.ShowInstructions();
		}

		public static void LogStartingAppFailed(AppInfo appToStart, string deviceName)
		{
			Logger.Warning(appToStart.Name + " can't be started on your " + appToStart.Platform +
				" device (" + deviceName + "). Please make sure your device is connected and the app was" +
				" correctly installed.");
		}
	}
}
