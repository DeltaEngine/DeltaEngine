using System.Diagnostics;
using System.IO;

namespace DeltaEngine.Editor.AppBuilder.Web
{
	public class WebDevice : Device
	{
		public WebDevice()
		{
			Name = "Web";
			IsEmulator = false;
		}

		public override bool IsAppInstalled(AppInfo app)
		{
			return File.Exists(app.FilePath);
		}

		protected override void InstallApp(AppInfo app) { }

		protected override void UninstallApp(AppInfo app) {}

		protected override void LaunchApp(AppInfo app)
		{
			Process.Start(app.FilePath);
		}

		public override string ToString()
		{
			return GetType().Name + "(" + Name + ")";
		}
	}
}