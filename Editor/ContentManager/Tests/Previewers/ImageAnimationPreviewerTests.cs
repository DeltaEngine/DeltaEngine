using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ImageAnimationPreviewerTests : TestWithMocksOrVisually
	{
		[Test]
		public void Setup()
		{
			imageAnimationPreviewer = new ImageAnimationPreviewer();
			imageAnimationPreviewer.PreviewContent("ImageAnimation");
		}

		private ImageAnimationPreviewer imageAnimationPreviewer;
	}
}