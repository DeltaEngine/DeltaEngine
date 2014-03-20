using System.IO;
using NUnit.Framework;

namespace DeltaEngine.Editor.Emulator.Tests
{
	public class UIToolNamesAndPathsTests
	{
		[SetUp]
		public void Init()
		{
			toolNamesAndPaths = new UIToolNamesAndPaths();
		}

		private UIToolNamesAndPaths toolNamesAndPaths;

		[Test]
		public void CheckNumberOfUIEditorTools()
		{
			Assert.AreEqual(13, toolNamesAndPaths.GetNames().Count);
		}

		[TestCase(UITool.Button, "CreateButton.png")]
		[TestCase(UITool.ProgressBar, "CreateProgressBar.png")]
		[TestCase(UITool.Tilemap, "CreateTilemap.png")]
		public void ToolsShouldHaveACorrectImagePath(UITool uiTool, string expectedFilename)
		{
			Assert.AreEqual(Path.Combine("..", "Images", "UIEditor", expectedFilename),
				toolNamesAndPaths.GetImagePath(uiTool.ToString()));
		}
	}
}