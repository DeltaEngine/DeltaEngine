using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class ChangeProjectRequestTests
	{
		[Test]
		public void CanCreateChangeProjectRequest()
		{
			changeProjectRequest = new ChangeProjectRequest("TestProject");
			Assert.AreEqual("TestProject", changeProjectRequest.ProjectName);
		}

		private ChangeProjectRequest changeProjectRequest;
	}
}