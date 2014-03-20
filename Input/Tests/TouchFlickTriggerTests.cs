using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchFlickTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnFlick()
		{
			new FontText(Font.Default, "Flick screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new TouchFlickTrigger());
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new TouchPressTrigger(State.Released));
		}

		[Test, CloseAfterFirstFrame]
		public void FlickDetection()
		{
			var trigger = new TouchFlickTrigger();
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			bool flickHappened = false;
			trigger.Invoked += () => flickHappened = true;
			AdvanceTouchTick(touch, State.Pressing, new Vector2D(0.5f, 0.5f), trigger);
			Assert.IsFalse(flickHappened);
			Assert.AreEqual(0, trigger.PressTime);
			AdvanceTouchTick(touch, State.Pressed, new Vector2D(0.52f, 0.5f), trigger);
			Assert.IsFalse(flickHappened);
			AdvanceTouchTick(touch, State.Releasing, new Vector2D(0.6f, 0.5f), trigger);
			Assert.IsTrue(flickHappened);
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), trigger.StartPosition);
		}

		private void AdvanceTouchTick(MockTouch touch, State state, Vector2D position,
			TouchFlickTrigger trigger)
		{
			touch.SetTouchState(0, state, position);
			AdvanceTimeAndUpdateEntities();
			touch.Update(new[] { trigger });
		}

		[Test, CloseAfterFirstFrame]
		public void Creation()
		{
			Assert.DoesNotThrow(() => new TouchFlickTrigger(""));
			Assert.DoesNotThrow(() => new TouchFlickTrigger(null));
			Assert.Throws<TouchFlickTrigger.TouchFlickTriggerHasNoParameters>(
				() => new TouchFlickTrigger("Left"));
		}
	}
}