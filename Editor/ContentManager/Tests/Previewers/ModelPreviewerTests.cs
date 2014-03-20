using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class ModelPreviewerTests : TestWithMocksOrVisually
	{
		[Test]
		public void SetupMesh()
		{
			var meshPreviewer = new ModelPreviewer();
			meshPreviewer.PreviewContent("TestModel");
		}
	}
}
