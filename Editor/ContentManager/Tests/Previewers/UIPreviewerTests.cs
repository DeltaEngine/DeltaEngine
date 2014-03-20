using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class UIPreviewerTests : TestWithMocksOrVisually
	{
		[Test]
		public void Setup()
		{
			var uiPreviewer = new UIPreviewer();
			uiPreviewer.PreviewContent("SceneWithAButton");
		}
	}
}
