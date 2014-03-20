using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Allows a touch dual drag to be detected.
	/// </summary>
	public class TouchDualDragTrigger : DragTrigger, TouchTrigger
	{
		public TouchDualDragTrigger(DragDirection direction = DragDirection.Free)
		{
			Direction = direction;
		}

		public Vector2D SecondStartPosition { get; set; }
		public Vector2D SecondPosition { get; set; }

		public TouchDualDragTrigger(string direction)
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
			if (touch.GetState(1) == State.Pressing)
				SecondStartPosition = touch.GetPosition(1);
			if (IsDualDragActive(touch))
				HandleOnActive(touch);
			else
				Reset();
		}
		private bool IsDualDragActive(Touch touch)
		{
			bool isFirstTouchActive = StartPosition != Vector2D.Unused &&
				touch.GetState(0) != State.Released;
			bool isSecondTouchActive = SecondStartPosition != Vector2D.Unused &&
				touch.GetState(0) != State.Released;
			return isFirstTouchActive && isSecondTouchActive;
		}

		private void HandleOnActive(Touch touch)
		{
			var movementDirection = StartPosition.DirectionTo(touch.GetPosition(0));
			var secondMovementDirection = SecondStartPosition.DirectionTo(touch.GetPosition(1));
			if (IsMovementWithinEpsilon(movementDirection, secondMovementDirection))
				return;
			if (IsDragDirectionCorrect(movementDirection) &&
				IsDragDirectionCorrect(secondMovementDirection))
			{
				Position = touch.GetPosition(0);
				SecondPosition = touch.GetPosition(1);
				DoneDragging = touch.GetState(0) <= State.Releasing && touch.GetState(1) <= State.Releasing;
			}
			Invoke();
		}

		private static bool IsMovementWithinEpsilon(Vector2D firstMovement, Vector2D secondMovement)
		{
			return firstMovement.Length <= PositionEpsilon || secondMovement.Length <= PositionEpsilon;
		}

		private void Reset()
		{
			StartPosition = Vector2D.Unused;
			SecondStartPosition = Vector2D.Unused;
			DoneDragging = false;
		}
	}
}