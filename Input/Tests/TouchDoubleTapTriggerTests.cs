using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchDoubleTapTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnTouch()
		{
			new FontText(Font.Default, "Touch screen to show red circle", Rectangle.One);
			var ellipse = new Ellipse(new Rectangle(0.1f, 0.1f, 0.1f, 0.1f), Color.Red);
			new Command(() => ellipse.Center = Vector2D.Half).Add(new TouchDoubleTapTrigger());
			new Command(() => ellipse.Center = Vector2D.Zero).Add(new TouchPressTrigger(State.Released));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			Assert.DoesNotThrow(() => new TouchDoubleTapTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			Assert.DoesNotThrow(() => new TouchDoubleTapTrigger(""));
			Assert.Throws<TouchDoubleTapTrigger.TouchDoubleTapTriggerHasNoParameters>(
				() => new TouchDoubleTapTrigger("Right"));
		}

		[Test]
		public void InvokeDoubleTap()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			var trigger = new TouchDoubleTapTrigger();
			bool wasInvoked = false;
			new Command(() => wasInvoked = true).Add(trigger);
			touch.SetTouchState(0, State.Pressing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			touch.SetTouchState(0, State.Releasing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			touch.SetTouchState(0, State.Pressing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			touch.SetTouchState(0, State.Releasing, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(wasInvoked);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateWithStringParameter()
		{
			Assert.DoesNotThrow(() => new TouchDoubleTapTrigger(""));
			Assert.Throws<TouchDoubleTapTrigger.TouchDoubleTapTriggerHasNoParameters>(
				() => new TouchDoubleTapTrigger("NonEmptyString"));
		}
	}
}