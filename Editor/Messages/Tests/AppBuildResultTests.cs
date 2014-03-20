using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class AppBuildResultTests
	{
		[Test]
		public void AppBuildResultdMessageMustBeRestorable()
		{
			var resultMessage = new AppBuildResult("CoolProject", PlatformName.Windows);
			MessageTestExtensions.AssertObjectWhenSavedAndRestoredByToString(resultMessage);
		}
	}
}