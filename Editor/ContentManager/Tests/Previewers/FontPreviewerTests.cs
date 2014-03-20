using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	internal class FontPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void LoadFontToDisplay()
		{
			fontPreviewer = new FontPreviewer();
			fontPreviewer.PreviewContent("Verdana12");
		}

		private FontPreviewer fontPreviewer;

		[Test]
		public void MoveFont()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
		}

		[Test]
		public void SettingNewFontSetsDefaultPosition()
		{
			var mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
			fontPreviewer.PreviewContent("DeltaEngineLogo");
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), fontPreviewer.currentDisplayText.Center);
		}
	}
}