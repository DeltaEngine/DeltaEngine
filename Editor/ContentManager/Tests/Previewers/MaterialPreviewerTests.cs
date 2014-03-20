using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class MaterialPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			materialPreviewer = new MaterialPreviewer();
		}

		private MaterialPreviewer materialPreviewer;

		[Test]
		public void LoadFontToDisplay()
		{
			materialPreviewer.PreviewContent("DeltaLogo2D");
		}
	}
}