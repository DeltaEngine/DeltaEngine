using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchDualDragTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void DragTouchToCreateRectangles()
		{
			MouseDragTriggerTests.DragToCreateRectangles(new TouchDualDragTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void DragTouch()
		{
			var touch = Resolve<Touch>() as MockTouch;
			if (touch == null)
				return; //ncrunch: no coverage
			bool isFinished = false;
			new Command((start, end, done) => isFinished = done).Add(new TouchDualDragTrigger());
			SetTouchState(touch, State.Pressing, Vector2D.Zero);
			SetTouchState(touch, State.Pressed, Vector2D.One);
			SetTouchState(touch, State.Releasing, Vector2D.One);
			Assert.IsTrue(isFinished);
		}

		private void SetTouchState(MockTouch touch, State state, Vector2D position)
		{
			touch.SetTouchState(0, state, position);
			touch.SetTouchState(1, state, position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new TouchDualDragTrigger("");
			Assert.AreEqual(DragDirection.Free, trigger.Direction);
			trigger = new TouchDualDragTrigger("Horizontal");
			Assert.AreEqual(DragDirection.Horizontal, trigger.Direction);
			trigger = new TouchDualDragTrigger("Vertical");
			Assert.AreEqual(DragDirection.Vertical, trigger.Direction);
		}
	}
}