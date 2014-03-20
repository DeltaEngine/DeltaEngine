using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input.Mocks;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Input.Tests
{
	public class MouseMovementTriggerTests : TestWithMocksOrVisually
	{
		[Test]
		public void MoveMouseToUpdatePositionOfCircle()
		{
			var ellipse = new Ellipse(new Rectangle(-0.1f, -0.1f, 0.1f, 0.1f), Color.Red);
			new Command(pos => ellipse.Center = pos).Add(new MouseMovementTrigger());
		}

		[Test, CloseAfterFirstFrame]
		public void UpdatePosition()
		{
			Vector2D position = Vector2D.Zero;
			new Command(pos => position = pos).Add(new MouseMovementTrigger());
			Assert.AreEqual(Vector2D.Zero, position);
			var mockMouse = Resolve<Mouse>() as MockMouse;
			if (mockMouse == null)
				return; //ncrunch: no coverage
			mockMouse.SetNativePosition(new Vector2D(0.4f, 0.6f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Vector2D(0.4f, 0.6f), position);
		}

		[Test, CloseAfterFirstFrame]
		public void Create()
		{
			var trigger = new MouseMovementTrigger();
			Assert.AreEqual(Vector2D.Zero, trigger.Position);
			Assert.IsFalse(trigger.IsPauseable);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateFromString()
		{
			var trigger = new MouseMovementTrigger("");
			Assert.AreEqual(Vector2D.Zero, trigger.Position);
			Assert.Throws<MouseMovementTrigger.MouseMovementTriggerHasNoParameters>(
				() => new MouseMovementTrigger("a"));
		}
	}
}