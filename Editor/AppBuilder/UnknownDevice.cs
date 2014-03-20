using System;
using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder
{
	public class UnknownDevice : Device
	{
		public override bool IsAppInstalled(AppInfo app)
		{
			return File.Exists(app.FilePath);
		}

		protected override void InstallApp(AppInfo app) { }

		protected override void UninstallApp(AppInfo app)
		{
			File.Delete(app.FilePath);
		}

		// ncrunch: no coverage start
		protected override void LaunchApp(AppInfo app)
		{
			string appFileDirectory = Path.GetDirectoryName(app.FilePath);
			if (appFileDirectory.Length == 0)
				appFileDirectory = Environment.CurrentDirectory;
			Process.Start(appFileDirectory);
		}
	}
}