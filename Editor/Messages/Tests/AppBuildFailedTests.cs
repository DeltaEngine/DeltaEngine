using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class AppBuildFailedTests
	{
		[Test]
		public void AppBuildFailedMessageMustBeRestorable()
		{
			var failedMessage = new AppBuildFailed("Whatever");
			MessageTestExtensions.AssertObjectWhenSavedAndRestoredByToString(failedMessage);
		}
	}
}