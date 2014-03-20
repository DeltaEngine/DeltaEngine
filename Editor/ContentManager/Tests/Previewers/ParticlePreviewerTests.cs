using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.ContentManager.Previewers;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Editor.ContentManager.Tests.Previewers
{
	internal class ParticlePreviewerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Setup()
		{
			particlePreviewer = new ParticlePreviewer();
			new Camera2DScreenSpace(Resolve<Window>());
			mockMouse = Resolve<Mouse>() as MockMouse;
			AdvanceTimeAndUpdateEntities();
		}

		private ParticlePreviewer particlePreviewer;
		private MockMouse mockMouse;

		[Test]
		public void SettingNewImageCreatesNewSizeAndPosition()
		{
			particlePreviewer.PreviewContent("TestParticle");
			mockMouse.SetButtonState(MouseButton.Left, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			mockMouse.SetButtonState(MouseButton.Middle, State.Pressed);
			mockMouse.SetNativePosition(new Vector2D(1f, 1f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
			particlePreviewer.PreviewContent("TestParticle");
			Assert.AreEqual(new Vector3D(0.5f, 0.5f, 0),
				particlePreviewer.currentDisplayParticle2D.Position);
		}

		[Test]
		public void CreateDifferent3DParticles()
		{
			particlePreviewer.PreviewContent("PointEmitter3D");
			particlePreviewer.PreviewContent("LineEmitter3D");
			particlePreviewer.PreviewContent("BoxEmitter3D");
			particlePreviewer.PreviewContent("SphericalEmitter3D");
		}
	}
}