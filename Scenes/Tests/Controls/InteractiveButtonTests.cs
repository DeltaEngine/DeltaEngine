using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class InteractiveButtonTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			button = new InteractiveButton(Center, "Click Me");
			InitializeMouse();
			InitializeTouch();
			AdvanceTimeAndUpdateEntities();
		}

		private InteractiveButton button;
		private static readonly Size BaseSize = new Size(0.3f, 0.1f);
		private static readonly Rectangle Center = Rectangle.FromCenter(Vector2D.Half, BaseSize);

		private void InitializeMouse()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse != null)
				mouse.SetNativePosition(Vector2D.Zero);
		}

		private MockMouse mouse;

		private void InitializeTouch()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch != null)
				touch.SetTouchState(0, State.Released, Vector2D.Zero);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderInteractiveButton() {}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderDisabledInteractiveButton()
		{
			button.IsEnabled = false;
		}

		[Test, CloseAfterFirstFrame]
		public void DefaultsToEnabled()
		{
			Assert.IsTrue(button.IsEnabled);
		}

		[Test, CloseAfterFirstFrame]
		public void BeginningClickMakesItShrink()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			SetMouseState(State.Pressing, Vector2D.Half);
			Assert.IsTrue(button.Size.Width < BaseSize.Width);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void FinishingClickMakesItGrow()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			SetMouseState(State.Pressing, Vector2D.Half);
			SetMouseState(State.Releasing, Vector2D.Half);
			Assert.IsTrue(button.Size.Width > BaseSize.Width);
		}

		[Test, CloseAfterFirstFrame]
		public void EnteringMakesItGrow()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			SetMouseState(State.Released, Vector2D.Half);
			Assert.IsTrue(button.Size.Width > BaseSize.Width);
		}

		[Test, CloseAfterFirstFrame]
		public void ExitingMakesItNormalize()
		{
			SetMouseState(State.Released, Vector2D.Half);
			SetMouseState(State.Released, new Vector2D(0.0f, 0.22f));
			Assert.AreEqual(BaseSize.Width, button.Size.Width, 0.05f);
			Assert.AreEqual(BaseSize.Height, button.Size.Height, 0.05f);
		}

		[Test, CloseAfterFirstFrame]
		public void BeginningClickDoesNothingIfDisabled()
		{
			button.IsEnabled = false;
			SetMouseState(State.Pressing, Vector2D.Half);
			Assert.AreEqual(BaseSize.Width, button.Size.Width, 0.05f);
		}

		[Test]
		public void RenderInteractiveButtonAttachedToMouse()
		{
			new Command(point => button.DrawArea = Rectangle.FromCenter(point, button.DrawArea.Size)).
				Add(new MouseMovementTrigger());
		}

		[Test]
		public void DrawLoadedButton()
		{
			button.Text = "Original";
			var stream = BinaryDataExtensions.SaveToMemoryStream(button);
			var loadedButton = (InteractiveButton)stream.CreateFromMemoryStream();
			loadedButton.Text = "Loaded";
			loadedButton.DrawArea = loadedButton.DrawArea.Move(0.0f, 0.15f);
		}
		
		[Test, CloseAfterFirstFrame, Ignore] //ncrunch: no coverage start
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(button);
			var loadedButton = (InteractiveButton)stream.CreateFromMemoryStream();
			Assert.AreEqual(Center, loadedButton.DrawArea);
			Assert.AreEqual("Click Me", loadedButton.Text);
		}

		[Test, CloseAfterFirstFrame, Ignore]
		public void LoadWithoutBinaryDataExtensions()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(button);
			button.Text = "Original";
			var loadedButton = new InteractiveButton();
			loadedButton.LoadFromStream(stream);
			Assert.AreEqual(Center, loadedButton.DrawArea);
			Assert.AreEqual("Click Me", loadedButton.Text);
			loadedButton.DrawArea = loadedButton.DrawArea.Move(new Vector2D(0.0f, 0.2f));
		}
	}
}