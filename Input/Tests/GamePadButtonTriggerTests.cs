using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadButtonTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void PressGamePadButtonsToShowCircles()
		{
			var circleA = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Green);
			var circleB = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			var circleX = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Blue);
			var circleY = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Yellow);
			new Command(() => circleA.Center = new Vector2D(0.5f, 0.75f)).Add(
				new GamePadButtonTrigger(GamePadButton.A, State.Pressed));
			new Command(() => circleA.Center = Vector2D.Zero).Add(new GamePadButtonTrigger(
				GamePadButton.A, State.Released));
			new Command(() => circleB.Center = new Vector2D(0.75f, 0.5f)).Add(
				new GamePadButtonTrigger(GamePadButton.B, State.Pressed));
			new Command(() => circleB.Center = Vector2D.Zero).Add(new GamePadButtonTrigger(
				GamePadButton.B, State.Released));
			new Command(() => circleX.Center = new Vector2D(0.25f, 0.5f)).Add(
				new GamePadButtonTrigger(GamePadButton.X, State.Pressed));
			new Command(() => circleX.Center = Vector2D.Zero).Add(new GamePadButtonTrigger(
				GamePadButton.X, State.Released));
			new Command(() => circleY.Center = new Vector2D(0.5f, 0.25f)).Add(
				new GamePadButtonTrigger(GamePadButton.Y, State.Pressed));
			new Command(() => circleY.Center = Vector2D.Zero).Add(new GamePadButtonTrigger(
				GamePadButton.Y, State.Released));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new GamePadButtonTrigger(GamePadButton.Y, State.Pressed);
			Assert.AreEqual(GamePadButton.Y, trigger.Button);
			Assert.AreEqual(State.Pressed, trigger.State);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new GamePadButtonTrigger("Y Pressed");
			Assert.AreEqual(GamePadButton.Y, trigger.Button);
			Assert.AreEqual(State.Pressed, trigger.State);
			Assert.Throws<GamePadButtonTrigger.CannotCreateGamePadTriggerWithoutKey>(
				() => new GamePadButtonTrigger(""));
		}
	}
}