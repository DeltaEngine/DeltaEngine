using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides the mouse position, mouse button states and allows to set the mouse position.
	/// </summary>
	public abstract class Mouse : InputDevice
	{
		public Vector2D Position { get; protected set; }
		/// <summary>
		/// Total accumulated mouse scroll wheel value. Compare with last frame to see the change (see
		/// MouseZoomTrigger). Will not use multipliers like 120 to report one mouse wheel up change.
		/// One mouse wheel up change results in a value change of +1, down is -1.
		/// </summary>
		public int ScrollWheelValue { get; protected set; }
		public State LeftButton { get; protected set; }
		public State MiddleButton { get; protected set; }
		public State RightButton { get; protected set; }
		public State X1Button { get; protected set; }
		public State X2Button { get; protected set; }

		public State GetButtonState(MouseButton button)
		{
			if (button == MouseButton.Right)
				return RightButton;
			if (button == MouseButton.Middle)
				return MiddleButton;
			if (button == MouseButton.X1)
				return X1Button;
			return button == MouseButton.X2 ? X2Button : LeftButton;
		}

		public abstract void SetNativePosition(Vector2D position);

		public override void Update(IEnumerable<Entity> entities)
		{
			if (IsAvailable)
				foreach (Entity entity in entities)
					InvokeTriggersForEntity(entity);
		}

		private void InvokeTriggersForEntity(Entity entity)
		{
			HandleTriggerOfType<MouseButtonTrigger>(entity);
			HandleTriggerOfType<MouseDoubleClickTrigger>(entity);
			HandleTriggerOfType<MouseDragTrigger>(entity);
			HandleTriggerOfType<MouseDragDropTrigger>(entity);
			HandleTriggerOfType<MouseHoldTrigger>(entity);
			HandleTriggerOfType<MouseHoverTrigger>(entity);
			HandleTriggerOfType<MouseMovementTrigger>(entity);
			HandleTriggerOfType<MousePositionTrigger>(entity);
			HandleTriggerOfType<MouseTapTrigger>(entity);
			HandleTriggerOfType<MouseZoomTrigger>(entity);
			HandleTriggerOfType<MouseFlickTrigger>(entity);
		}

		private void HandleTriggerOfType<T>(Entity e) where T : class, MouseTrigger
		{
			var trigger = e as T;
			if (trigger != null)
				trigger.HandleWithMouse(this);
		}
	}
}