using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class CreateProjectTest
	{
		[Test]
		public void CreateNewProject()
		{
			var project = new CreateProject("TestProject", "TestStarterKit");
			Assert.AreEqual("TestProject", project.ProjectName);
			Assert.AreEqual("TestStarterKit", project.StarterKit);
		}
	}
}
