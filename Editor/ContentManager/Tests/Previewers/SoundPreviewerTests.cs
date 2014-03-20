using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	public class SoundPreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreatePreviewer()
		{
			soundPreviewer = new SoundPreviewer();
			mockMouse = Resolve<MockMouse>();
		}

		private SoundPreviewer soundPreviewer;
		private MockMouse mockMouse;

		[Test]
		public void PlaySound()
		{
			soundPreviewer.PreviewContent("Sound");
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, soundPreviewer.Sound.NumberOfInstances);
			Assert.AreEqual(1, soundPreviewer.Sound.NumberOfPlayingInstances);
		}

		[Test]
		public void PlayingSecondSoundInstanceDisposesOfTheFirstInstance()
		{
			soundPreviewer.PreviewContent("Sound");
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			soundPreviewer.PreviewContent("Sound");
			mockMouse.SetButtonState(MouseButton.Left, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(1, soundPreviewer.Sound.NumberOfInstances);
			Assert.AreEqual(1, soundPreviewer.Sound.NumberOfPlayingInstances);
		}
	}
}
