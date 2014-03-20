using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Editor.Core;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Scenes.Controls;
using GalaSoft.MvvmLight.Messaging;

namespace DeltaEngine.Editor.UIEditor
{
	public class ControlTransformer
	{
		public ControlTransformer(Service service)
		{
			uiMouseCursor = new MouseCursor(service) { UpdatePriority = Priority.First };
			isTransforming = false;
			Messenger.Default.Register<string>(this, "SetDefaultCursor", SetDefaultCursor);
			Messenger.Default.Register<string>(this, "AllowChangeOfCursor", AllowChangeOfCursor);
			canChangeCursor = false;
		}

		private static void SetDefaultCursor(string obj)
		{
			canChangeCursor = false;
		}

		private static bool canChangeCursor;

		private static void AllowChangeOfCursor(string obj)
		{
			canChangeCursor = true;
		}

		public MouseCursor uiMouseCursor;

		public class MouseCursor : Entity, Updateable
		{
			public MouseCursor(Service service)
			{
				this.service = service;
			}

			public string CurrentCursor;
			internal List<Entity2D> selectedControlList;
			internal Vector2D mousePos;
			private readonly Service service;
			public ControlProcessor controlProcessor;

			public void ChangeCursorIfCanTransform()
			{
				if (selectedControlList == null || isTransforming || !canChangeCursor)
					return;
				service.Viewport.Window.SetCursorIcon();
				foreach (var control in selectedControlList)
				{
					if (CheckIfCanChangeCursorToScale(control))
						break;
					if (CheckIfCanChangeCursorToRotate(control))
						break;
					if (CheckIfCanChangeCursorToMove(control))
						break;
				}
			}

			private bool CheckIfCanChangeCursorToMove(Entity2D control)
			{
				if (!control.DrawArea.Contains(mousePos))
					return false;
				service.Viewport.Window.SetCursorIcon(MoveCursor);
				CurrentCursor = MoveCursor;
				usedControl = (Control)control;
				return true;
			}

			private bool CheckIfCanChangeCursorToScale(Entity2D control)
			{
				if (!controlProcessor.GizmoList[0].DrawArea.Contains(mousePos))
					return false;
				service.Viewport.Window.SetCursorIcon(ScaleCursor);
				CurrentCursor = ScaleCursor;
				usedControl = (Control)control;
				return true;
			}

			private bool CheckIfCanChangeCursorToRotate(Entity2D control)
			{
				if (!controlProcessor.GizmoList[1].DrawArea.Contains(mousePos))
					return false;
				service.Viewport.Window.SetCursorIcon(RotateCursor);
				CurrentCursor = RotateCursor;
				usedControl = (Control)control;
				return true;
			}

			public void Update()
			{
				ChangeCursorIfCanTransform();
			}
		}

		public const string MoveCursor = "../../Images/UIEditor/TransformGizmo.cur";
		public const string RotateCursor = "../../Images/UIEditor/RotateGizmo.cur";
		public const string ScaleCursor = "../../Images/UIEditor/ScaleGizmo.cur";
		public static Control usedControl;

		public class MouseCursorUpdater : UpdateBehavior
		{
			//ncrunch: no coverage start
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
					(entity as MouseCursor).ChangeCursorIfCanTransform();
			} //ncrunch: no coverage end
		}

		public void ChangeCursorIfCanTransform(List<Entity2D> selectedEntity2DList, Vector2D mousePos,
			ControlProcessor processor)
		{
			uiMouseCursor.mousePos = mousePos;
			uiMouseCursor.selectedControlList = selectedEntity2DList;
			uiMouseCursor.controlProcessor = processor;
		}

		public void RotateControl(Vector2D position, List<Entity2D> selectedControlList)
		{
			if (CheckIfAlreadyTransforming(position))
				return;
			var lastPosVector = CalculateBoundingBox(selectedControlList).Center - lastPosition;
			var newPosVector = CalculateBoundingBox(selectedControlList).Center - position;
			var angle = ChangeAngleIfRotatingLeft(lastPosVector, newPosVector);
			foreach (var control in selectedControlList)
			{
				control.RotationCenter = CalculateBoundingBox(selectedControlList).Center;
				control.Rotation += angle;
			}
			lastPosition = position;
		}

		private bool CheckIfAlreadyTransforming(Vector2D position)
		{
			if (isTransforming)
				return false;
			lastPosition = position;
			isTransforming = true;
			return true;
		}

		private Vector2D lastPosition;
		public static bool isTransforming;

		private static float ChangeAngleIfRotatingLeft(Vector2D lastPosVector, Vector2D newPosVector)
		{
			var angle = lastPosVector.AngleBetweenVector(newPosVector);
			var cross = Vector3D.Cross(lastPosVector, newPosVector);
			var dot = Vector3D.Dot(cross, new Vector3D(0, 0, 1));
			return dot < 0 ? -angle : angle;
		}

		public void ScaleControl(Vector2D position, List<Entity2D> selectedControlList)
		{
			if (CheckIfAlreadyTransforming(position))
				return;
			var lastPosVector = CalculateBoundingBox(selectedControlList).Center - lastPosition;
			var newPosVector = CalculateBoundingBox(selectedControlList).Center - position;
			var distanceX = lastPosVector.X - newPosVector.X;
			var distanceY = newPosVector.Y - lastPosVector.Y;
			foreach (var control in selectedControlList)
				control.DrawArea =
					new Rectangle(
						new Vector2D(control.DrawArea.Left - (distanceX), control.DrawArea.Top - distanceY),
						new Size(control.DrawArea.Width + (distanceX * 2),
							control.DrawArea.Height + (distanceY * 2)));
			lastPosition = position;
		}

		public Rectangle CalculateBoundingBox(List<Entity2D> selectedControlList)
		{
			if (selectedControlList.Count == 0)
				return Rectangle.Zero;
			var rect =
				selectedControlList[0].DrawArea.GetBoundingBoxAfterRotation(selectedControlList[0].Rotation);
			for (int i = 1; i < selectedControlList.Count; i++)
				rect =
					rect.Merge(
						selectedControlList[i].DrawArea.GetBoundingBoxAfterRotation(
							selectedControlList[i].Rotation));
			return rect;
		}
	}
}