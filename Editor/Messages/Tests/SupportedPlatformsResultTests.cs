using DeltaEngine.Core;
using NUnit.Framework;

namespace DeltaEngine.Editor.Messages.Tests
{
	public class PlatformNameTests
	{
		[Test]
		public void UnsupportedPlatformFileExtensionShouldCrash()
		{
			Assert.Throws<PlatformNameExtensions.UnsupportedPlatformPackageFileExtension>(
				() => PlatformNameExtensions.GetPlatformFromFileExtension("jpg"));
		}

		[Test]
		public void UnsupportedPlatformHasNoFileExtension()
		{
			Assert.AreEqual(".none", PlatformNameExtensions.GetAppExtension((PlatformName)(-1)));
		}

		[TestCase(PlatformName.Windows, ".exe")]
		[TestCase(PlatformName.Windows8, ".appx")]
		[TestCase(PlatformName.WindowsPhone7, ".xap")]
		[TestCase(PlatformName.Android, ".apk")]
		[TestCase(PlatformName.IOS, ".ipa")]
		[TestCase(PlatformName.Web, ".zip")]
		public void PlatformShouldUseCorrectFileExtension(PlatformName platform, string extension)
		{
			Assert.AreEqual(platform, PlatformNameExtensions.GetPlatformFromFileExtension(extension));
			Assert.AreEqual(extension, PlatformNameExtensions.GetAppExtension(platform));
		}
		
		[Test]
		public void SaveAndLoadPlatformsResult()
		{
			var result = new SupportedPlatformsResult(new[] { PlatformName.Android, PlatformName.Web });
			var stream = BinaryDataExtensions.SaveDataIntoMemoryStream(result);
			var loaded = BinaryDataExtensions.
				LoadDataWithKnownTypeFromMemoryStream<SupportedPlatformsResult>(stream);
			Assert.AreEqual(2, loaded.Platforms.Length);
		}
	}
}