using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class AppBuildRequestTests
	{
		[Test]
		public void NoSolutionFileNameSpecifiedException()
		{
			Assert.Throws<AppBuildRequest.NoSolutionFileNameSpecified>(
				() => new AppBuildRequest(null, null, PlatformName.Windows, null));
			Assert.Throws<AppBuildRequest.NoSolutionFileNameSpecified>(
				() => new AppBuildRequest("MySolution", null, PlatformName.Windows, null));
		}

		[Test]
		public void NoProjectNameSpecifiedException()
		{
			Assert.Throws<AppBuildRequest.NoProjectNameSpecified>(
				() => new AppBuildRequest("MySolution.sln", null, PlatformName.Windows, null));
		}

		[Test]
		public void NoPackedCodeDataSpecifiedException()
		{
			Assert.Throws<AppBuildRequest.NoPackedCodeDataSpecified>(
				() => new AppBuildRequest("MySolution.sln", "MyApp", PlatformName.Windows, null));
			Assert.Throws<AppBuildRequest.NoPackedCodeDataSpecified>(
				() => new AppBuildRequest("MySolution.sln", "MyApp", PlatformName.Windows, new byte[0]));
		}

		[Test]
		public void ValidBuildRequestDoesNotThrowException()
		{
			Assert.DoesNotThrow(() => CreateValidBuildRequestWithDummyByte());
		}

		private static AppBuildRequest CreateValidBuildRequestWithDummyByte()
		{
			return new AppBuildRequest("MyApp.sln", "MyApp", PlatformName.Windows, new byte[1]);
		}

		[Test]
		public void AppBuildRequestMessageMustBeRestorable()
		{
			AppBuildRequest requestMessage = CreateValidBuildRequestWithDummyByte();
			MessageTestExtensions.AssertObjectWhenSavedAndRestoredByToString(requestMessage);
		}
	}
}