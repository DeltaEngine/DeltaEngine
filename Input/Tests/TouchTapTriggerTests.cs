using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchTapTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnTap()
		{
			new FontText(Font.Default, "Tap screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new TouchTapTrigger());
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new TouchPressTrigger(State.Released));
		}

		[Test]
		public void InvokeTouch()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			var trigger = new TouchTapTrigger();
			bool wasInvoked = false;
			new Command(() => wasInvoked = true).Add(trigger);
			touch.SetTouchState(0, State.Pressing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			touch.SetTouchState(0, State.Releasing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(wasInvoked);
		}

		[Test]
		public void TouchTapTrigger()
		{
			Assert.Throws<TouchTapTrigger.TouchTapTriggerHasNoParameters>(
				() => new TouchTapTrigger("NotAnEmptyString"));
		}
	}
}