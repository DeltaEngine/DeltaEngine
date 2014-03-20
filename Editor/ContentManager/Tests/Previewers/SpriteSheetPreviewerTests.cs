using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class SpriteSheetPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			spriteSheetPreviewer = new SpriteSheetPreviewer();
			spriteSheetPreviewer.PreviewContent("DeltaEngineLogo");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private SpriteSheetPreviewer spriteSheetPreviewer;
		private MockMouse mockMouse;

		[Test]
		public void MoveCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), spriteSheetPreviewer.currentDisplayAnimation.Center);
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), spriteSheetPreviewer.currentDisplayAnimation.Center);
		}

		[Test]
		public void ZoomCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), spriteSheetPreviewer.currentDisplayAnimation.Center);
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(0.5f, spriteSheetPreviewer.currentDisplayAnimation.DrawArea.Width);
		}

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), spriteSheetPreviewer.currentDisplayAnimation.Center);
			Assert.AreEqual(0.5f, spriteSheetPreviewer.currentDisplayAnimation.DrawArea.Width);
			spriteSheetPreviewer.PreviewContent("DeltaEngineLogo");
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), spriteSheetPreviewer.currentDisplayAnimation.Center);
			Assert.AreEqual(0.5f, spriteSheetPreviewer.currentDisplayAnimation.DrawArea.Width);
		}
	}
}
