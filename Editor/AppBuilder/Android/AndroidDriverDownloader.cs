using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Ionic.Zip;

namespace DeltaEngine.Editor.AppBuilder.Android
{
	public class AndroidDriverDownloader
	{
		public AndroidDriverDownloader()
		{
			DownloadDirectory = AndroidDriverFolderName;
		}

		public string DownloadDirectory { get; private set; }
		private const string AndroidDriverFolderName = "AndroidDriver";

		public void DownloadAndroidDriver()
		{
			MakeSureAndroidDriverDirectoryExists();
			if (IsRunningOnWindows8())
				DownloadAndroidDriverForWindows8();
			else
				DownloadAndroidDriverForWindowsLegacy();
		}

		private void MakeSureAndroidDriverDirectoryExists()
		{
			if (!Directory.Exists(DownloadDirectory))
				Directory.CreateDirectory(DownloadDirectory);
		}

		private static bool IsRunningOnWindows8()
		{
			Version pureOsVersion = Environment.OSVersion.Version;
			return pureOsVersion.Major > 6 || (pureOsVersion.Major == 6 && pureOsVersion.Minor > 1);
		}

		private void DownloadAndroidDriverForWindows8()
		{
			string expectedDirectory = Path.Combine(DownloadDirectory, "Win8");
			if (!Directory.Exists(expectedDirectory))
				DownloadAndroidDriverForWin8();
		}

		private void DownloadAndroidDriverForWin8()
		{
			try
			{
				TryDownloadAndroidDriverForWin8("Windows8.zip");
			}
			catch (Exception ex)
			{
				throw new DownloadWindows8DriverFailed(ex);
			}
		}

		public class DownloadWindows8DriverFailed : Exception
		{
			public DownloadWindows8DriverFailed(Exception reason)
				: base(reason.Message, reason) {}
		}

		private void TryDownloadAndroidDriverForWin8(string fileName)
		{
			string fileSourceUrl = "http://DeltaEngine.net/" + AndroidDriverFolderName + "/" + fileName;
			string targetFilePath = Path.Combine(AndroidDriverFolderName, fileName);
			new WebClient().DownloadFile(fileSourceUrl, targetFilePath);
			var driversFile = new ZipFile(targetFilePath);
			driversFile.ExtractAll(DownloadDirectory, ExtractExistingFileAction.OverwriteSilently);
			driversFile.Dispose();
			File.Delete(targetFilePath);
		}

		private void DownloadAndroidDriverForWindowsLegacy()
		{
			string expectedDirectory = Path.Combine(DownloadDirectory, "Win7+Vista+XP");
			if (!Directory.Exists(expectedDirectory))
				TryDownloadAndroidDriverForWindowsLegacy();
		}

		private void TryDownloadAndroidDriverForWindowsLegacy()
		{
			try
			{
				TryDownloadAndroidDriverForWin8("WindowsLegacy.zip");
			}
			catch (Exception ex)
			{
				throw new DownloadWindowsLegacyDriverFailed(ex);
			}
		}

		public class DownloadWindowsLegacyDriverFailed : Exception
		{
			public DownloadWindowsLegacyDriverFailed(Exception reason)
				: base(reason.Message, reason) {}
		}

		public void ShowInstructions()
		{
			try
			{
				TryShowInstructions();
			}
			catch (Exception ex)
			{
				throw new ShowingInstructionsFailed(ex);
			}
		}

		private void TryShowInstructions()
		{
			Process.Start(Path.Combine(DownloadDirectory, "How_to_Install_Drivers.pdf"));
		}

		public class ShowingInstructionsFailed : Exception
		{
			public ShowingInstructionsFailed(Exception reason)
				: base(reason.Message, reason) {}
		}
	}
}
