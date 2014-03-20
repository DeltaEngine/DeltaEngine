using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch flick to be detected.
	/// </summary>
	public class TouchFlickTrigger : InputTrigger, TouchTrigger
	{
		public TouchFlickTrigger() {}

		public TouchFlickTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchFlickTriggerHasNoParameters();
		}

		public class TouchFlickTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			if (touch.GetState(0) == State.Pressing)
				SetFlickPositionAndResetTime(touch.GetPosition(0));
			else if (StartPosition != Vector2D.Unused && touch.GetState(0) != State.Released)
			{
				PressTime += Time.Delta;
				if (touch.GetState(0) == State.Releasing &&
					StartPosition.DistanceTo(touch.GetPosition(0)) > PositionEpsilon && PressTime < 0.3f)
					Invoke();
			}
			else
				SetFlickPositionAndResetTime(Vector2D.Unused);
		}

		public float PressTime { get; private set; }
		public Vector2D StartPosition { get; private set; }

		private void SetFlickPositionAndResetTime(Vector2D position)
		{
			StartPosition = position;
			PressTime = 0;
		}
	}
}