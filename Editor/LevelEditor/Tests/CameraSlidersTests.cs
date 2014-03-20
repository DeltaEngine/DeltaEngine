using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Shapes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Editor.LevelEditor.Tests
{
	public class CameraSlidersTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			ScreenSpace.Current = new Camera2DScreenSpace(Resolve<Window>());
			renderer = new LevelDebugRenderer(new Level(new Size(24, 24)));
		}

		private LevelDebugRenderer renderer;

		[Test]
		public void CanCreateSlidersAndShow()
		{
			var cameraSliders = new CameraSliders(renderer);
			cameraSliders.CreateSliders();
			cameraSliders.Show();
			cameraSliders.horizontalSlider.Value = 10;
			cameraSliders.verticalSlider.Value = 13;
			cameraSliders.SetCameraPosition();
			cameraSliders.CalculateSliderPositionAndScale();
		}

		[Test, CloseAfterFirstFrame]
		public void ValueChangedEventSetsCameraPositionAndDrawsSlider()
		{
			var cameraSliders = new CameraSlidersMock(renderer);
			cameraSliders.CreateSliders();
			cameraSliders.Show();
			int sliderValue = -1;
			cameraSliders.horizontalSlider.ValueChanged += value => sliderValue = value;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(cameraSliders.horizontalSlider.Value, sliderValue);
		}

		private class CameraSlidersMock : CameraSliders
		{
			public CameraSlidersMock(LevelDebugRenderer renderer)
				: base(renderer) {}

			public override void CreateSliders()
			{
				horizontalSlider = new MockSlider(new Rectangle(0, 0, 1, 1));
				verticalSlider = new MockSlider(new Rectangle(0, 0, 1, 1)) { Rotation = 90 };
				horizontalSlider.ValueChanged += value => {};
				verticalSlider.ValueChanged += value => {};
			}
		}

		private class MockSlider : Slider
		{
			public MockSlider(Rectangle drawArea)
				: base(drawArea) {}

			protected override void UpdatePointerValue()
			{
				Value = 5;
				if (ValueChanged != null)
					ValueChanged(Value);
			}
		}
	}
}