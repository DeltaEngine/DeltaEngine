using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class ProjectNamesResultTests
	{
		[Test]
		public void CanCreateProjectNamesResult()
		{
			var privateProjects = new[] { "PrivateProject", "AnotherPrivateProject" };
			var publicProjects = new[] { "PublicProject" };
			var projectNamesResult = new ProjectNamesResult(privateProjects, publicProjects);
			Assert.AreEqual(privateProjects.Length, projectNamesResult.PrivateProjects.Length);
			Assert.AreEqual(publicProjects.Length, projectNamesResult.PublicProjects.Length);
		}
	}
}