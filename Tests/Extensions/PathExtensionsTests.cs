using System;
using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	[Category("Slow")]
	public class PathExtensionsTests
	{
		//ncrunch: no coverage start
		[Test]
		public void InstallerSetsDeltaEnginePathEnvironmentVariable()
		{
			MakeSureEnvironmentVariableIsSet();
			Assert.IsTrue(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		private static void MakeSureEnvironmentVariableIsSet()
		{
			if (!PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				SetEnvironmentVariable(PathExtensions.GetDeltaEngineSolutionFilePath());
		}

		private static void SetEnvironmentVariable(string value)
		{
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, value);
		}

		[Test]
		public void WithoutInstallerDeltaEnginePathEnvironmentVariableIsNotSet()
		{
			DeleteEnvironmentVariableIfSet();
			Assert.IsFalse(PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable());
		}

		private static void DeleteEnvironmentVariableIfSet()
		{
			if (PathExtensions.IsDeltaEnginePathEnvironmentVariableAvailable())
				SetEnvironmentVariable("");
		}

		[Test, Ignore]
		public void DeltaEnginePathEnvironmentVariableMustBeAnExistingDirectory()
		{
			MakeSureEnvironmentVariableIsSet();
			Assert.IsTrue(Directory.Exists(PathExtensions.GetDeltaEngineInstalledDirectory()), 
				PathExtensions.GetDeltaEngineInstalledDirectory());
		}

		[Test]
		public void GetAbsolutePath()
		{
			Assert.AreEqual(Environment.CurrentDirectory, PathExtensions.GetAbsolutePath(null));
			Assert.AreEqual(Environment.CurrentDirectory, PathExtensions.GetAbsolutePath(""));
			Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "File.ext"),
				PathExtensions.GetAbsolutePath("File.ext"));
		}

		[Test, Ignore]
		public void DefaultDeltaEngineSolutionFileShouldBeAvailable()
		{
			string defaultSourceCodeDirectory = PathExtensions.GetDeltaEngineSolutionFilePath();
			Assert.IsTrue(File.Exists(defaultSourceCodeDirectory));
		}

		[Test, Ignore]
		public void InvalidStartupPathWillBeFixed()
		{
			Directory.SetCurrentDirectory(@"C:\");
			var fixedPath = PathExtensions.GetFallbackEngineSourceCodeDirectory();
			Assert.IsTrue(Directory.Exists(fixedPath), fixedPath);
		}

		[Test, Ignore]
		public void DeltaEngineSolutionFileHasToBeAvailable()
		{
			string deltaEngineSolutionFilePath = PathExtensions.GetDeltaEngineSolutionFilePath();
			Assert.IsTrue(deltaEngineSolutionFilePath.Contains(DeltaEngineSolutionFilename));
			Assert.IsTrue(File.Exists(deltaEngineSolutionFilePath));
		}

		private const string DeltaEngineSolutionFilename = "DeltaEngine.sln";
	}
}