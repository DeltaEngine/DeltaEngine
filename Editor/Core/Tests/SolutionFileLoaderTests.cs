using System;
using System.Collections.Generic;
using System.IO;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class SolutionFileLoaderTests
	{
		[Test]
		public void WithoutSolutionThereWillBeNoAvailableProjects()
		{
			Assert.IsEmpty(new SolutionFileLoader(null).GetCSharpProjects());
			Assert.IsEmpty(new SolutionFileLoader("").GetCSharpProjects());
		}

		[Test]
		public void LoadProjectEntriesOfDeltaEngineSolution()
		{
			SolutionFileLoader solutionLoader = GetLoaderWithLoadedEngineSolution();
			Assert.IsNotEmpty(solutionLoader.ProjectEntries);
		}

		private static SolutionFileLoader GetLoaderWithLoadedEngineSolution()
		{
			return new SolutionFileLoader(PathExtensions.GetDeltaEngineSolutionFilePath());
		}

		[Test]
		public void GetCSharpProjectsOfDeltaEngineSolution()
		{
			var solutionLoader = GetLoaderWithLoadedEngineSolution();
			List<ProjectEntry> csharpProjects = solutionLoader.GetCSharpProjects();
			Assert.IsNotEmpty(csharpProjects);
			Assert.IsTrue(csharpProjects.Exists(project => project.Name == "DeltaEngine.Platforms"));
		}

		[Test]
		public void GetProjectsFoldersOfDeltaEngineSolution()
		{
			var solutionLoader = GetLoaderWithLoadedEngineSolution();
			List<ProjectEntry> csharpProjects = solutionLoader.GetSolutionFolders();
			Assert.IsNotEmpty(csharpProjects);
			Assert.IsTrue(csharpProjects.Exists(project => project.Name == "Platforms"));
		}

		[Test]
		public void ProjectNotFoundInSolutionException()
		{
			var solutionLoader = GetLoaderWithLoadedEngineSolution();
			Assert.Throws<SolutionFileLoader.ProjectNotFoundInSolution>(
				() => solutionLoader.GetCSharpProject("NonExistingProject"));
		}

		[Test]
		public void GetSpecificCSharpProjectFromDeltaEngineSamplesSolution()
		{
			string engineSamplesSolution = SolutionExtensions.GetSamplesSolutionFilePath();
			Assert.IsTrue(File.Exists(engineSamplesSolution));
			var solutionLoader = new SolutionFileLoader(engineSamplesSolution);
			ProjectEntry logoAppProject = solutionLoader.GetCSharpProject("LogoApp");
			Assert.IsNotNull(logoAppProject);
			Assert.AreEqual("LogoApp", logoAppProject.Name);
		}
	}
}