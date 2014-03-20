using System.Collections.Generic;
using DeltaEngine.Commands;
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
	public class ScrollbarTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			center = Rectangle.FromCenter(0.5f, 0.5f, 0.5f, 0.1f);
			scrollbar = new Scrollbar(center);
			scrollbar.Add(new FontText(Font.Default, "", new Rectangle(0.5f, 0.7f, 0.2f, 0.1f)));
			scrollbar.Start<DisplayScrollbarValue>();
			InitializeMouse();
		}

		private static Rectangle center;
		private Scrollbar scrollbar;

		private class DisplayScrollbarValue : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Scrollbar scrollbar in entities)
					scrollbar.Get<FontText>().Text = scrollbar.LeftValue + " - " + scrollbar.RightValue;
			}
		}

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private MockMouse mouse;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderScrollbarZeroToOneHundredWithPointerWidthTwenty()
		{
			scrollbar.ValueWidth = 20;
		}

		[Test]
		public void RenderScrollbarWithValueWidthEqualingValues()
		{
			scrollbar.MaxValue = 2;
			scrollbar.ValueWidth = 3;
		}

		[Test]
		public void RenderScrollbarWithOneMoreValueThanValueWidth()
		{
			scrollbar.MaxValue = 3;
			scrollbar.ValueWidth = 3;
		}

		[Test, CloseAfterFirstFrame]
		public void DefaultsToEnabled()
		{
			Assert.IsTrue(scrollbar.IsEnabled);
			Assert.AreEqual(Color.Gray, scrollbar.Color);
			Assert.AreEqual(Color.LightGray, scrollbar.Pointer.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void DisablingScrollbarDisablesPointer()
		{
			scrollbar.IsEnabled = false;
			Assert.IsFalse(scrollbar.IsEnabled);
			Assert.IsFalse(scrollbar.Pointer.IsEnabled);
		}

		[Test]
		public void RenderDisabledScrollbar()
		{
			scrollbar.IsEnabled = false;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Color.DarkGray, scrollbar.Color);
			Assert.AreEqual(Color.Gray, scrollbar.Pointer.Color);
		}

		[Test]
		public void RenderScrollbarZeroToOneThousandWithPointerWidthFiveHundred()
		{
			scrollbar.MaxValue = 1000;
			scrollbar.ValueWidth = 500;
		}

		[Test]
		public void RenderGrowingScrollbar()
		{
			scrollbar.Start<Grow>();
		}

		[Test, CloseAfterFirstFrame]
		public void DefaultValues()
		{
			Assert.AreEqual(0, scrollbar.MinValue);
			Assert.AreEqual(99, scrollbar.MaxValue);
			Assert.AreEqual(10, scrollbar.ValueWidth);
			Assert.AreEqual(90, scrollbar.LeftValue);
			Assert.AreEqual(95, scrollbar.CenterValue);
			Assert.AreEqual(99, scrollbar.RightValue);
		}

		[Test, CloseAfterFirstFrame]
		public void UpdateValues()
		{
			scrollbar.MinValue = 1;
			scrollbar.MaxValue = 10;
			scrollbar.ValueWidth = 2;
			scrollbar.CenterValue = 4;
			Assert.AreEqual(1, scrollbar.MinValue);
			Assert.AreEqual(10, scrollbar.MaxValue);
			Assert.AreEqual(4, scrollbar.CenterValue);
			Assert.AreEqual(2, scrollbar.ValueWidth);
		}

		[Test, CloseAfterFirstFrame]
		public void ValidatePointerSize()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Assert.AreEqual(new Size(0.05f, 0.1f), scrollbar.Pointer.DrawArea.Size);
		}

		[Test, CloseAfterFirstFrame]
		public void ValidatePointerCenter()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			var position = new Vector2D(0.3f, 0.52f);
			DragMouse(position);
			Assert.AreEqual(new Vector2D(0.3f, 0.5f), scrollbar.Pointer.DrawArea.Center);
		}

		private void DragMouse(Vector2D position)
		{
			SetMouseState(State.Pressing, position + new Vector2D(0.1f, 0.1f));
			SetMouseState(State.Pressing, position);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void RenderVerticalScrollbar()
		{
			scrollbar.MaxValue = 9;
			scrollbar.ValueWidth = 3;
			scrollbar.Rotation = 90;
		}

		[Test]
		public void Render45DegreeScrollbar()
		{
			scrollbar.Rotation = 45;
		}

		[Test]
		public void RenderSpinningScrollbar()
		{
			scrollbar.Start<Spin>();
		}

		[Test]
		public void RenderSpinningScrollbarAttachedToMouse()
		{
			scrollbar.Start<Spin>();
			new Command(
				point => scrollbar.DrawArea =  //ncrunch: no coverage
					Rectangle.FromCenter(point, scrollbar.DrawArea.Size)).Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			scrollbar.CenterValue = 20;
			var stream = BinaryDataExtensions.SaveToMemoryStream(scrollbar);
			var loadedScrollbar = (Scrollbar)stream.CreateFromMemoryStream();
			Assert.AreEqual(center, loadedScrollbar.DrawArea);
			Assert.AreEqual(20, loadedScrollbar.CenterValue);
			Assert.AreEqual(scrollbar.Get<Picture>().Material.DefaultColor,
				loadedScrollbar.Get<Picture>().Material.DefaultColor);
		}

		[Test]
		public void DrawLoadedScrollbar()
		{
			scrollbar.CenterValue = 50;
			var stream = BinaryDataExtensions.SaveToMemoryStream(scrollbar);
			scrollbar.IsActive = false;
			stream.CreateFromMemoryStream();
		}

		[Test]
		public void ChangingPointerChangesChild()
		{
			var pointer = new Picture(new Theme(), new Theme().ScrollbarPointer, Rectangle.Unused);
			scrollbar.Set(pointer);
			Assert.AreEqual(pointer, scrollbar.children[0].Entity2D);
		}
	}
}