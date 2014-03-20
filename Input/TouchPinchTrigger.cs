using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch pinch to be detected.
	/// </summary>
	public class TouchPinchTrigger : InputTrigger, ZoomTrigger, TouchTrigger
	{
		public TouchPinchTrigger() {}

		public TouchPinchTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchPinchTriggerHasNoParameters();
		}

		public class TouchPinchTriggerHasNoParameters : Exception {}

		public float ZoomAmount { get; set; }
		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			var direction = touch.GetPosition(0).DirectionTo(touch.GetPosition(1));
			if (touch.GetState(0) >= State.Pressing && touch.GetState(1) >= State.Pressing)
			{
				ZoomAmount = direction.Length - lastDistance;
				Invoke();
			}
			else
				ZoomAmount = 0f;
			lastDistance = direction.Length;
		}

		private float lastDistance;
	}
}