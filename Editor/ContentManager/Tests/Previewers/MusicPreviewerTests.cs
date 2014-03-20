using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class MusicPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			musicPreviewer = new MusicPreviewer();
			musicPreviewer.PreviewContent("Music");
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private MusicPreviewer musicPreviewer;
		private MockMouse mockMouse;

		[Test]
		public void MoveCamera()
		{
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(musicPreviewer.music.IsPlaying());
		}
	}
}
