using System;
using System.IO;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.Core
{
	public static class SolutionExtensions
	{
		public static string GetSamplesSolutionFilePath()
		{
			return GetFilePathFromSourceCode(SamplesSolutionFilename) ??
				GetFilePathFromInstallerRelease(SamplesSolutionFilename);
		}

		public const string SamplesSolutionFilename = "DeltaEngine.Samples.sln";

		private static string GetFilePathFromSourceCode(string filename)
		{
			string originalDirectory = Environment.CurrentDirectory;
			try
			{
				if (StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					Environment.CurrentDirectory = PathExtensions.GetFallbackEngineSourceCodeDirectory();
				var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
				return GetFilePathFromSourceCodeRecursively(currentDirectory, filename);
			}
			finally
			{
				Environment.CurrentDirectory = originalDirectory;
			}
		}

		private static string GetFilePathFromSourceCodeRecursively(DirectoryInfo directory,
			string filename)
		{
			if (directory.Parent == null)
				return null; //ncrunch: no coverage
			foreach (var file in directory.GetFiles())
				if (file.Name == filename)
					return file.FullName;
			return GetFilePathFromSourceCodeRecursively(directory.Parent, filename); //ncrunch: no coverage
		}

		//ncrunch: no coverage start
		private static string GetFilePathFromInstallerRelease(string filePath)
		{
			return PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable()
				? Path.Combine(PathExtensions.GetDeltaEngineInstalledDirectory(), "OpenTK", filePath)
				: null;
		} //ncrunch: no coverage end

		public static string GetTutorialsSolutionFilePath()
		{
			string sourceSamplesSln = GetFilePathFromSourceCode(SamplesSolutionFilename);
			if (!string.IsNullOrEmpty(sourceSamplesSln))
				return Path.Combine(new DirectoryInfo(sourceSamplesSln).Parent.FullName, "Tutorials",
					TutorialsSolutionFilename);
			//ncrunch: no coverage start
			string installerSamplesSln = GetFilePathFromInstallerRelease(SamplesSolutionFilename);
			if (!string.IsNullOrEmpty(installerSamplesSln))
				return Path.Combine(new DirectoryInfo(installerSamplesSln).Parent.FullName, "Tutorials",
					TutorialsSolutionFilename);
			return null;
		}

		public const string TutorialsSolutionFilename = "DeltaEngine.Tutorials.Basics.sln";
	}
}