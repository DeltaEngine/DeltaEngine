using System;
using DeltaEngine.Commands;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any touch movement, useful to update cursor positions.
	/// </summary>
	public class TouchMovementTrigger : DragTrigger, TouchTrigger
	{
		public TouchMovementTrigger() {}

		public TouchMovementTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new TouchMovementTriggerHasNoParameters();
		}

		public class TouchMovementTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			bool changedPosition = Position != touch.GetPosition(0);
			Position = touch.GetPosition(0);
			if(changedPosition)
				Invoke();
		}
	}
}