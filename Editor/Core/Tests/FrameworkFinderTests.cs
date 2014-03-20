using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Editor.Mocks;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class FrameworkFinderTests
	{
		[Test]
		public void SearchForAllFrameworksOnlyOnce()
		{
			var spy = new FrameworkFinderSpy();
			Assert.AreEqual(0, spy.NumberOfSearches);
			DeltaEngineFramework[] allFrameworks;
			Assert.IsNotNull(allFrameworks = spy.All);
			Assert.AreEqual(1, spy.NumberOfSearches);
			Assert.AreEqual(allFrameworks, spy.All);
			Assert.AreEqual(1, spy.NumberOfSearches);
		}

		[Test]
		public void SearchForDefaultFrameworkOnlyOnce()
		{
			var spy = new FrameworkFinderSpy();
			Assert.AreEqual(0, spy.NumberOfSearches);
			DeltaEngineFramework defaultFramework;
			Assert.IsNotNull(defaultFramework = spy.Default);
			Assert.AreEqual(1, spy.NumberOfSearches);
			Assert.AreEqual(defaultFramework, spy.Default);
			Assert.AreEqual(1, spy.NumberOfSearches);
		}

		[Test]
		public void WithoutDeltaEngineEnvironmentVariableSetTheCurrentWorkingDirectoryIsUsedAsBase()
		{
			DeleteEnvironmentVariable();
			var spy = new FrameworkFinderSpy();
			int allFrameworksWithoutDefault = DeltaEngineFramework.Default.GetCount() - 1;
			Assert.AreEqual(allFrameworksWithoutDefault, spy.All.Length);
			Assert.AreEqual(allFrameworksWithoutDefault + 2, spy.PotentialFrameworkDirectories.Length);
		}

		private static void DeleteEnvironmentVariable()
		{
			Environment.SetEnvironmentVariable(PathExtensions.EnginePathEnvironmentVariableName, "");
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void OutputDirectoryOfTestsIsNotValidFrameworkDirectory()
		{
			DeleteEnvironmentVariable();
			var spy = new FrameworkFinder();
			Assert.AreEqual(0, spy.All.Length);
		}

		[Test, Category("Slow")]
		public void DeltaEngineInstallerShouldHaveBeenExecutedAndAllFrameworkFoldersBeenCreated()
		{
			var spy = new FrameworkFinder();
			Assert.AreEqual(6, spy.All.Length);
		}

		[Test, Category("Slow")]
		public void CheckAvailableDeltaEngineFrameworks()
		{
			DeleteEnvironmentVariable();
			CreateFrameworkFolders(GetAllDeltaEngineFrameworks());
			frameworks = new FrameworkFinder();
			Assert.AreEqual(6, frameworks.All.Length);
			DeleteFrameworkFolders();
		}

		private static IEnumerable<string> GetAllDeltaEngineFrameworks()
		{
			return new[] { "GLFW", "MonoGame", "OpenTK", "SharpDX", "SlimDX", "Xna" };
		}

		private void CreateFrameworkFolders(IEnumerable<string> frameworksToCreate)
		{
			installerDirectories.AddRange(frameworksToCreate);
			foreach (var directoryName in installerDirectories)
			{
				Directory.CreateDirectory(directoryName);
				foreach (var additionalSubFolder in additionalSubFolders)
					Directory.CreateDirectory(Path.Combine(directoryName, additionalSubFolder));
			}
		}

		private readonly List<string> installerDirectories = new List<string>();
		private readonly string[] additionalSubFolders = { "Samples", "VisualStudioTemplates" };
		private FrameworkFinder frameworks;

		private void DeleteFrameworkFolders()
		{
			foreach (var directoryName in installerDirectories)
			{
				foreach (var additionalSubFolder in additionalSubFolders)
				{
					var folderToDelete = Path.Combine(directoryName, additionalSubFolder);
					if (Directory.Exists(folderToDelete))
						Directory.Delete(folderToDelete);
				}
				if (Directory.Exists(directoryName))
					Directory.Delete(directoryName);
			}
		}

		[Test, Category("Slow")]
		public void CheckAvailabilityOfDefaultFramework()
		{
			DeleteEnvironmentVariable();
			CreateFrameworkFolders(GetAllDeltaEngineFrameworks());
			frameworks = new FrameworkFinder();
			Assert.AreEqual(DefaultFramework, frameworks.Default);
			DeleteFrameworkFolders();
		}

		private const DeltaEngineFramework DefaultFramework = DeltaEngineFramework.OpenTK;

		[Test, Category("Slow")]
		public void InvalidFrameworkStructureDoesNotShowUp()
		{
			DeleteEnvironmentVariable();
			Directory.CreateDirectory(DefaultFramework.ToString());
			var invalidFrameworks = new FrameworkFinder();
			Assert.AreEqual(0, invalidFrameworks.All.Length);
			Directory.Delete(DefaultFramework.ToString());
			DeleteFrameworkFolders();
		}
	}
}