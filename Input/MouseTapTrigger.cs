using System;
using DeltaEngine.Commands;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows mouse button tap to be tracked.
	/// </summary>
	public class MouseTapTrigger : InputTrigger, MouseTrigger
	{
		public MouseTapTrigger(MouseButton button)
		{
			Button = button;
		}

		public MouseButton Button { get; private set; }

		public MouseTapTrigger(string button)
		{
			Button = String.IsNullOrEmpty(button) ? MouseButton.Left : button.Convert<MouseButton>();
		}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			bool wasJustStartedPressing = lastState == State.Pressing;
			State currentState = mouse.GetButtonState(Button);
			var isNowReleased = currentState == State.Releasing;
			lastState = currentState;
			if (isNowReleased && wasJustStartedPressing)
				Invoke();
		}

		private State lastState;
	}
}