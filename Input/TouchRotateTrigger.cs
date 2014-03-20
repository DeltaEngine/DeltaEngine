using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	public class TouchRotateTrigger : DragTrigger, TouchTrigger
	{
		public TouchRotateTrigger() {}

		public TouchRotateTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchRotateTriggerHasNoParameters();
		}

		public class TouchRotateTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public float Angle { get; set; }

		public void HandleWithTouch(Touch touch)
		{
			if (touch.GetState(0) >= State.Pressing && touch.GetState(1) >= State.Pressing)
			{
				var vector = (touch.GetPosition(1) - touch.GetPosition(0));
				vector = vector / vector.Length;
				Angle = (float)Math.Atan2(vector.X, vector.Y);
				if (Angle < 0)
					Angle += (float)(2 * Math.PI);
				Invoke();
			}
			else
				Angle = 0f;
		}
	}
}