using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch tap to be detected.
	/// </summary>
	public class TouchTapTrigger : InputTrigger, TouchTrigger
	{
		public TouchTapTrigger() {}

		public TouchTapTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchTapTriggerHasNoParameters();
		} //ncrunch: no coverage

		public class TouchTapTriggerHasNoParameters : Exception {}

		public void HandleWithTouch(Touch touch)
		{
			bool wasJustStartedPressing = lastState == State.Pressing;
			State currentState = touch.GetState(0);
			var isNowReleased = currentState == State.Releasing;
			lastState = currentState;
			if (isNowReleased && wasJustStartedPressing)
				Invoke();
		}

		private State lastState;
		protected override void StartInputDevice()
		{
			Start<Touch>();
		}
	}
}