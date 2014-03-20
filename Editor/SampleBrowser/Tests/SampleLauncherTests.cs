using System.ComponentModel;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.SampleBrowser.Tests
{
	public class SampleLauncherTests
	{
		[Test, Ignore]
		public void LaunchingInvalidSampleShouldThrow()
		{
			Assert.Throws<Win32Exception>(
				() => SampleLauncher.OpenSolutionForProject(GetTestSample(false)));
			Assert.Throws<Win32Exception>(() => SampleLauncher.StartExecutable(GetGameSample(false)));
		}

		private static Sample GetTestSample(bool isValid)
		{
			return new Sample("TestSample", SampleCategory.Test, "Test.sln", @"Test\Test.csproj",
				GetPathToTestAssembly(isValid)) { EntryClass = "TestClass", EntryMethod = "TestMethod" };
		}

		private static string GetPathToTestAssembly(bool isValid)
		{
			return isValid ? Path.GetFullPath("TestAssembly.dll") : @"Z:\invalid\TestAssembly.dll";
		}

		private static Sample GetGameSample(bool isValid)
		{
			return new Sample("TestSample", SampleCategory.Game, "Test.sln", @"Test\Test.csproj",
				GetPathToTestAssembly(isValid));
		}

		[Test, NUnit.Framework.Category("Slow")]
		public void FakeProjectShouldNotBeAvailable()
		{
			Assert.IsFalse(SampleLauncher.DoesProjectExist(GetTestSample(true)));
			Assert.IsFalse(SampleLauncher.DoesProjectExist(GetGameSample(true)));
		}

		[Test, NUnit.Framework.Category("Slow")]
		public void MockAssemblyShouldBeAvailable()
		{
			Assert.IsTrue(SampleLauncher.DoesAssemblyExist(GetTestSample(true)));
			Assert.IsTrue(SampleLauncher.DoesAssemblyExist(GetGameSample(true)));
		}

		[Test, Ignore]
		public void StartTutorialInVisualStudio()
		{
			const string TutorialDirectory = @"C:\code\DeltaEngine\Tutorials\";
			const string SolutionFilePath = TutorialDirectory + "DeltaEngine.Tutorials.Basics.sln";
			const string ProjectName = "DeltaEngine.Tutorials.Basic01CreateWindow";
			const string ProjectFilePath =
				TutorialDirectory + @"Basic01CreateWindow\" + ProjectName + ".csproj";
			const string ExePath = TutorialDirectory + @"Basic01CreateWindow\" + ProjectName + ".exe";
			var tutorial = new Sample(ProjectName, SampleCategory.Tutorial, SolutionFilePath,
				ProjectFilePath, ExePath);
			SampleLauncher.OpenSolutionForProject(tutorial);
		}
	}
}