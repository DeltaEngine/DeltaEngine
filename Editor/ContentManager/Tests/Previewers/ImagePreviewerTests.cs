using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	internal class ImagePreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			imagePreviewer = new ImagePreviewer();
		}

		private ImagePreviewer imagePreviewer;

		[Test]
		public void LoadImageToDisplay()
		{
			imagePreviewer.PreviewContent("DeltaLogo");
		}

		[Test]
		public void ChangingPreviewImageWillRemovePreviousImage()
		{
			imagePreviewer.PreviewContent("DeltaLogo");
			Assert.AreEqual("<GeneratedMaterial:Position2DTextured:DeltaLogo>",
				imagePreviewer.currentDisplaySprite.Material.Name);
			imagePreviewer.PreviewContent("DeltaLogo2");
			Assert.AreEqual("<GeneratedMaterial:Position2DTextured:DeltaLogo2>",
				imagePreviewer.currentDisplaySprite.Material.Name);
		}
	}
}