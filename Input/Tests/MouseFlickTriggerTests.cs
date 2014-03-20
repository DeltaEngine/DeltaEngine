using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseFlickTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnFlick()
		{
			new FontText(Font.Default, "Flick screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new MouseFlickTrigger());
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new MouseButtonTrigger(State.Released));
		}

		[Test]
		public void FlickWithMouse()
		{
			var trigger = new MouseFlickTrigger();
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			bool flickHappened = false;
			trigger.Invoked += () => flickHappened = true;
			AdvanceMouseTick(mouse, State.Pressing, new Vector2D(0.5f, 0.5f));
			Assert.IsFalse(flickHappened);
			Assert.AreEqual(0.0f, trigger.PressTime);
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), trigger.StartPosition);
			AdvanceMouseTick(mouse, State.Pressed, new Vector2D(0.52f, 0.5f));
			Assert.IsFalse(flickHappened);
			AdvanceMouseTick(mouse, State.Releasing, new Vector2D(0.8f, 0.8f));
			Assert.IsTrue(flickHappened);
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), trigger.StartPosition);
		}

		private void AdvanceMouseTick(MockMouse mouse, State state, Vector2D position)
		{
			mouse.SetButtonState(MouseButton.Left, state);
			mouse.SetNativePosition(position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test]
		public void Creation()
		{
			Assert.DoesNotThrow(() => new MouseFlickTrigger(""));
			Assert.DoesNotThrow(() => new MouseFlickTrigger(null));
			Assert.Throws<MouseFlickTrigger.MouseFlickTriggerHasNoParameters>(
				() => new MouseFlickTrigger("Left"));
		}
	}
}