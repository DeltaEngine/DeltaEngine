using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	public class SlnFileDataGeneratorTests
	{
		[SetUp]
		public void LoadGeneratedSlnFileData()
		{
			fullProjectFilePath = CreatorTestExtensions.GetDeltaEngineProjectFilePath();
			string directory = Path.GetDirectoryName(fullProjectFilePath);
			projectFileName = Path.GetFileName(fullProjectFilePath);
			var fileDataGenerator = new SlnFileDataGenerator(projectFileName, directory);
			fileData = fileDataGenerator.GenerateVisualStudioSlnFileData();
		}

		private string fullProjectFilePath;
		private string projectFileName;
		private string fileData;

		[Test]
		public void CheckForVisualStudioSlnFileHeader()
		{
			Assert.IsTrue(fileData.Contains("Format Version 12.00"));
			Assert.IsTrue(fileData.Contains("VisualStudioVersion = 12.0"));
			Assert.IsTrue(fileData.Contains("MinimumVisualStudioVersion = 10.0"));
		}

		[Test]
		public void CheckForProjectReferenceInfo()
		{
			Assert.IsTrue(fileData.Contains("Project("));
			Assert.IsTrue(fileData.Contains(projectFileName));
			Assert.IsTrue(fileData.Contains(CsProjectExtensions.GetProjectGuid(fullProjectFilePath)));
			Assert.IsTrue(fileData.Contains("EndProject"));
		}

		[Test]
		public void CheckForGlobalRootSection()
		{
			Assert.IsTrue(fileData.Contains("Global"));
			Assert.IsTrue(fileData.Contains("EndGlobal"));
		}

		[Test]
		public void CheckForGlobalSolutionConfigurationPlatformsSection()
		{
			Assert.IsTrue(fileData.Contains("GlobalSection(SolutionConfigurationPlatforms)"));
			Assert.IsTrue(fileData.Contains("Debug|"));
			Assert.IsTrue(fileData.Contains("Release|"));
			Assert.IsTrue(fileData.Contains("EndGlobalSection"));
		}

		[Test]
		public void CheckForGlobalSolutionProjectConfigurationPlatformsSection()
		{
			Assert.IsTrue(fileData.Contains("GlobalSection(ProjectConfigurationPlatforms)"));
			Assert.IsTrue(fileData.Contains(".Debug|Any CPU.ActiveCfg"));
			Assert.IsTrue(fileData.Contains(".Debug|Any CPU.Build.0"));
			Assert.IsTrue(fileData.Contains("EndGlobalSection"));
		}

		[Test]
		public void CheckForGlobalSolutionSolutionPropertiesSection()
		{
			Assert.IsTrue(fileData.Contains("GlobalSection(SolutionProperties)"));
			Assert.IsTrue(fileData.Contains("HideSolutionNode"));
			Assert.IsTrue(fileData.Contains("EndGlobalSection"));
		}

		[Test]
		public void GivingFailingFilePathThrowsError()
		{
			Assert.Throws<SlnFileDataGenerator.ProjectFilePathMustBeRelativeToSolutionPath>(
				() => new SlnFileDataGenerator("C:/s.exe", ""));
		}
	}
}