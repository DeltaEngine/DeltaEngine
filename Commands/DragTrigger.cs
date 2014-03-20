using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Allows a start and end position based drag trigger to be invoked. 
	/// </summary>
	public abstract class DragTrigger : InputTrigger, PositionTrigger
	{
		public Vector2D Position { get; set; }
		public Vector2D StartPosition { get; set; }
		public DragDirection Direction { get; protected set; }
		public bool DoneDragging { get; set; }

		protected bool IsDragDirectionCorrect(Vector2D movementDirection)
		{
			if (Direction == DragDirection.Horizontal)
				return Math.Abs(movementDirection.Y) < AllowedDragDirectionOffset;
			if (Direction == DragDirection.Vertical)
				return Math.Abs(movementDirection.X) < AllowedDragDirectionOffset;
			return Direction == DragDirection.Free;
		}

		private const float AllowedDragDirectionOffset = 0.01f;
	}
}