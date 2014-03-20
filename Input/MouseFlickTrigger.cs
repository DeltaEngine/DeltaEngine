using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a mouse flick to be detected.
	/// </summary>
	public class MouseFlickTrigger : InputTrigger, MouseTrigger
	{
		public MouseFlickTrigger()
		{
			StartPosition = new Vector2D();
		}

		public MouseFlickTrigger(string empty)
		{
			StartPosition = new Vector2D();
			if (!String.IsNullOrEmpty(empty))
				throw new MouseFlickTriggerHasNoParameters();
		}

		public class MouseFlickTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public float PressTime { get; private set; }
		public Vector2D StartPosition { get; private set; }

		public void HandleWithMouse(Mouse mouse)
		{
			if (mouse.LeftButton == State.Pressing)
				SetFlickPositionAndResetTime(mouse.Position);
			else if (StartPosition != Vector2D.Unused && mouse.LeftButton != State.Released)
			{
				PressTime += Time.Delta;
				if (mouse.LeftButton == State.Releasing &&
					StartPosition.DistanceTo(mouse.Position) > PositionEpsilon && PressTime < 0.3f)
					Invoke();
			}
			else
				SetFlickPositionAndResetTime(Vector2D.Unused);
		}

		private void SetFlickPositionAndResetTime(Vector2D position)
		{
			StartPosition = position;
			PressTime = 0;
		}
	}
}