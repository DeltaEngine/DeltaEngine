using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.AppBuilder.Windows
{
	public class WindowsDevice : Device
	{
		public WindowsDevice()
		{
			Name = Environment.UserDomainName;
			IsEmulator = false;
		}

		public override bool IsAppInstalled(AppInfo app)
		{
			return app != null && Directory.Exists(GetAppExtractionDirectory(app));
		}

		private static string GetAppExtractionDirectory(AppInfo app)
		{
			string fullFilePath = Path.IsPathRooted(app.FilePath)
				? app.FilePath : Path.Combine(Environment.CurrentDirectory, app.FilePath);
			return Path.Combine(Path.GetDirectoryName(fullFilePath), app.Name);
		}

		protected override void InstallApp(AppInfo app)
		{
			var startedProcess = Process.Start(app.FilePath, GetSilentInstallArguments(app));
			startedProcess.WaitForExit();
		}

		private static string GetSilentInstallArguments(AppInfo app)
		{
			string targetDirectory = GetAppExtractionDirectory(app);
			return "/S /D=" + targetDirectory;
		}

		protected override void UninstallApp(AppInfo app)
		{
			Directory.Delete(GetAppExtractionDirectory(app), true);
		}

		protected override void LaunchApp(AppInfo app)
		{
			try
			{
				TryLaunchApp(app);
			}
			catch (Exception ex)
			{
				Logger.Warning(app.Name + " was closed with error: " + ex);
			}
		}

		private void TryLaunchApp(AppInfo app)
		{
			string exeFilePath = Path.Combine(GetAppExtractionDirectory(app), app.Name + ".exe");
			string exeDirectory = PathExtensions.GetAbsolutePath(Path.GetDirectoryName(exeFilePath));
			var processRunner = new ProcessRunner(exeFilePath) { WorkingDirectory = exeDirectory };
			processRunner.Start();
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}