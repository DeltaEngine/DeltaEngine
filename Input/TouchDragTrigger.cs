using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch drag to be detected.
	/// </summary>
	public class TouchDragTrigger : DragTrigger, TouchTrigger
	{
		public TouchDragTrigger(DragDirection direction = DragDirection.Free)
		{
			Direction = direction;
		}

		public TouchDragTrigger(string direction)
		{
			var parameters = direction.SplitAndTrim(new[] { ' ' });
			Direction = parameters.Length > 0
				? parameters[0].Convert<DragDirection>() : DragDirection.Free;
		}

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			if (touch.GetState(0) == State.Pressing)
				StartPosition = touch.GetPosition(0);
			else if (StartPosition != Vector2D.Unused && touch.GetState(0) != State.Released)
				UpdateWhileDragging(touch);
			else
				Reset();
		}

		private void UpdateWhileDragging(Touch touch)
		{
			var movementDirection = StartPosition.DirectionTo(touch.GetPosition(0));
			if (movementDirection.Length <= PositionEpsilon)
				return;
			Position = touch.GetPosition(0);
			if (IsDragDirectionCorrect(movementDirection))
				DoneDragging = touch.GetState(0) == State.Releasing;
			Invoke();
		}

		private void Reset()
		{
			StartPosition = Vector2D.Unused;
			DoneDragging = false;
		}
	}
}