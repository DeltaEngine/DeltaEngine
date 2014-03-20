using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class GamePadAnalogTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void MovingSticksTranslatesCircle()
		{
			var ellipseLeft = new Ellipse(new Rectangle(0.4f, 0.5f, 0.1f, 0.1f), Color.Green);
			var ellipseRight = new Ellipse(new Rectangle(0.6f, 0.5f, 0.1f, 0.1f), Color.Blue);
			new Command(pos => ellipseLeft.Center = pos * 0.2f + Vector2D.Half).Add(
				new GamePadAnalogTrigger(GamePadAnalog.LeftThumbStick));
			new Command(pos => ellipseRight.Center = pos * 0.2f + Vector2D.Half).Add(
				new GamePadAnalogTrigger(GamePadAnalog.RightThumbStick));
		}

		[Test]
		public void PressingTriggersUpdatesValues()
		{
			var font = Font.Default;
			var leftTrigger = new FontText(font, "left", new Rectangle(0.2f, 0.5f, 0.2f, 0.2f));
			var rightTrigger = new FontText(font, "right", new Rectangle(0.7f, 0.5f, 0.2f, 0.2f));
			new Command(pos => leftTrigger.Text = "Left Trigger = " + pos.X).Add(
				new GamePadAnalogTrigger(GamePadAnalog.LeftTrigger));
			new Command(pos => rightTrigger.Text = "Right Trigger = " + pos.X).Add(
				new GamePadAnalogTrigger(GamePadAnalog.RightTrigger));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new GamePadAnalogTrigger(GamePadAnalog.RightThumbStick);
			Assert.AreEqual(GamePadAnalog.RightThumbStick, trigger.Analog);
			Assert.AreEqual(Vector2D.Zero, trigger.Position);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new GamePadAnalogTrigger("RightTrigger");
			Assert.AreEqual(GamePadAnalog.RightTrigger, trigger.Analog);
			Assert.AreEqual(Vector2D.Zero, trigger.Position);
			Assert.Throws<GamePadAnalogTrigger.CannotCreateGamePadStickTriggerWithoutGamePadStick>(
				() => new GamePadAnalogTrigger(""));
		}
	}
}