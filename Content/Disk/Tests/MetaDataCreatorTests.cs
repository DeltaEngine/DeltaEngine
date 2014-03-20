using System.IO;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Content.Disk.Tests
{
	[Category("Slow")]
	public class MetaDataCreatorTests
	{
		//ncrunch: no coverage start
		[TearDown]
		public void DisposeContentLoader()
		{
			ContentLoader.DisposeIfInitialized();
		}

		[Test, Ignore]
		public void TryCreatingAnimationFromFiles()
		{
			File.Delete(Path.Combine("Content", "ContentMetaData.xml"));
			ContentLoader.Use<DiskContentLoader>();
			Assert.IsTrue(ContentLoader.Exists("ImageAnimation", ContentType.ImageAnimation));
		}

		[Test]
		public void IfNamesAreJustNumbersDoNotCreateSequence()
		{
			ContentDiskTestsExtensions.CreateImage(Path.Combine("Content", "1.png"), Size.One);
			ContentDiskTestsExtensions.CreateImage(Path.Combine("Content", "2.png"), Size.One);
			ContentDiskTestsExtensions.CreateImage(Path.Combine("Content", "3.png"), Size.One);
			Assert.DoesNotThrow(ContentLoader.Use<DiskContentLoader>);
		}
	}
}