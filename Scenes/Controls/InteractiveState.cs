using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Holds information about the state of a control - eg. is the mouse/touch inside of it, or
	/// does the control have focus etc.
	/// </summary>
	public class InteractiveState
	{
		public bool IsInside { get; set; }
		public bool IsPressed { get; set; }
		public bool IsSelected { get; set; }
		public Vector2D RelativePointerPosition { get; set; }
		public bool CanHaveFocus { get; set; }
		public bool HasFocus { get; set; }
		public bool WantsFocus { get; set; }
		public Vector2D DragStart { get; set; }
		public Vector2D DragEnd { get; set; }
		public Vector2D DragDelta
		{
			get { return DragEnd - DragStart; }
		}
		public bool DragDone { get; set; }
	}
}