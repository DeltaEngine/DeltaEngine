using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseDragDropTriggerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			mouse = Resolve<Mouse>() as MockMouse;
			if (mouse != null)
				mouse.SetNativePosition(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		private MockMouse mouse;

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new MouseDragDropTrigger(Rectangle.One, MouseButton.Right);
			Assert.AreEqual(Rectangle.One, trigger.StartArea);
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			Assert.AreEqual(Vector2D.Unused, trigger.StartDragPosition);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MouseDragDropTrigger("1.1 2.2 3.3 4.4 Right");
			Assert.AreEqual(new Rectangle(1.1f, 2.2f, 3.3f, 4.4f), trigger.StartArea);
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			Assert.AreEqual(Vector2D.Unused, trigger.StartDragPosition);
			Assert.Throws<MouseDragDropTrigger.CannotCreateMouseDragDropTriggerWithoutStartArea>(
				() => new MouseDragDropTrigger("1 2 3"));
		}

		[Test, CloseAfterFirstFrame]
		public void DragDropOutsideStartArea()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => { startPoint = position; }).Add(
				new MouseDragDropTrigger(Rectangle.HalfCentered, MouseButton.Left));
			SetMouseState(State.Pressing, Vector2D.Zero);
			SetMouseState(State.Pressed, Vector2D.One);
			SetMouseState(State.Releasing, Vector2D.One);
			SetMouseState(State.Released, Vector2D.One);
			Assert.AreEqual(-Vector2D.One, startPoint);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetButtonState(MouseButton.Left, state);
			mouse.SetNativePosition(position);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void DragDropInsideStartArea()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => { startPoint = position; }).Add(
				new MouseDragDropTrigger(Rectangle.HalfCentered, MouseButton.Left));
			SetMouseState(State.Pressing, Vector2D.Half);
			SetMouseState(State.Pressed, Vector2D.One);
			SetMouseState(State.Releasing, Vector2D.One);
			Assert.AreEqual(Vector2D.Half, startPoint);
		}

		[Test, CloseAfterFirstFrame]
		public void DragDropCloseToStartPointWillDoNothing()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D startPoint = -Vector2D.One;
			new Command(position => { startPoint = position; }).Add(
				new MouseDragDropTrigger(Rectangle.HalfCentered, MouseButton.Left));
			SetMouseState(State.Pressing, Vector2D.Half);
			SetMouseState(State.Pressed, Vector2D.Half);
			SetMouseState(State.Releasing, Vector2D.Half);
			Assert.AreEqual(-Vector2D.One, startPoint);
		}
	}
}