using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks mouse movement with a mouse button in a prescribed state.
	/// </summary>
	public class MousePositionTrigger : InputTrigger, PositionTrigger, MouseTrigger
	{
		public MousePositionTrigger(MouseButton button = MouseButton.Left, State state = State.Pressing)
		{
			Button = button;
			State = state;
		}

		public MouseButton Button { get; private set; }
		public State State { get; private set; }
		public Vector2D Position { get; set; }

		public MousePositionTrigger(string buttonAndState)
		{
			var parameters = buttonAndState.SplitAndTrim(new[] { ' ' });
			if (parameters.Length == 0)
				throw new CannotCreateMousePositionTriggerWithoutKey();
			Button = parameters[0].Convert<MouseButton>();
			State = parameters.Length > 1 ? parameters[1].Convert<State>() : State.Pressing;
		}

		public class CannotCreateMousePositionTriggerWithoutKey : Exception {}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			if (Position == mouse.Position)
				return;
			var isButton = mouse.GetButtonState(Button) == State;
			Position = mouse.Position;
			if (isButton && ScreenSpace.Current.Viewport.Contains(mouse.Position))
				Invoke();
		}
	}
}