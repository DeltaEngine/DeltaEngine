using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DeltaEngine.Core;
using DeltaEngine.Editor.Core;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SampleBrowser
{
	/// <summary>
	/// Searches all Samples and passes them into a container.
	/// </summary>
	public class SampleCreator
	{
		public SampleCreator()
		{
			Samples = new List<Sample>();
			sourceCodeRootDirectory = GetEngineSourceCodeDirectory(Directory.GetCurrentDirectory()) ??
				GetFallbackEngineSourceCodeDirectory();
			sourceCodeSamplesPath = Path.Combine(sourceCodeRootDirectory, "Samples");
			sourceCodeTutorialsPath = Path.Combine(sourceCodeRootDirectory, "Tutorials");
			installPath = PathExtensions.GetDeltaEngineInstalledDirectory() ??
				GetParentOfWorkingDirectory();
		}

		public List<Sample> Samples { get; private set; }
		private readonly string sourceCodeRootDirectory;
		private readonly string sourceCodeSamplesPath;
		private readonly string sourceCodeTutorialsPath;

		private static string GetEngineSourceCodeDirectory(string subDirectory)
		{
			string parentDirectory = Path.GetFullPath(Path.Combine(subDirectory, ".."));
			if (Path.GetFileName(parentDirectory) == "DeltaEngine")
				return parentDirectory;
			if (Path.GetPathRoot(parentDirectory) == parentDirectory)
				return null;
			return GetEngineSourceCodeDirectory(parentDirectory);
		}

		private static string GetFallbackEngineSourceCodeDirectory()
		{
			try
			{
				return TryGetFallbackEngineSourceCodeDirectory();
			}
			catch (PathExtensions.NoDeltaEngineFoundInFallbackPaths)
			{
				return "";
			}
		}

		private static string TryGetFallbackEngineSourceCodeDirectory()
		{
			return PathExtensions.GetFallbackEngineSourceCodeDirectory();
		}

		private readonly string installPath;

		private static string GetParentOfWorkingDirectory()
		{
			return new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
		}

		public void CreateSamples(DeltaEngineFramework framework)
		{
			if (framework == DeltaEngineFramework.Default)
				CreateSamplesFromSourceCodeDirectories();
			else
			{
				CreateSamplesFromInstallerDirectories(framework);
				if (Samples.Count == 0)
					CreateSamplesFromSourceCodeDirectories();
			}
			if (Samples.Count == 0)
				Logger.Warning("No Samples found. Please setup the " +
					PathExtensions.EnginePathEnvironmentVariableName +
					" or compile the DeltaEngine solutions.");
		}

		private void CreateSamplesFromSourceCodeDirectories()
		{
			if (!Directory.Exists(sourceCodeSamplesPath) || !Directory.Exists(sourceCodeTutorialsPath))
				return;
			UsingPrecompiledSamplesFromInstaller = false;
			GetSamplesFromSolutionDirectory();
			GetTutorialsFromSolutionDirectory();
			//GetVisualTestsFromSolutionDirectory(new DirectoryInfo(sourceCodeRootDirectory));
		}

		public bool UsingPrecompiledSamplesFromInstaller { get; private set; }

		private void CreateSamplesFromInstallerDirectories(DeltaEngineFramework framework)
		{
			if (!Directory.Exists(installPath))
				return;
			UsingPrecompiledSamplesFromInstaller = true;
			GetExecutablesFromInstallPath(framework, SampleCategory.Game);
			GetExecutablesFromInstallPath(framework, SampleCategory.Tutorial);
			//GetVisualTestsFromInstallPath(framework);
		}

		private void GetExecutablesFromInstallPath(DeltaEngineFramework framework,
			SampleCategory category)
		{
			if (category != SampleCategory.Game && category != SampleCategory.Tutorial)
				return;
			string frameworkPath = Path.Combine(installPath, framework.ToString());
			string[] directories =
				Directory.GetDirectories(Path.Combine(frameworkPath,
					category == SampleCategory.Game ? "Samples" : "Tutorials"));
			foreach (string projectDirectory in directories)
				if (!projectDirectory.Contains("EmptyLibrary"))
					AddSample(category, projectDirectory, frameworkPath);
		}

		private void AddSample(SampleCategory category, string projectDirectory, string frameworkPath)
		{
			string projectName = GetProjectNameFromLocation(projectDirectory);
			string prefix = category == SampleCategory.Game ? "" : "DeltaEngine.Tutorials.";
			string solutionFilePath = category == SampleCategory.Game
				? Path.Combine(frameworkPath, "DeltaEngine.Samples.sln")
				: Path.Combine(frameworkPath, "Tutorials", GetTutorialSolutionFileName(projectName));
			string projectFilePath = Path.Combine(projectDirectory, prefix + projectName + ".csproj");
			string executableFilePath = Path.Combine(frameworkPath, prefix + projectName + ".exe");
			Samples.Add(new Sample(projectName, category, solutionFilePath, projectFilePath,
				executableFilePath));
		}

		private static string GetProjectNameFromLocation(string projectDirectory)
		{
			string name = projectDirectory.TrimEnd(Path.DirectorySeparatorChar);
			return name.Split(Path.DirectorySeparatorChar).Last();
		}

		private static string GetTutorialSolutionFileName(string projectName)
		{
			return projectName.StartsWith("Basic")
				? "DeltaEngine.Tutorials.Basics.sln" : "DeltaEngine.Tutorials.Entities.sln";
		}

		private void GetVisualTestsFromInstallPath(DeltaEngineFramework framework)
		{
			string frameworkPath = Path.Combine(installPath, framework.ToString());
			foreach (var file in Directory.GetFiles(frameworkPath))
			{
				if (file.Contains(".Editor.") ||
					(!file.EndsWith(".Tests.exe") && !file.EndsWith(".Tests.dll")))
					continue;
				try
				{
					TryGetVisualTestsFromInstallPath(frameworkPath, file);
				}
				catch (ReflectionTypeLoadException ex)
				{
					ShowLoaderExceptionWarning(file, ex);
				}
				catch (FileLoadException ex)
				{
					Logger.Warning("Failed to load dependency for " + file + ": " + ex.Message);
				}
			}
		}

		private void TryGetVisualTestsFromInstallPath(string frameworkPath, string file)
		{
			Assembly assembly = Assembly.LoadFrom(file);
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsDefined(typeof(CompilerGeneratedAttribute), false) || !IsVisualTestClass(type))
					continue;
				foreach (var method in type.GetMethods().Where(IsVisualTestMethod))
				{
					string solutionFilePath = "";
					string projectFilePath = "";
					if (!type.Namespace.Contains("DeltaEngine"))
					{
						solutionFilePath = Path.Combine(frameworkPath, "DeltaEngine.Samples.sln");
						projectFilePath = GetSampleTestsProjectFilePaths(frameworkPath, assembly.GetName().Name);
					}
					Samples.Add(new Sample(assembly.GetName().Name + ": " + method.Name, SampleCategory.Test,
						solutionFilePath, projectFilePath, file)
					{
						EntryClass = type.Name,
						EntryMethod = method.Name
					});
				}
			}
		}

		private static void ShowLoaderExceptionWarning(string file, ReflectionTypeLoadException ex)
		{
			Logger.Warning("Failed to load " + file + ". LoaderExceptions: " +
				ex.LoaderExceptions.ToText());
		}

		private static string GetSampleTestsProjectFilePaths(string frameworkPath, string sampleName)
		{
			string testDirectory = Path.Combine(frameworkPath, "Samples",
				sampleName.Replace(".Tests", ""), "Tests");
			return Directory.GetFiles(testDirectory).FirstOrDefault(f => f.EndsWith(".Tests.csproj")) ??
				"";
		}

		private void GetSamplesFromSolutionDirectory()
		{
			string[] directories = Directory.GetDirectories(sourceCodeSamplesPath);
			string solutionFilePath = Path.Combine(sourceCodeRootDirectory, "DeltaEngine.Samples.sln");
			foreach (string projectDirectory in directories)
			{
				string projectName = GetProjectNameFromLocation(projectDirectory);
				if (!HasExecutableFile(projectDirectory, projectName))
					continue;
				string projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
				if (!File.Exists(projectFile))
					continue;
				string executableFile = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
					projectName + ".exe");
				Samples.Add(new Sample(projectName, SampleCategory.Game, solutionFilePath, projectFile,
					executableFile));
			}
		}

		private void GetTutorialsFromSolutionDirectory()
		{
			string[] directories = Directory.GetDirectories(sourceCodeTutorialsPath);
			foreach (string projectDirectory in directories)
			{
				string projectName = "DeltaEngine.Tutorials." + Path.GetFileName(projectDirectory);
				if (!HasExecutableFile(projectDirectory, projectName))
					continue;
				string projectFile = Path.Combine(projectDirectory, projectName + ".csproj");
				if (!File.Exists(projectFile))
					continue;
				AddTutorial(projectDirectory, projectName, projectFile);
			}
		}

		private static bool HasExecutableFile(string projectDirectory, string projectName)
		{
			string exePath = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
				projectName + ".exe");
			return File.Exists(exePath);
		}

		private static string GetConfigurationName()
		{
			return ExceptionExtensions.IsDebugMode ? "Debug" : "Release";
		}

		private void GetVisualTestsFromSolutionDirectory(DirectoryInfo parentDirectory)
		{
			foreach (var directory in parentDirectory.GetDirectories())
			{
				if (IsIgnored(directory))
					continue;
				if (directory.Name == "Tests")
				{
					string output = Path.Combine(directory.FullName, "bin", GetConfigurationName());
					if (!Directory.Exists(output))
						continue;
					string solutionFilePath = Path.Combine(sourceCodeRootDirectory,
						output.Contains("Samples") ? "DeltaEngine.Samples.sln" : "DeltaEngine.sln");
					FileInfo projectFilePath =
						directory.GetFiles().FirstOrDefault(file => file.Name.EndsWith(".csproj"));
					AddVisualTests(output, parentDirectory.Name, solutionFilePath,
						projectFilePath == null ? "" : projectFilePath.FullName);
				}
				GetVisualTestsFromSolutionDirectory(directory);
			}
		}

		private static bool IsIgnored(DirectoryInfo directory)
		{
			return directory.Name == ".hg" || directory.Name == "obj" || directory.Name == "packages" ||
				directory.Name.StartsWith("_NCrunch_") || directory.Name == "Editor";
		}

		private void AddVisualTests(string testsOutputDirectory, string projectName,
			string solutionFilePath, string projectFilePath)
		{
			foreach (var file in Directory.GetFiles(testsOutputDirectory))
			{
				if (!file.EndsWith(projectName + ".Tests.exe") &&
					!file.EndsWith(projectName + ".Tests.dll"))
					continue;
				try
				{
					TryAddVisualTests(projectName, solutionFilePath, projectFilePath, file);
				}
				catch (ReflectionTypeLoadException ex)
				{
					ShowLoaderExceptionWarning(file, ex);
				}
			}
		}

		private void TryAddVisualTests(string projectName, string solutionFilePath,
			string projectFilePath, string file)
		{
			Assembly assembly = Assembly.LoadFrom(file);
			foreach (var type in assembly.GetTypes())
			{
				if (type.IsDefined(typeof(CompilerGeneratedAttribute), false) || !IsVisualTestClass(type))
					continue;
				foreach (var method in type.GetMethods().Where(IsVisualTestMethod))
					Samples.Add(new Sample(projectName + ": " + method.Name, SampleCategory.Test,
						solutionFilePath, projectFilePath, file)
					{
						EntryClass = type.Name,
						EntryMethod = method.Name
					});
			}
		}

		private void AddTutorial(string projectDirectory, string projectName, string projectFile)
		{
			string solutionFilePath = Path.Combine(projectDirectory, "..",
				GetTutorialSolutionFileName(projectName));
			string executableFile = Path.Combine(projectDirectory, "bin", GetConfigurationName(),
				projectName + ".exe");
			Samples.Add(new Sample(projectName, SampleCategory.Tutorial, solutionFilePath, projectFile,
				executableFile));
		}

		private static bool IsVisualTestClass(Type type)
		{
			return !type.IsInterface &&
				type.BaseType.FullName == "DeltaEngine.Platforms.TestWithMocksOrVisually";
		}

		private static bool IsVisualTestMethod(MethodInfo method)
		{
			object[] attributes = method.GetCustomAttributes(false);
			bool isNUnitTest = false;
			bool isCloseAfterFirstFrame = false;
			foreach (object attribute in attributes)
			{
				if (attribute.GetType().FullName == "NUnit.Framework.TestAttribute")
					isNUnitTest = true;
				if (attribute.GetType().FullName == "DeltaEngine.Platforms.CloseAfterFirstFrameAttribute")
					isCloseAfterFirstFrame = true;
			}
			return isNUnitTest && !isCloseAfterFirstFrame;
		}
	}
}