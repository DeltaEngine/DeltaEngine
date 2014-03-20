using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchDragTriggerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			touch = Resolve<Touch>() as MockTouch;
			if (touch != null)
				touch.SetTouchState(0, State.Released, Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private MockTouch touch;

		[Test]
		public void DragTouchToCreateRectangles()
		{
			MouseDragTriggerTests.DragToCreateRectangles(new TouchDragTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void DragTouch()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			bool isFinished = false;
			new Command((start, end, done) => isFinished = done).Add(new TouchDragTrigger());
			SetTouchState(State.Pressing, Vector2D.Zero);
			SetTouchState(State.Pressed, Vector2D.One);
			SetTouchState(State.Releasing, Vector2D.One);
			Assert.IsTrue(isFinished);
		}

		private void SetTouchState(State state, Vector2D position)
		{
			if (touch == null)
				return; //ncrunch: no coverage
			touch.SetTouchState(0, state, position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void IfDistanceIsToSmallNothingWillHappen()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			bool isFinished = false;
			new Command((start, end, done) => isFinished = done).Add(new TouchDragTrigger());
			SetTouchState(State.Pressing, Vector2D.Zero);
			SetTouchState(State.Pressed, Vector2D.Zero);
			SetTouchState(State.Releasing, Vector2D.Zero);
			Assert.False(isFinished);
		}

		[Test, CloseAfterFirstFrame]
		public void DragTouchHorizontal()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			bool isFinished = false;
			new Command((start, end, done) => isFinished = done).Add(
				new TouchDragTrigger(DragDirection.Horizontal));
			SetTouchState(State.Pressing, Vector2D.Zero);
			SetTouchState(State.Pressed, new Vector2D(1,0));
			SetTouchState(State.Releasing, new Vector2D(1,0));
			Assert.IsTrue(isFinished);
		}

		[Test, CloseAfterFirstFrame]
		public void DragTouchVertical()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			bool isFinished = false;
			new Command((start, end, done) => isFinished = done).Add(
				new TouchDragTrigger(DragDirection.Vertical));
			SetTouchState(State.Pressing, Vector2D.Zero);
			SetTouchState(State.Pressed, new Vector2D(0, 1));
			SetTouchState(State.Releasing, new Vector2D(0, 1));
			Assert.IsTrue(isFinished);
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new TouchDragTrigger("");
			Assert.AreEqual(DragDirection.Free, trigger.Direction);
			trigger = new TouchDragTrigger("Horizontal");
			Assert.AreEqual(DragDirection.Horizontal, trigger.Direction);
			trigger = new TouchDragTrigger("Vertical");
			Assert.AreEqual(DragDirection.Vertical, trigger.Direction);
		}
	}
}