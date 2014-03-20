using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class AppBuildMessageTests
	{
		[Test]
		public void AppBuildMessageMustBeRestorable()
		{
			var buildMessage = new AppBuildMessage("I see compiled code");
			MessageTestExtensions.AssertObjectWhenSavedAndRestoredByToString(buildMessage);
		}
	}
}