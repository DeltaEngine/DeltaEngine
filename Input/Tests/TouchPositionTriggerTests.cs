using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchPositionTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnTouchWithMovement()
		{
			var ellipse = new Ellipse(new Rectangle(-0.1f, -0.1f, 0.1f, 0.1f), Color.Red);
			new Command(pos => ellipse.Center = pos).Add(new TouchPositionTrigger(State.Pressed));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new TouchPositionTrigger(State.Pressed);
			Assert.AreEqual(State.Pressed, trigger.State);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new TouchPositionTrigger("Pressed");
			Assert.AreEqual(State.Pressed, trigger.State);
			trigger = new TouchPositionTrigger("");
			Assert.AreEqual(State.Pressing, trigger.State);
		}

		[Test]
		public void InvokeTouch()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			var trigger = new TouchPositionTrigger(State.Pressed);
			bool wasInvoked = false;
			new Command(() => wasInvoked = true).Add(trigger);
			touch.SetTouchState(0, State.Pressed, Vector2D.Half);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(wasInvoked);
		}

		[Test]
		public void TouchPositionTriggerState()
		{
			var trigger = new TouchPositionTrigger(null);
			Assert.AreEqual(State.Pressing, trigger.State);
		}
	}
}