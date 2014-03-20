using System;
using System.Diagnostics;
using System.IO;
using DeltaEngine.Extensions;
using Microsoft.Win32;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Starts files associated with Samples.
	/// </summary>
	public static class SampleLauncher
	{
		public static void OpenSolutionForProject(Sample sample)
		{
			if (sample.Category == SampleCategory.Game)
				Process.Start(sample.ProjectFilePath);
			else
				StartVisualStudio(sample.SolutionFilePath, sample.ProjectFilePath, sample.EntryClass);
		}

		private static void StartVisualStudio(string solutionFilePath, string projectFilePath,
			string entryClass)
		{
			string cSharpFilePath =
				GetInitialProjectCSharpFilePath(Path.GetDirectoryName(projectFilePath), entryClass);
			Process.Start(Path.Combine(GetLatestVisualStudioBinPath(), "devenv.exe"),
				solutionFilePath + " " + cSharpFilePath);
		}

		private static string GetInitialProjectCSharpFilePath(string projectPath,
			string entryClassName)
		{
			string classFileName = Path.GetFullPath(Path.Combine(projectPath, entryClassName + ".cs"));
			string gameFileName = Path.GetFullPath(Path.Combine(projectPath, "Game.cs"));
			string programFileName = Path.GetFullPath(Path.Combine(projectPath, "Program.cs"));
			return File.Exists(classFileName)
				? classFileName : (File.Exists(gameFileName) ? gameFileName : programFileName);
		}

		private static string GetLatestVisualStudioBinPath()
		{
			for (int versionNumber = 12; versionNumber >= 8; versionNumber--)
			{
				var key = Environment.Is64BitOperatingSystem
					? "Wow6432Node\\Microsoft\\VisualStudio\\" + versionNumber + ".0\\"
					: "Microsoft\\VisualStudio\\" + versionNumber + ".0\\";
				var installationPath =
					(string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\" + key, "InstallDir", null);
				if (!String.IsNullOrEmpty(installationPath))
					return installationPath;
			}
			return "";
		}

		public static bool DoesProjectExist(Sample sample)
		{
			return File.Exists(sample.ProjectFilePath);
		}

		public static void StartExecutable(Sample sample)
		{
			if (sample.Category == SampleCategory.Test)
				StartTest(sample.AssemblyFilePath, sample.EntryClass, sample.EntryMethod);
			else
				StartExecutable(sample.AssemblyFilePath);
		}

		private static void StartExecutable(string filePath)
		{
			int index = filePath.LastIndexOf(@"\", StringComparison.Ordinal);
			string exeDirectory = filePath.Substring(0, index);
			var processInfo = new ProcessStartInfo(filePath) { WorkingDirectory = exeDirectory };
			Process.Start(processInfo);
		}

		private static void StartTest(string assembly, string entryClass, string entryMethod)
		{
			using (var starter = new AssemblyStarter(assembly, false))
				starter.Start(entryClass, entryMethod);
		}

		public static bool DoesAssemblyExist(Sample sample)
		{
			return File.Exists(sample.AssemblyFilePath);
		}
	}
}