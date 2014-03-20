using System;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows mouse double clicks to be tracked.
	/// </summary>
	public class MouseDoubleClickTrigger : InputTrigger, PositionTrigger, MouseTrigger
	{
		public MouseDoubleClickTrigger(MouseButton button = MouseButton.Left)
		{
			Button = button;
		}

		public MouseButton Button { get; private set; }
		public Vector2D Position { get; set; }

		public MouseDoubleClickTrigger(string button)
		{
			Button = String.IsNullOrEmpty(button) ? MouseButton.Left : button.Convert<MouseButton>();
		}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			State currentState = mouse.GetButtonState(Button);
			if ((lastState == State.Pressing || lastState == State.Pressed) &&
				currentState == State.Releasing)
			{
				if (!firstClickDetected)
					firstClickDetected = true;
				else
				{
					if (elapsedAfterFirstClick < 0.2f && ScreenSpace.Current.Viewport.Contains(mouse.Position))
						Invoke();
					firstClickDetected = false;
				}
			}
			lastState = currentState;
			if (firstClickDetected)
				elapsedAfterFirstClick += Time.Delta;
			else
				elapsedAfterFirstClick = 0f;
		}

		private State lastState;
		private bool firstClickDetected;
		private float elapsedAfterFirstClick;
	}
}