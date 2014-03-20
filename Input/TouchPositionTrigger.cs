using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks touch movement with a mouse button in a prescribed state.
	/// </summary>
	public class TouchPositionTrigger : InputTrigger, PositionTrigger, TouchTrigger
	{
		public TouchPositionTrigger(State state = State.Pressing)
		{
			State = state;
		}

		public State State { get; private set; }
		public Vector2D Position { get; set; }

		public TouchPositionTrigger(string state)
		{
			State = String.IsNullOrEmpty(state) ? State.Pressing : state.Convert<State>();
		}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			var isButton = touch.GetState(0) == State;
			bool changedPosition = Position != touch.GetPosition(0);
			Position = touch.GetPosition(0);
			if (isButton && changedPosition && ScreenSpace.Current.Viewport.Contains(Position))
				Invoke();
		}
	}
}