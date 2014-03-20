using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using DeltaEngine.Editor.Helpers;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor
{
	/// <summary>
	/// Only one instance of the Editor will be allowed and we can send command line arguments to it.
	/// </summary>
	public partial class App : SingleInstanceApp
	{
		[STAThread]
		public static void Main()
		{
			if (!SingleInstance<App>.InitializeAsFirstInstance("DeltaEngine.Editor"))
				return;
			MakeSureEditorDirectoryUsed();
			var application = new App();
			application.InitializeComponent();
			application.Run();
			SingleInstance<App>.Cleanup();
		}

		private static void MakeSureEditorDirectoryUsed()
		{
			if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "DeltaEngine.Editor.exe")))
				return;
			string path = GetInstalledOrFallbackEnginePath();
			if (!String.IsNullOrEmpty(path))
			{
				Directory.SetCurrentDirectory(path);
				if (File.Exists("DeltaEngine.Editor.exe"))
					return;
				var editorPath = Path.Combine(path, "Editor", "bin",
					ExceptionExtensions.IsDebugMode ? "Debug" : "Release");
				if (Directory.Exists(editorPath))
				{
					Directory.SetCurrentDirectory(editorPath);
					if (File.Exists("DeltaEngine.Editor.exe"))
						return;
				}
			}
			MessageBox.Show("Can't find DeltaEngine.Editor.exe. " +
				"Please start the Editor from the folder it is located in!");
			Environment.Exit(-1);
		}

		public static string GetInstalledOrFallbackEnginePath()
		{
			return PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable()
				? Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(), "OpenTK")
				: PathExtensions.GetFallbackEngineSourceCodeDirectory();
		}

		public void SignalExternalCommandLineArguments(IList<string> arguments)
		{
			if (arguments.Count > 1)
				InvokeCommandLineArguments(arguments);
		}

		public static Action<IList<string>> InvokeCommandLineArguments;
	}
}