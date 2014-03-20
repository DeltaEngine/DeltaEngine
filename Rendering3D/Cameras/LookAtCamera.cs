using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// Basic camera for 3D which can zoom, move and rotate around the Target.
	/// </summary>
	public class LookAtCamera : TargetedCamera
	{
		public LookAtCamera(Device device, Window window)
			: base(device, window)
		{
			// ReSharper disable DoNotCallOverridableMethodsInConstructor
			//Position = Vector3D.One * 3.0f;
			Position = DefaultPosition;
			Target = Vector3D.Zero;
			UpdatePosition();
			new Command(Command.Zoom, Zoom);
			new Command(Command.MoveLeft, () => Move(-MoveSpeedPerSecond, 0));
			new Command(Command.MoveRight, () => Move(MoveSpeedPerSecond, 0));
			new Command(Command.MoveUp, () => Move(0, MoveSpeedPerSecond));
			new Command(Command.MoveDown, () => Move(0, -MoveSpeedPerSecond));
			new Command(Command.Drag,
				//ncrunch: no coverage start
				(startPos, currentPos, isDragDone) => RotateByDragCommand(currentPos, isDragDone));
			//ncrunch: no coverage end
		}

		private static readonly Vector3D DefaultPosition = new Vector3D(3.0f, -3.0f, 3.0f);
		private const float MoveSpeedPerSecond = 4;

		public override Vector3D Position
		{
			get { return base.Position; }
			set
			{
				base.Position = value;
				ComputeCameraRotation();
			}
		}

		public override Vector3D Target
		{
			get { return base.Target; }
			set
			{
				base.Target = value;
				ComputeCameraRotation();
			}
		}

		private void ComputeCameraRotation()
		{
			Vector3D lookVector = NormalizedLookVector;
			yawPitchRoll.Y = MathExtensions.Asin(-lookVector.Z);
			yawPitchRoll.X = -MathExtensions.Atan2(-lookVector.X, -lookVector.Y);
		}

		public Vector3D NormalizedLookVector
		{
			get { return Vector3D.Normalize(LookVector); }
		}

		private Vector3D LookVector
		{
			get { return Target - Position; }
		}

		private Vector3D yawPitchRoll;

		public Vector3D YawPitchRoll
		{
			get { return yawPitchRoll; }
			set
			{
				yawPitchRoll = value;
				UpdatePosition();
			}
		}

		private void UpdatePosition()
		{
			Matrix rotationMatrix = Matrix.CreateRotationX(yawPitchRoll.Y) *
				Matrix.CreateRotationZ(yawPitchRoll.X);
			float lookDistance = LookVector.Length;
			Position = Target + rotationMatrix.TransformNormal(new Vector3D(0, lookDistance, 0));
		}

		public void Zoom(float amount)
		{
			Vector3D lookVector = LookVector;
			float lookDistance = lookVector.Length;
			if (amount > lookDistance)
				amount = lookDistance - MathExtensions.Epsilon;
			Position = Position + lookVector / lookDistance * amount;
		}

		//ncrunch: no coverage start
		private void Move(float rightMovement, float forwardMovement)
		{
			Vector3D lookDirection = NormalizedLookVector;
			lookDirection.Z = 0;
			Vector3D forward = lookDirection * forwardMovement;
			Vector3D right = Vector3D.Cross(lookDirection, UpDirection) * rightMovement;
			Vector3D moveOffset = (forward + right) * Time.Delta;
			Target += moveOffset;
			Position += moveOffset;
		}

		private void RotateByDragCommand(Vector2D currentScreenPosition, bool isDragDone)
		{
			Vector2D moveDifference = currentScreenPosition - lastMovePosition;
			lastMovePosition = isDragDone ? Vector2D.Zero : currentScreenPosition;
			if ((moveDifference - currentScreenPosition).LengthSquared < 0.01f * 0.01f)
				return;
			Vector3D newYawPitchRoll = YawPitchRoll;
			newYawPitchRoll.X -= moveDifference.X * RotationSpeed;
			newYawPitchRoll.Y += moveDifference.Y * RotationSpeed;
			YawPitchRoll = newYawPitchRoll;
		}

		public override void ResetDefaults()
		{
			Position = DefaultPosition;
			Target = Vector3D.Zero;
			UpdatePosition();
		}

		private Vector2D lastMovePosition;
		private const float RotationSpeed = 240;
	}
}