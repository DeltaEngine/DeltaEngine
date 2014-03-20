using DeltaEngine.Commands;
using DeltaEngine.Extensions;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Drag events with Mouse.
	/// </summary>
	public class MouseDragTrigger : DragTrigger, MouseTrigger
	{
		public MouseDragTrigger(MouseButton button = MouseButton.Left,
			DragDirection direction = DragDirection.Free)
		{
			Button = button;
			Direction = direction;
		}

		public MouseButton Button { get; private set; }

		public MouseDragTrigger(string button)
		{
			var parameters = button.SplitAndTrim(new[] { ' ' });
			Button = parameters.Length > 0 ? parameters[0].Convert<MouseButton>() : MouseButton.Left;
			Direction = parameters.Length > 1
				? parameters[1].Convert<DragDirection>() : DragDirection.Free;
		}

		protected override void StartInputDevice()
		{
			Start<Mouse>();
		}

		public void HandleWithMouse(Mouse mouse)
		{
			if (mouse.GetButtonState(Button) == State.Pressing &&
				ScreenSpace.Current.Viewport.Contains(mouse.Position))
				StartPosition = mouse.Position;
			else if (mouse.GetButtonState(Button) != State.Released)
				UpdateWhileDragging(mouse);
			else
				Reset();
		}

		private void UpdateWhileDragging(Mouse mouse)
		{
			var movementDirection = StartPosition.DirectionTo(Position);
			Position = mouse.Position;
			if (IsDragDirectionCorrect(movementDirection))
				DoneDragging = mouse.GetButtonState(Button) == State.Releasing;
			if (ScreenSpace.Current.Viewport.Contains(mouse.Position))
				Invoke();
		}

		private void Reset()
		{
			DoneDragging = false;
		}
	}
}