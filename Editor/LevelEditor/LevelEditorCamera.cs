using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;
using DeltaEngine.Input;
using DeltaEngine.Rendering3D.Cameras;

namespace DeltaEngine.Editor.LevelEditor
{
	public class LevelEditorCamera : FreeCamera
	{
		public LevelEditorCamera(Device device, Window window)
			: base(device, window)
		{
			new Command(Command.Drag,
				(startPos, currentPos, isDragDone) => RotateByDrag(startPos, currentPos)).Add(
					new MouseDragTrigger(MouseButton.Right));
			new Command(Command.Zoom, Zoom);
			//new Command(Command.Drag,
			//	(startPos, currentPos, isDragDone) => PanByDrag(startPos, currentPos)).Add(
			//		new MouseDragTrigger(MouseButton.Middle));
		}

		private void RotateByDrag(Vector2D startPos, Vector2D currentPos)
		{
			Vector2D rotationDifference = (currentPos - startPos) * RotationSpeed;
			Matrix rotationMatrix = Matrix.CreateRotationZYX(Position.X, Position.Y, Position.Z);
			Position = rotationMatrix.TransformNormal(new Vector3D(0, 0, 1));
			//var curPos = Position;
			//Position = Vector3D.Zero;
			AdjustPitch(rotationDifference.Y);
			AdjustRoll(rotationDifference.X);
			//Position = curPos;
		}

		private const float RotationSpeed = 10;

		private void Zoom(float amount)
		{
			LookVector = Position - Vector3D.One;
			Vector3D currentLookVector = LookVector;
			float lookDistance = currentLookVector.Length;
			if (amount > lookDistance)
				amount = lookDistance - MathExtensions.Epsilon;
			Position = Position + currentLookVector / lookDistance * -amount;
		}

		private Vector3D LookVector
		{
			get { return lookVector; }
			set { lookVector = value; }
		}

		private Vector3D lookVector;

		private void PanByDrag(Vector2D startPos, Vector2D currentPos)
		{
			Vector2D moveDifference = (currentPos - startPos) * MoveSpeed;
			MoveUp(moveDifference.Y);
			MoveLeft(moveDifference.X);
		}

		private const float MoveSpeed = 30;
	}
}