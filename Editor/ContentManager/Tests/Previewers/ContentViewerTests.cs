using DeltaEngine.Content;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ContentViewerTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void PreviewImage()
		{
			var contentViewer = new MockContentViewer();
			contentViewer.View("DeltaEngineLogo", ContentType.Image);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Sprite>().Count);
		}

		[Test, CloseAfterFirstFrame]
		public void CannotPreviewContentTypeIfNoPreviewerIsAvailable()
		{
			var contentViewer = new MockContentViewer();
			Assert.Throws<MockContentViewer.PreviewerNotAvailable>(
				() => contentViewer.View("Position2DUv", ContentType.Shader));
		}

		[Test]
		public void ShowNoPreviewTextIfNoPreviewerIsAvailable()
		{
			var contentViewer = new ContentViewer();
			contentViewer.View("Position2DUv", ContentType.Shader);
		}
	}
}