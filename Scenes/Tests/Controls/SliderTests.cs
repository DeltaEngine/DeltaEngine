using System.Collections.Generic;
using System.Globalization;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class SliderTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);
			slider = new Slider(center);
			slider.Add(new FontText(Font.Default, "", new Rectangle(0.5f, 0.7f, 0.2f, 0.1f)));
			slider.Start<DisplaySliderValue>();
			InitializeMouse();
		}

		private Slider slider;
		private static Rectangle center;

		private class DisplaySliderValue : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Slider slider in entities)
					slider.Get<FontText>().Text = slider.Value.ToString(CultureInfo.InvariantCulture);
			}
		}

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>();
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private Mouse mouse;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderSliderZeroToOneHundred() {}

		[Test, CloseAfterFirstFrame]
		public void DefaultProperties()
		{
			Assert.IsTrue(slider.IsEnabled);
			Assert.AreEqual(Color.Gray, slider.Color);
			Assert.AreEqual(Color.LightGray, slider.Pointer.Color);
			Assert.AreEqual(0, slider.MinValue);
			Assert.AreEqual(100, slider.Value);
			Assert.AreEqual(100, slider.MaxValue);
		}

		[Test, CloseAfterFirstFrame]
		public void DisablingSliderDisablesPointer()
		{
			slider.IsEnabled = false;
			Assert.IsFalse(slider.IsEnabled);
			Assert.IsFalse(slider.Pointer.IsEnabled);
		}

		[Test]
		public void RenderDisabledSlider()
		{
			slider.IsEnabled = false;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Color.DarkGray, slider.Color);
			Assert.AreEqual(Color.Gray, slider.Pointer.Color);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderSliderMinusFiveToFive()
		{
			slider.MinValue = -5;
			slider.Value = 0;
			slider.MaxValue = 5;
		}

		[Test]
		public void RenderGrowingSlider()
		{
			slider.Start<Grow>();
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateValues()
		{
			slider.MinValue = 1;
			slider.Value = 2;
			slider.MaxValue = 3;
			Assert.AreEqual(1, slider.MinValue);
			Assert.AreEqual(2, slider.Value);
			Assert.AreEqual(3, slider.MaxValue);
		}

		[Test, CloseAfterFirstFrame]
		public void ValidatePointerSize()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var pointer = new Theme().SliderPointer;
			var width = pointer.MaterialRenderSize.AspectRatio * 0.1f;
			var pointerSize = new Size(width, 0.1f);
			Assert.AreEqual(pointerSize, slider.Pointer.DrawArea.Size);
		}

		[Test, CloseAfterFirstFrame]
		public void ValidatePointerCenter()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			DragMouse(new Vector2D(0.42f, 0.52f));
			var pointerCenter = slider.Pointer.DrawArea.Center;
			Assert.IsTrue(pointerCenter.IsNearlyEqual(new Vector2D(0.424f, 0.5f)),
				pointerCenter.ToString());
		}

		private void DragMouse(Vector2D position)
		{
			SetMouseState(State.Pressing, position + new Vector2D(0.01f, 0.01f));
			SetMouseState(State.Pressing, position);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			mouse.SetNativePosition(position);
			(mouse as MockMouse).SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void RenderVerticalSlider()
		{
			slider.Rotation = 90;
		}

		[Test]
		public void Render45DegreeSlider()
		{
			slider.Rotation = 45;
		}

		[Test]
		public void RenderSpinningSlider()
		{
			slider.Start<Spin>();
		}

		[Test]
		public void RenderSpinningSliderAttachedToMouse()
		{
			slider.Start<Spin>();
			new Command(point => slider.DrawArea = Rectangle.FromCenter(point, slider.DrawArea.Size)).
				Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void VerifyValueChangedEvent()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			int sliderValue = -1;
			slider.ValueChanged += value => sliderValue = value;
			var position = new Vector2D(0.42f, 0.52f);
			DragMouse(position);
			Assert.AreEqual(slider.Value, sliderValue);
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			slider.MinValue = 1;
			slider.Value = 2;
			slider.MaxValue = 3;
			var stream = BinaryDataExtensions.SaveToMemoryStream(slider);
			var loadedSlider = (Slider)stream.CreateFromMemoryStream();
			Assert.AreEqual(center, loadedSlider.DrawArea);
			Assert.AreEqual(1, loadedSlider.MinValue);
			Assert.AreEqual(2, loadedSlider.Value);
			Assert.AreEqual(3, loadedSlider.MaxValue);
			Assert.AreEqual(slider.Get<Picture>().Material.DefaultColor,
				loadedSlider.Get<Picture>().Material.DefaultColor);
		}

		[Test]
		public void DrawLoadedSlider()
		{
			slider.Value = 70;
			var stream = BinaryDataExtensions.SaveToMemoryStream(slider);
			slider.IsActive = false;
			stream.CreateFromMemoryStream();
		}

		[Test, CloseAfterFirstFrame]
		public void LoadSceneWithASlider()
		{
			var loadedScene = ContentLoader.Load<Scene>("SceneWithASlider");
			var loadedslider = loadedScene.Controls[0] as Slider;
			Assert.AreEqual(2, loadedslider.GetActiveBehaviors().Count);
		}
	}
}