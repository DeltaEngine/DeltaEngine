using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class AppBuildProgressTests
	{
		[Test]
		public void AppBuildProgressMessageMustBeRestorable()
		{
			var progressMessage = new AppBuildProgress("I see some progress", 30);
			MessageTestExtensions.AssertObjectWhenSavedAndRestoredByToString(progressMessage);
		}
	}
}