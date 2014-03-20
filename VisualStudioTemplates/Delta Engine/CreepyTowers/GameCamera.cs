using CreepyTowers.Content;
using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D.Cameras;

namespace $safeprojectname$
{
	public class GameCamera : Entity
	{
		public GameCamera(float minZoom = 1.0f, float maxZoom = 1.0f,
			float zoomSmoothingFactor = 0.1f)
		{
			SetDefaults();
			CreateGameCamera(minZoom, maxZoom, zoomSmoothingFactor);
		}

		private void SetDefaults()
		{
			zAxisRotation = 45.0f;
			transformedPosition = DefaultPostion.RotateAround(Vector2D.Zero, zAxisRotation);
			MaximumTurnAngle = 45.0f;
			RotationPerSecond = 80.0f;
		}

		private float zAxisRotation;
		private Vector2D transformedPosition;
		public float MaximumTurnAngle { get; set; }
		public float RotationPerSecond { get; set; }

		private void CreateGameCamera(float minZoom, float maxZoom, float zoomSmoothingFactor)
		{
			orthoCamera = Camera.Use<OrthoCamera>();
			orthoCamera.Position = new Vector3D(transformedPosition, CameraHeight);
			orthoCamera.MinZoom = minZoom;
			orthoCamera.MaxZoom = maxZoom;
			orthoCamera.ZoomSmoothFactor = zoomSmoothingFactor;
			orthoCamera.ZoomLevel = minZoom;
			moveCommand = new Command(GameCommands.ViewPanning.ToString(), MoveCameraByMouse);
			zoomCommand = new Command(GameCommands.ViewZooming.ToString(), orthoCamera.Zoom);
			turnRightCommand = new Command(GameCommands.TurnViewRight.ToString(), TurnRight);
			turnLeftCommand = new Command(GameCommands.TurnViewLeft.ToString(), TurnLeft);
		}

		private OrthoCamera orthoCamera;
		internal static readonly Vector2D DefaultPostion = new Vector2D(0.0f, -5.9f);
		internal const float CameraHeight = 4.3f;
		private Command zoomCommand, moveCommand, turnRightCommand, turnLeftCommand;

		public void MoveCameraByMouse(Vector2D formerPosition, Vector2D nextPosition, bool dragEnd)
		{
			var mouseDistance = nextPosition - formerPosition;
			mouseDistance.X *= -SpeedFactor;
			mouseDistance.Y *= SpeedFactor;
			mouseDistance = mouseDistance.RotateAround(Vector2D.Zero, zAxisRotation);
			var expectedPosition = (orthoCamera.Target + mouseDistance).GetVector2D();
			if (!AllowedMovementRect.Contains(expectedPosition))
				return;
			orthoCamera.Position += mouseDistance;
			orthoCamera.Target += mouseDistance;
		}

		private const float SpeedFactor = 5.0f;

		public Rectangle AllowedMovementRect { get; set; }

		private Vector2D horizontalTransformToTarget;

		private void TurnRight()
		{
			RotateCamera(RotationPerSecond * Time.Delta);
		}

		private void TurnLeft()
		{
			RotateCamera(-RotationPerSecond * Time.Delta);
		}

		private void RotateCamera(float amount)
		{
			zAxisRotation += amount;
			if (zAxisRotation >= MaximumTurnAngle)
			{
				zAxisRotation = MaximumTurnAngle;
				return;
			}

			if (zAxisRotation <= -MaximumTurnAngle)
			{
				zAxisRotation = -MaximumTurnAngle;
				return;
			}

			horizontalTransformToTarget = transformedPosition.RotateAround(Vector2D.Zero, zAxisRotation);
			var futurePosition = new Vector3D(horizontalTransformToTarget, CameraHeight);
			orthoCamera.Position = futurePosition;
		}

		public Vector3D Position
		{
			get { return orthoCamera.Position; }
			set { orthoCamera.Position = value; }
		}

		public float MaxZoom
		{
			get { return orthoCamera.MaxZoom; }
			set { orthoCamera.MaxZoom = value; }
		}

		public float MinZoom
		{
			get { return orthoCamera.MinZoom; }
			set { orthoCamera.MinZoom = value; }
		}
		public float ZoomSmoothFactor
		{
			get { return orthoCamera.ZoomSmoothFactor; }
			set { orthoCamera.ZoomSmoothFactor = value; }
		}

		public float ZoomLevel
		{
			get { return orthoCamera.ZoomLevel; }
			set { orthoCamera.ZoomLevel = value; }
		}

		public void ResetToMinZoom()
		{
			ZoomLevel = MinZoom;
		}

		public void ResetPositionToDefault()
		{
			Position = new Vector3D(transformedPosition, CameraHeight);
		}

		public override void Dispose()
		{
			if (moveCommand != null)
				moveCommand.IsActive = false;
			if (zoomCommand != null)
				zoomCommand.IsActive = false;
			if (turnLeftCommand != null)
				turnLeftCommand.IsActive = false;
			if (turnRightCommand != null)
				turnRightCommand.IsActive = false;
		}
	}
}