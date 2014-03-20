using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseHoldTriggerTests : TestWithMocksOrVisually
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

		[Test]
		public void HoldLeftClickOnRectangle()
		{
			var drawArea = new Rectangle(0.25f, 0.25f, 0.5f, 0.25f);
			new FilledRect(drawArea, Color.Blue);
			var trigger = new MouseHoldTrigger(drawArea);
			var counter = 0;
			var text = new FontText(Font.Default, "", drawArea.Move(new Vector2D(0.0f, 0.25f)));
			new Command(() => text.Text = "MouseHold Triggered " + ++counter + " times.").Add(trigger);
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new MouseHoldTrigger(Rectangle.One, 0.5f, MouseButton.Right);
			Assert.AreEqual(Rectangle.One, trigger.HoldArea);
			Assert.AreEqual(0.5f, trigger.HoldTime);
			Assert.AreEqual(MouseButton.Right, trigger.Button);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MouseHoldTrigger("1 2 3 4 5.5 Right");
			Assert.AreEqual(new Rectangle(1, 2, 3, 4), trigger.HoldArea);
			Assert.AreEqual(5.5f, trigger.HoldTime);
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			Assert.Throws<MouseHoldTrigger.CannotCreateMouseHoldTriggerWithoutHoldArea>(
				() => new MouseHoldTrigger("1 2 3"));
		}

		[Test, CloseAfterFirstFrame]
		public void HoldMouseOutsideHoldArea()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D mousePosition = -Vector2D.One;
			new Command(position => { mousePosition = position; }).Add(
				new MouseHoldTrigger(Rectangle.HalfCentered));
			SetMouseState(State.Pressing, Vector2D.Zero);
			SetMouseState(State.Pressed, Vector2D.Zero);
			AdvanceTimeAndUpdateEntities(1.05f);
			Assert.AreEqual(-Vector2D.One, mousePosition);
		}

		private void SetMouseState(State state, Vector2D position)
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			mouse.SetNativePosition(position);
			mouse.SetButtonState(MouseButton.Left, state);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void HoldMouseHovering()
		{
			var drawArea = new Rectangle(0.25f, 0.25f, 0.5f, 0.25f);
			var rect = new FilledRect(drawArea, Color.Blue);
			var trigger = new MouseHoldTrigger(drawArea);
			trigger.Invoked += () => rect.Color = Color.Red;
			trigger.Position = new Vector2D(0.3f, 0.3f);
			Assert.IsFalse(trigger.IsHovering());
			AdvanceTimeAndUpdateEntities(1.05f);
			Assert.IsFalse(trigger.IsHovering());
		}

		[Test, CloseAfterFirstFrame]
		public void MoveMouseInsideHoldArea()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D mousePosition = -Vector2D.One;
			new Command(position => { mousePosition = position; }).Add(
				new MouseHoldTrigger(Rectangle.HalfCentered));
			SetMouseState(State.Pressing, Vector2D.Half);
			SetMouseState(State.Pressed, Vector2D.Half);
			AdvanceTimeAndUpdateEntities(0.5f);
			SetMouseState(State.Pressed, new Vector2D(0.6f, 0.6f));
			AdvanceTimeAndUpdateEntities(0.5f);
			Assert.AreEqual(Vector2D.Half, mousePosition);
		}

		//ncrunch: no coverage start
		[Test, CloseAfterFirstFrame, Category("Slow")]
		public void HoldMouseInsideHoldArea()
		{
			if (mouse == null)
				return; //ncrunch: no coverage
			Vector2D mousePosition = -Vector2D.One;
			new Command(position => { mousePosition = position; }).Add(
				new MouseHoldTrigger(Rectangle.HalfCentered));
			SetMouseState(State.Pressing, Vector2D.Half);
			SetMouseState(State.Pressed, Vector2D.Half);
			AdvanceTimeAndUpdateEntities(1.05f);
			Assert.AreEqual(Vector2D.Half, mousePosition);
		}
	}
}