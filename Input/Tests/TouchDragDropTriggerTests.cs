using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class TouchDragDropTriggerTests : TestWithMocksOrVisually
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

		[Test, CloseAfterFirstFrame]
		public void DragDropTouchOutsideStartArea()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => { startPoint = position; }).Add(
				new TouchDragDropTrigger(Rectangle.HalfCentered));
			SetTouchState(State.Pressing, Vector2D.Zero);
			SetTouchState(State.Pressed, Vector2D.One);
			SetTouchState(State.Releasing, Vector2D.One);
			SetTouchState(State.Released, Vector2D.One);
			Assert.AreEqual(-Vector2D.One, startPoint);
		}

		private void SetTouchState(State state, Vector2D position)
		{
			if (touch == null)
				return; //ncrunch: no coverage
			touch.SetTouchState(0, state, position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void DragDropTouchInsideStartArea()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => { startPoint = position; }).Add(
				new TouchDragDropTrigger(Rectangle.HalfCentered));
			SetTouchState(State.Pressing, Vector2D.Half);
			SetTouchState(State.Pressed, Vector2D.One);
			SetTouchState(State.Releasing, Vector2D.One);
			Assert.AreEqual(Vector2D.Half, startPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new TouchDragDropTrigger(Rectangle.One);
			Assert.AreEqual(Rectangle.One, trigger.StartArea);
			Assert.AreEqual(Vector2D.Unused, trigger.StartDragPosition);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new TouchDragDropTrigger("0.1 0.2 0.3 0.4");
			Assert.AreEqual(new Rectangle(0.1f, 0.2f, 0.3f, 0.4f), trigger.StartArea);
			Assert.AreEqual(Vector2D.Unused, trigger.StartDragPosition);
		}

		[Test, CloseAfterFirstFrame]
		public void DragDropCloseToStartPointWillDoNothing()
		{
			if (touch == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => //ncrunch: no coverage start
			{ startPoint = position; }).Add(new TouchDragDropTrigger(Rectangle.HalfCentered));
			//ncrunch: no coverage end
			SetTouchState(State.Pressing, Vector2D.Half);
			SetTouchState(State.Pressed, Vector2D.Half);
			SetTouchState(State.Releasing, Vector2D.Half);
			Assert.AreEqual(-Vector2D.One, startPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateTouchDragDropTriggerFromString()
		{
			var startArea = Rectangle.One;
			var trigger = new TouchDragDropTrigger(startArea.ToString());
			Assert.AreEqual(startArea, trigger.StartArea);
		}
	}
}