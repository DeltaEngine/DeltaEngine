using System;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Core.Tests
{
	public class SolutionExtensionsTests
	{
		[Test]
		public void SamplesSolutionNeedsToBeAvailable()
		{
			var samplesSolutionFilePath = SolutionExtensions.GetSamplesSolutionFilePath();
			Assert.IsTrue(File.Exists(samplesSolutionFilePath));
			Assert.IsTrue(samplesSolutionFilePath.Contains(SolutionExtensions.SamplesSolutionFilename));
		}

		[Test]
		public void TutorialsSolutionNeedsToBeAvailable()
		{
			var tutorialsSolutionFilePath = SolutionExtensions.GetTutorialsSolutionFilePath();
			Assert.IsTrue(File.Exists(tutorialsSolutionFilePath));
			Assert.IsTrue(
				tutorialsSolutionFilePath.Contains(SolutionExtensions.TutorialsSolutionFilename));
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void GetTutorialsSolutionShouldReturnFilePathFromInstallerFolder()
		{
			Environment.CurrentDirectory = @"C:\";
			Assert.IsTrue(
				SolutionExtensions.GetTutorialsSolutionFilePath().Contains(
				@"DeltaEngine.1.0\OpenTK\Tutorials\DeltaEngine.Tutorials.Basics.sln"));
		}
	}
}