using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Enables a control to respond to mouse and touch input. Also processes controls anchored to
	/// other controls or the screen edge
	/// </summary>
	public class ControlUpdater : UpdateBehavior
	{
		public ControlUpdater()
			: base(Priority.High, false)
		{
			new Command(point => leftClick = point).Add(new MouseButtonTrigger()).Add(
				new TouchPressTrigger());
			new Command(point => leftRelease = point).Add(new MouseButtonTrigger(State.Releasing)).Add(
				new TouchPressTrigger(State.Releasing));
			new Command(point => movement = point).Add(new MouseMovementTrigger()).Add(
				new TouchMovementTrigger());
			new Command((start, end, done) =>
			{
				dragStart = start;
				dragEnd = end;
				dragDone = done;
			}).Add(new MouseDragTrigger()).Add(new TouchDragTrigger());
		}

		private Vector2D leftClick = Vector2D.Unused;
		private Vector2D leftRelease = Vector2D.Unused;
		private Vector2D movement = Vector2D.Unused;
		private Vector2D dragStart = Vector2D.Unused;
		private Vector2D dragEnd = Vector2D.Unused;
		private bool dragDone;

		public override void Update(IEnumerable<Entity> entities)
		{
			if (dragStart == Vector2D.Unused)
				ResetDrag(entities);
			if (DidATriggerFireThisFrame())
				UpdateStateOfControls(entities);
			ProcessShiftOfFocus(entities);
			Reset();
			ProcessAnchoring(entities);
		}

		private static void ResetDrag(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>())
				ResetDragForControl(control.State);
		}

		private static void ResetDragForControl(InteractiveState state)
		{
			state.DragStart = Vector2D.Unused;
			state.DragEnd = Vector2D.Unused;
			state.DragDone = false;
		}

		private bool DidATriggerFireThisFrame()
		{
			return leftClick != Vector2D.Unused || leftRelease != Vector2D.Unused ||
				movement != Vector2D.Unused || dragStart != Vector2D.Unused;
		}

		private void UpdateStateOfControls(IEnumerable<Entity> entities)
		{
			foreach (Control control in entities.OfType<Control>().Where(
				control => control.IsEnabled && control.IsVisible))
				UpdateState(control, control.State);
		}

		private void UpdateState(Control control, InteractiveState state)
		{
			ProcessAnyLeftClick(control, state);
			ProcessAnyLeftRelease(control, state);
			ProcessAnyMovement(control, state);
			ProcessAnyDrag(state);
		}

		private void ProcessAnyLeftClick(Control control, InteractiveState state)
		{
			if (leftClick != Vector2D.Unused)
				state.IsPressed = control.RotatedDrawAreaContains(leftClick);
		}

		private void ProcessAnyLeftRelease(Control control, InteractiveState state)
		{
			if (leftRelease == Vector2D.Unused)
				return;
			if (state.IsPressed && control.RotatedDrawAreaContains(leftRelease) && control.IsVisible)
				ClickControl(control, state);
			state.IsPressed = false;
		}

		private static void ClickControl(Control control, InteractiveState state)
		{
			control.Click();
			if (state.CanHaveFocus)
				state.WantsFocus = true;
		}

		private void ProcessAnyMovement(Control control, InteractiveState state)
		{
			if (movement == Vector2D.Unused)
				return;
			state.IsInside = control.RotatedDrawAreaContains(movement);
			Vector2D rotatedMovement = control.Rotation == 0.0f
				? movement : movement.RotateAround(control.RotationCenter, -control.Rotation);
			state.RelativePointerPosition = control.DrawArea.GetRelativePoint(rotatedMovement);
		}

		private void ProcessAnyDrag(InteractiveState state)
		{
			if (dragStart == Vector2D.Unused)
				return;
			state.DragStart = dragStart;
			state.DragEnd = dragEnd;
			state.DragDone = dragDone;
		}

		private static void ProcessShiftOfFocus(IEnumerable<Entity> entities)
		{
			var entityWhichWantsFocus =
				entities.FirstOrDefault(entity => entity.Get<InteractiveState>().WantsFocus);
			if (entityWhichWantsFocus == null)
				return;
			foreach (var state in entities.Select(entity => entity.Get<InteractiveState>()))
			{
				state.WantsFocus = false;
				state.HasFocus = false;
			}
			entityWhichWantsFocus.Get<InteractiveState>().HasFocus = true;
		}

		private void Reset()
		{
			leftClick = Vector2D.Unused;
			leftRelease = Vector2D.Unused;
			movement = Vector2D.Unused;
			dragStart = Vector2D.Unused;
			dragDone = false;
		}

		internal static void ProcessAnchoring(IEnumerable<Entity> entities)
		{
			int level = 0;
			do { } while (RefreshControlsAtLevel(entities, level++));
		}

		private static bool RefreshControlsAtLevel(IEnumerable<Entity> entities, int level)
		{
			bool wereAnyRefreshed = false;
			foreach (Control control in entities.OfType<Control>())
				wereAnyRefreshed |= RefreshControlIfAtCurrentLevel(control, level);
			return wereAnyRefreshed;
		}

		private static bool RefreshControlIfAtCurrentLevel(Control control, int level)
		{
			if (control.GetLevel() != level)
				return false;
			control.RefreshDrawAreaIfAnchored();
			return true;
		}
	}
}