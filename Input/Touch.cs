using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace DeltaEngine.Input
{
	/// <summary>
	/// Provides a way to fetch the current input values from a Touch device.
	/// </summary>
	public abstract class Touch : InputDevice
	{
		public abstract Vector2D GetPosition(int touchIndex);
		public abstract State GetState(int touchIndex);

		public override void Update(IEnumerable<Entity> entities)
		{
			if (IsAvailable)
				foreach (Entity entity in entities)
					InvokeTriggersForEntity(entity);
		}

		private void InvokeTriggersForEntity(Entity entity)
		{
			HandleTriggerOfType<TouchPressTrigger>(entity);
			HandleTriggerOfType<TouchDoubleTapTrigger>(entity);
			HandleTriggerOfType<TouchMovementTrigger>(entity);
			HandleTriggerOfType<TouchPositionTrigger>(entity);
			HandleTriggerOfType<TouchTapTrigger>(entity);
			HandleTriggerOfType<TouchDragTrigger>(entity);
			HandleTriggerOfType<TouchDualDragTrigger>(entity);
			HandleTriggerOfType<TouchDragDropTrigger>(entity);
			HandleTriggerOfType<TouchHoldTrigger>(entity);
			HandleTriggerOfType<TouchPinchTrigger>(entity);
			HandleTriggerOfType<TouchRotateTrigger>(entity);
			HandleTriggerOfType<TouchFlickTrigger>(entity);
		}

		private void HandleTriggerOfType<T>(Entity e) where T : class, TouchTrigger
		{
			var trigger = e as T;
			if (trigger != null)
				trigger.HandleWithTouch(this);
		}
	}
}