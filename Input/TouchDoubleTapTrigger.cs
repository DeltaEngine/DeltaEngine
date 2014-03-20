using System;
using DeltaEngine.Commands;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch double tap to be detected.
	/// </summary>
	public class TouchDoubleTapTrigger : InputTrigger, TouchTrigger
	{
		public TouchDoubleTapTrigger() {}

		public TouchDoubleTapTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchDoubleTapTriggerHasNoParameters();
		} 

		public class TouchDoubleTapTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			State currentState = touch.GetState(0);
			if ((lastState == State.Pressing || lastState == State.Pressed) &&
				currentState == State.Releasing)
			{
				if (!firstTapDetected)
					firstTapDetected = true;
				else
				{
					if (elapsedAfterFirstTap < 0.2f)
						Invoke();
					firstTapDetected = false;
				}
			}
			lastState = currentState;
			if (firstTapDetected)
				elapsedAfterFirstTap += Time.Delta;
			else
				elapsedAfterFirstTap = 0f;
		}

		private State lastState;
		private bool firstTapDetected;
		private float elapsedAfterFirstTap;
	}
}