using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Fires once when the mouse button is pressed and the mouse has not moved for some time.
	/// </summary>
	public class MouseHoldTrigger : InputTrigger, PositionTrigger, MouseTrigger
	{
		public MouseHoldTrigger(Rectangle holdArea, float holdTime = DefaultHoldTime,
			MouseButton button = MouseButton.Left)
		{
			HoldArea = holdArea;
			HoldTime = holdTime;
			Button = button;
		}

		public Rectangle HoldArea { get; private set; }
		public float HoldTime { get; private set; }
		public MouseButton Button { get; private set; }
		private const float DefaultHoldTime = 0.5f;
		public Vector2D Position { get; set; }

		public MouseHoldTrigger(string holdAreaAndHoldTimeAndButton)
		{
			var parameters = holdAreaAndHoldTimeAndButton.SplitAndTrim(new[] { ' ' });
			if (parameters.Length < 4)
				throw new CannotCreateMouseHoldTriggerWithoutHoldArea();
			HoldArea = BuildStringForParameter(parameters).Convert<Rectangle>();
			HoldTime = parameters.Length > 4 ? parameters[4].Convert<float>() : DefaultHoldTime;
			Button = parameters.Length > 5 ? parameters[5].Convert<MouseButton>() : MouseButton.Left;
		}

		public class CannotCreateMouseHoldTriggerWithoutHoldArea : Exception {}

		private static string BuildStringForParameter(IList<string> parameters)
		{
			return parameters[0] + " " + parameters[1] + " " + parameters[2] + " " + parameters[3];
		}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			if (mouse.GetButtonState(Button) == State.Pressing)
				StartPosition = mouse.Position;
			Position = mouse.Position;
			if (!CheckHoverState(mouse))
				Elapsed = 0.0f;
			else if (IsHovering())
				Invoke();
		}

		private bool CheckHoverState(Mouse mouse)
		{
			return HoldArea.Contains(StartPosition) &&
				mouse.GetButtonState(Button) == State.Pressed &&
				StartPosition.DistanceTo(mouse.Position) < PositionEpsilon;
		}

		public bool IsHovering()
		{
			if (Elapsed >= HoldTime || !HoldArea.Contains(Position))
				return false;
			Elapsed += Time.Delta;
			return Elapsed >= HoldTime;
		}

		private float Elapsed { get; set; }
		private Vector2D StartPosition { get; set; }
	}
}