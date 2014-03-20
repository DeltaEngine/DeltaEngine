using System.IO;
using System.Net;

namespace DeltaEngine.Editor.AppBuilder.Android
{
	public class AdbPathProvider
	{
		public string GetAdbPath()
		{
			if (IsSupportFileNotAvailable(AdbExe))
				DownloadAndroidSupportFiles();
			return GetExpectedSupportFilePath(AdbExe);
		}

		private const string AdbExe = "adb.exe";

		private static bool IsSupportFileNotAvailable(string fileName)
		{
			return !File.Exists(GetExpectedSupportFilePath(fileName));
		}

		private static string GetExpectedSupportFilePath(string fileName)
		{
			return Path.Combine(AndroidSupportFilesFolderName, fileName);
		}

		public const string AndroidSupportFilesFolderName = "AndroidSupportFiles";

		private static void DownloadAndroidSupportFiles()
		{
			MakeSureTargetDirectoryExists();
			string[] requiredFiles = new[] { AdbExe, "aapt.exe", "AdbWinApi.dll", "AdbWinUsbApi.dll" };
			var webClient = new WebClient();
			foreach (string file in requiredFiles)
				if (IsSupportFileNotAvailable(file))
					DownloadSupportFile(webClient, file);
		}

		private static void MakeSureTargetDirectoryExists()
		{
			if (!Directory.Exists(AndroidSupportFilesFolderName))
				Directory.CreateDirectory(AndroidSupportFilesFolderName);
		}

		private static void DownloadSupportFile(WebClient downloadClient, string fileName)
		{
			string sourceUrl = Path.Combine("http://DeltaEngine.net/", AndroidSupportFilesFolderName,
				fileName);
			string targetPath = Path.Combine(AndroidSupportFilesFolderName, fileName);
			downloadClient.DownloadFile(sourceUrl, targetPath);
		}
	}
}