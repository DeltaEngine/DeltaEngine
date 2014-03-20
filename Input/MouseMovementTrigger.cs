using System;
using DeltaEngine.Commands;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Tracks any mouse movement, useful to update cursor positions or check hover states.
	/// </summary>
	public class MouseMovementTrigger : DragTrigger, MouseTrigger
	{
		public MouseMovementTrigger() {}

		public MouseMovementTrigger(string empty)
		{
			if (!String.IsNullOrEmpty(empty))
				throw new MouseMovementTriggerHasNoParameters();
		}

		public class MouseMovementTriggerHasNoParameters : Exception {}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			if (Position == mouse.Position)
				return;
			Position = mouse.Position;
			if (ScreenSpace.Current.Viewport.Contains(mouse.Position))
				Invoke();
		}
	}
}