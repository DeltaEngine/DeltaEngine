using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows mouse button presses to be tracked.
	/// </summary>
	public class MouseButtonTrigger : InputTrigger, PositionTrigger, MouseTrigger
	{
		public MouseButtonTrigger(State state)
			: this(MouseButton.Left, state) {}

		public MouseButtonTrigger(MouseButton button = MouseButton.Left, State state = State.Pressing)
		{
			Button = button;
			State = state;
		}

		public MouseButton Button { get; private set; }
		public State State { get; private set; }
		public Vector2D Position { get; set; }

		public MouseButtonTrigger(string buttonAndState)
		{
			var parameters = buttonAndState.SplitAndTrim(new[] { ' ' });
			if (parameters.Length == 0)
				throw new CannotCreateMouseButtonTriggerWithoutButton();
			Button = parameters[0].Convert<MouseButton>();
			State = parameters.Length > 1 ? parameters[1].Convert<State>() : State.Pressing;
		}

		public class CannotCreateMouseButtonTriggerWithoutButton : Exception {}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			Position = mouse.Position;
			if (mouse.GetButtonState(Button) == State &&
				ScreenSpace.Current.Viewport.Contains(mouse.Position))
				Invoke();
		}
	}
}