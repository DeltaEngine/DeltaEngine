using NUnit.Framework;

namespace DeltaEngine.Editor.ProjectCreator.Tests
{
	public class CsProjectExtensionsTests
	{
		[Test]
		public void CanNotGetProjectGuidWithoutPath()
		{
			Assert.Throws<CsProjectExtensions.NoProjectFilePathSpecified>(
				() => CsProjectExtensions.GetProjectGuid(null));
			Assert.Throws<CsProjectExtensions.NoProjectFilePathSpecified>(
				() => CsProjectExtensions.GetProjectGuid(""));
		}

		[Test]
		public void CanNotGetProjectGuidIfFileIsNoCsProjectFile()
		{
			Assert.Throws<CsProjectExtensions.SpecifiedFileIsNoProjectFile>(
				() => CsProjectExtensions.GetProjectGuid(@"C:\MyProject.proj"));
		}

		[Test]
		public void CanNotGetProjectGuidWithoutExistingProjectFile()
		{
			Assert.Throws<CsProjectExtensions.ProjectFileDoesNotExistsAtSpecifiedPath>(
				() => CsProjectExtensions.GetProjectGuid(@"C:\MyProject.csproj"));
		}

		[Test]
		public void Start()
		{
			Assert.AreEqual("{20FA8D33-A964-4000-AD82-67BD6900793B}",
				CsProjectExtensions.GetProjectGuid(CreatorTestExtensions.GetDeltaEngineProjectFilePath()));
		}
	}
}