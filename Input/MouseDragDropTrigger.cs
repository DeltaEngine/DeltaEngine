using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Drag and Drop events with Mouse.
	/// </summary>
	public class MouseDragDropTrigger : InputTrigger, MouseTrigger
	{
		public MouseDragDropTrigger(Rectangle startArea, MouseButton button)
		{
			StartArea = startArea;
			Button = button;
			StartDragPosition = Vector2D.Unused;
		}

		public Rectangle StartArea { get; private set; }
		public MouseButton Button { get; private set; }
		public Vector2D StartDragPosition { get; set; }

		public MouseDragDropTrigger(string startAreaAndButton)
		{
			var parameters = startAreaAndButton.SplitAndTrim(new[] { ' ' });
			if (parameters.Length < 4)
				throw new CannotCreateMouseDragDropTriggerWithoutStartArea();
			StartArea = BuildParameterStringForRectangle(parameters).Convert<Rectangle>();
			Button = parameters.Length > 4 ? parameters[4].Convert<MouseButton>() : MouseButton.Left;
			StartDragPosition = Vector2D.Unused;
		}

		private static string BuildParameterStringForRectangle(IList<string> parameters)
		{
			return parameters[0] + " " + parameters[1] + " " + parameters[2] + " " + parameters[3];
		}

		public class CannotCreateMouseDragDropTriggerWithoutStartArea : Exception {}

		public void HandleWithMouse(Mouse mouse)
		{
			if (StartArea.Contains(mouse.Position) && mouse.GetButtonState(Button) == State.Pressing)
				StartDragPosition = mouse.Position;
			else if (StartDragPosition != Vector2D.Unused &&
				mouse.GetButtonState(Button) != State.Released)
				if (StartDragPosition.DistanceTo(mouse.Position) > PositionEpsilon)
					Invoke();
				else
					StartDragPosition = Vector2D.Unused;
		}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}
	}
}