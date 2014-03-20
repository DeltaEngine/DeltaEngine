using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadTests : TestWithMocksOrVisually
	{
		[Test]
		public void PressingGamePadButtonShowsCircle()
		{
			new FontText(Font.Default, "Press X on GamePad to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new GamePadButtonTrigger(
				GamePadButton.X, State.Pressed));
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new GamePadButtonTrigger(
				GamePadButton.X, State.Released));
		}

		[Test]
		public void VibrateOnButtonPress()
		{
			var gamePad = Resolve<GamePad>();
			new FontText(Font.Default, "Press X on GamePad to vibrate", Rectangle.One);
			new Command(() => gamePad.Vibrate(1f)).Add(new GamePadButtonTrigger(
				GamePadButton.X, State.Pressed));
			new Command(() => gamePad.Vibrate(0f)).Add(new GamePadButtonTrigger(
				GamePadButton.X, State.Released));
		}

		[Test, CloseAfterFirstFrame]
		public void TestGamePadButtonPress()
		{
			bool isPressed = false;
			new Command(() => isPressed = true).Add(new GamePadButtonTrigger(GamePadButton.A,
				State.Pressed));
			Assert.IsFalse(isPressed);
			var mockGamePad = Resolve<GamePad>() as MockGamePad;
			if (mockGamePad == null)
				return; //ncrunch: no coverage
			mockGamePad.SetGamePadState(GamePadButton.A, State.Pressed);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(isPressed);
			Assert.IsTrue(mockGamePad.IsAvailable);
		}

		[Test]
		public void CheckGamePadIsAvailable()
		{
			var gamePad = Resolve<GamePad>();
			if (gamePad is MockGamePad)
				((MockGamePad)gamePad).SetUnavailable();
			Assert.IsFalse(gamePad.IsAvailable);
		}
	}
}