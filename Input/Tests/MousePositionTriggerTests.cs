using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MousePositionTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowRedCircleOnMouseClickWithMovement()
		{
			var ellipse = new Ellipse(new Rectangle(-0.1f, -0.1f, 0.1f, 0.1f), Color.Red);
			new Command(pos => ellipse.Center = pos).Add(new MousePositionTrigger(MouseButton.Left,
				State.Pressed));
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new MousePositionTrigger(MouseButton.Right, State.Pressed);
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			Assert.AreEqual(State.Pressed, trigger.State);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MousePositionTrigger("Right Pressed");
			Assert.AreEqual(MouseButton.Right, trigger.Button);
			Assert.AreEqual(State.Pressed, trigger.State);
			Assert.Throws<MousePositionTrigger.CannotCreateMousePositionTriggerWithoutKey>(
				() => new MousePositionTrigger(""));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckMouseAndTriggerPosition()
		{
			var mouse = Resolve<Mouse>() as MockMouse;
			if (mouse == null)
				return; //ncrunch: no coverage
			var trigger = new MousePositionTrigger(MouseButton.Right, State.Pressed);
			trigger.Position = Vector2D.Zero;
			mouse.SetNativePosition(trigger.Position);
			Assert.AreEqual(mouse.Position, trigger.Position);
		}
	}
}