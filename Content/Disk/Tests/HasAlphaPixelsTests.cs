using System.Drawing;
using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Content.Disk.Tests
{
	[Ignore]
	public class HasAlphaPixelsTests
	{
		//ncrunch: no coverage start
		[Test]
		public void DeltaEngineLogoHasAlphaPixels()
		{
			var image = new Bitmap(Path.Combine("Content", "DeltaEngineLogo.png"));
			Assert.IsTrue(ContentMetaDataFileCreator.HasBitmapAlphaPixels(image));
		}

		[Test]
		public void SimpleSubMenuBackgroundHasNoAlphaPixels()
		{
			var image = new Bitmap(Path.Combine("Content", "SimpleSubMenuBackground.png"));
			Assert.IsFalse(ContentMetaDataFileCreator.HasBitmapAlphaPixels(image));
		}

		[Test]
		public void DefaultRadiobuttonOnHasAlphaPixels()
		{
			var image = new Bitmap(Path.Combine("Content", "DefaultRadiobuttonOn.png"));
			Assert.IsTrue(ContentMetaDataFileCreator.HasBitmapAlphaPixels(image));
		}

		[Test]
		public void SmallImageHasNoAlphaPixels()
		{
			var image = new Bitmap(Path.Combine("Content", "SmallImage.png"));
			Assert.IsFalse(ContentMetaDataFileCreator.HasBitmapAlphaPixels(image));
		}
	}
}