using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Drag and Drop events with Touch.
	/// </summary>
	public class TouchDragDropTrigger : InputTrigger, TouchTrigger
	{
		public TouchDragDropTrigger(string startArea)
			: this(startArea.Convert<Rectangle>()) {}

		public TouchDragDropTrigger(Rectangle startArea)
		{
			StartArea = startArea;
			StartDragPosition = Vector2D.Unused;
		}

		public Rectangle StartArea { get; private set; }
		public Vector2D StartDragPosition { get; set; }

		protected override void StartInputDevice()
		{
			Start<Touch>();
		}

		public void HandleWithTouch(Touch touch)
		{
			var position = touch.GetPosition(0);
			if (StartArea.Contains(position) && touch.GetState(0) == State.Pressing)
				StartDragPosition = position;
			else if (StartDragPosition != Vector2D.Unused && touch.GetState(0) != State.Released)
				InvokeIfMovedFarEnough(position);
			else
				StartDragPosition = Vector2D.Unused;
		}

		private void InvokeIfMovedFarEnough(Vector2D position)
		{
			if (StartDragPosition.DistanceTo(position) > PositionEpsilon)
				Invoke();
		}
	}
}