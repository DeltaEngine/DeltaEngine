using System;
using DeltaEngine.Commands;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a simple touch to be detected.
	/// </summary>
	public class TouchPressTrigger : InputTrigger, TouchTrigger
	{
		public TouchPressTrigger(State state = State.Pressing)
		{
			State = state;
		}

		public State State { get; internal set; }

		public TouchPressTrigger(string state)
		{
			State = String.IsNullOrEmpty(state) ? State.Pressing : state.Convert<State>();
		}

		public void HandleWithTouch(Touch touch)
		{
			if (touch.GetState(0) == State)
				Invoke();
		}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}
	}
}