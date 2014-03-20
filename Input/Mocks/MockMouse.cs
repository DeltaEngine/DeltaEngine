using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input.Mocks
{
	/// <summary>
	/// Mock mouse for unit testing.
	/// </summary>
	public sealed class MockMouse : Mouse
	{
		public MockMouse()
		{
			IsAvailable = true;
			Position = nextPosition = Vector2D.Half;
		}

		public override bool IsAvailable { get; protected set; }

		public override void Dispose() {}

		public override void SetNativePosition(Vector2D position)
		{
			nextPosition = position;
			Position = position;
		}

		private Vector2D nextPosition;

		public void SetButtonState(MouseButton button, State state)
		{
			if (button == MouseButton.Right)
				RightButton = state;
			else if (button == MouseButton.Middle)
				MiddleButton = state;
			else if (button == MouseButton.X1)
				X1Button = state;
			else if (button == MouseButton.X2)
				X2Button = state;
			else
				LeftButton = state;
		}

		public void ScrollUp()
		{
			ScrollWheelValue += 1;
		}

		public void ScrollDown()
		{
			ScrollWheelValue -= 1;
		}

		public override void Update(IEnumerable<Entity> entities)
		{
			Position = nextPosition;
			base.Update(entities);
			LeftButton = GetUpdatedPressingAndReleasingState(LeftButton);
			RightButton = GetUpdatedPressingAndReleasingState(RightButton);
			MiddleButton = GetUpdatedPressingAndReleasingState(MiddleButton);
			X1Button = GetUpdatedPressingAndReleasingState(X1Button);
			X2Button = GetUpdatedPressingAndReleasingState(X2Button);
		}

		private static State GetUpdatedPressingAndReleasingState(State buttonState)
		{
			if (buttonState == State.Pressing)
				return State.Pressed;
			if (buttonState == State.Releasing)
				return State.Released;
			return buttonState;
		}
	}
}