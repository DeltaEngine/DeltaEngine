using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using NUnit.Framework;

namespace DeltaEngine.Scenes.Tests.Controls
{
	public class TextBoxTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			topTextBox = new TextBox(Top, "Hello") { RenderLayer = 2 };
			bottomTextBox = new TextBox(Bottom, "World");
			InitializeKeyboardAndMouse();
		}

		private TextBox topTextBox;
		private TextBox bottomTextBox;
		private static readonly Rectangle Top = Rectangle.FromCenter(0.5f, 0.4f, 0.3f, 0.1f);
		private static readonly Rectangle Bottom = Rectangle.FromCenter(0.5f, 0.6f, 0.3f, 0.1f);

		private void InitializeKeyboardAndMouse()
		{
			keyboard = Resolve<Keyboard>();
			lastKey = Key.None;
			mouse = Resolve<Mouse>();
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private Keyboard keyboard;
		private Key lastKey;
		private Mouse mouse;

		[Test, ApproveFirstFrameScreenshot]
		public void RenderTwoTextBoxes() {}

		[Test, CloseAfterFirstFrame]
		public void DefaultsToEnabled()
		{
			Assert.IsTrue(topTextBox.IsEnabled);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingTextBoxGivesItFocus()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			Assert.IsFalse(topTextBox.State.HasFocus);
			PressAndReleaseMouse(Vector2D.One);
			Assert.IsFalse(topTextBox.State.HasFocus);
			PressAndReleaseMouse(Top.Center);
			Assert.IsTrue(topTextBox.State.HasFocus);
		}

		private void PressAndReleaseMouse(Vector2D position)
		{
			SetMouseState(State.Pressing, position);
			SetMouseState(State.Releasing, position);
			SetMouseState(State.Released, position);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			(mouse as MockMouse).SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingOneTextBoxCausesOtherTextBoxToLoseFocus()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			PressAndReleaseMouse(Top.Center);
			Assert.IsTrue(topTextBox.State.HasFocus);
			Assert.IsFalse(bottomTextBox.State.HasFocus);
			PressAndReleaseMouse(Bottom.Center);
			Assert.IsFalse(topTextBox.State.HasFocus);
			Assert.IsTrue(bottomTextBox.State.HasFocus);
			Assert.AreEqual(Color.Gray, topTextBox.Color);
			Assert.AreEqual(Color.LightGray, bottomTextBox.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void TypingHasNoEffectIfTextBoxDoesNotHaveFocus()
		{
			topTextBox.Text = "";
			PressKey(Key.A);
			Assert.AreEqual("", topTextBox.Text);
		}

		private void PressKey(Key key)
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			var mockKeyboard = keyboard as MockKeyboard;
			if (lastKey != Key.None)
				mockKeyboard.SetKeyboardState(lastKey, State.Pressed);
			mockKeyboard.SetKeyboardState(key, State.Pressing);
			AdvanceTimeAndUpdateEntities();
			lastKey = key;
		}

		[Test, CloseAfterFirstFrame]
		public void TypingGoesIntoTheTextBoxWithFocus()
		{
			if (!IsMockResolver)
				return; //ncrunch: no coverage
			topTextBox.Text = "";
			bottomTextBox.Text = "";
			PressAndReleaseMouse(Bottom.Center);
			PressKeys();
			Assert.AreEqual("", topTextBox.Text);
			Assert.AreEqual("a 2", bottomTextBox.Text);
		}

		private void PressKeys()
		{
			PressKey(Key.A);
			PressKey(Key.Space);
			PressKey(Key.D1);
			PressKey(Key.Backspace);
			PressKey(Key.D2);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderOneEnabledAndOneDisabledTextBox()
		{
			bottomTextBox.IsEnabled = false;
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Color.Gray, topTextBox.Color);
			Assert.AreEqual(Color.DarkGray, bottomTextBox.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ClickingTextBoxDoesNotGivesItFocusIfItIsDisabled()
		{
			topTextBox.IsEnabled = false;
			PressAndReleaseMouse(Top.Center);
			Assert.IsFalse(topTextBox.State.HasFocus);
		}

		[Test, CloseAfterFirstFrame]
		public void TypingDoesNotGoIntoADisabledTextBox()
		{
			bottomTextBox.IsEnabled = false;
			bottomTextBox.Text = "";
			PressAndReleaseMouse(Bottom.Center);
			PressKeys();
			Assert.AreEqual("", bottomTextBox.Text);
		}

		[Test]
		public void RenderTextBoxAttachedToMouse()
		{
			new Command(point => topTextBox.DrawArea = //ncrunch: no coverage
				Rectangle.FromCenter(point, topTextBox.DrawArea.Size)).Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void SaveAndLoad()
		{
			var stream = BinaryDataExtensions.SaveToMemoryStream(topTextBox);
			var loadedTextBox = (TextBox)stream.CreateFromMemoryStream();
			Assert.AreEqual(Top, loadedTextBox.DrawArea);
			Assert.AreEqual("Hello", loadedTextBox.Text);
			Assert.AreEqual(2, loadedTextBox.RenderLayer);
			Assert.IsTrue(loadedTextBox.State.CanHaveFocus);
			Assert.AreEqual(topTextBox.children.Count, loadedTextBox.children.Count);
		}

		[Test]
		public void DrawLoadedTextBox()
		{
			topTextBox.Text = "Original";
			bottomTextBox.IsActive = false;
			var stream = BinaryDataExtensions.SaveToMemoryStream(topTextBox);
			var loadedTextBox = (TextBox)stream.CreateFromMemoryStream();
			loadedTextBox.Text = "Loaded";
			loadedTextBox.DrawArea = loadedTextBox.DrawArea.Move(0.0f, 0.15f);
		}

		[Test]
		public void ChangingFontTextChangesChild()
		{
			var text = new FontText(Font.Default, "Hello", Rectangle.HalfCentered);
			topTextBox.Set(text);
			Assert.AreEqual(text, topTextBox.children[0].Entity2D);
		}

		[Test]
		public void CheckRightKeys()
		{
			var textBox = new TextBox(Rectangle.One, "test");
			textBox.IsEnabled = true;
			textBox.State.HasFocus = true;
			new Command(() => textBox.Text = keyboard.HandleInput(textBox.Text)).Add(
				new KeyTrigger(Key.None, State.Released));
		}
	}
}