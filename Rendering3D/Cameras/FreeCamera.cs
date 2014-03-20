using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// A camera that can be moved in any direction and rotated around any axis
	/// </summary>
	public class FreeCamera : Camera
	{
		public FreeCamera(Device device, Window window)
			: base(device, window)
		{
			Rotation = Quaternion.Identity;
		}

		public Quaternion Rotation { get; private set; }

		public void MoveUp(float distance)
		{
			Move(Vector3D.UnitY * distance);
		}

		public void Move(Vector3D localAxisDistance)
		{
			Position += Rotation * localAxisDistance;
		}

		public void Move(Vector3D localAxis, float distance)
		{
			Move(localAxis * distance);
		}

		public void MoveDown(float distance)
		{
			Move(-Vector3D.UnitY * distance);
		}

		public void MoveLeft(float distance)
		{
			Move(-Vector3D.UnitX * distance);
		}

		public void MoveRight(float distance)
		{
			Move(Vector3D.UnitX * distance);
		}

		public void MoveForward(float distance)
		{
			Move(-Vector3D.UnitZ * distance);
		}

		public void MoveBackward(float distance)
		{
			Move(Vector3D.UnitZ * distance);
		}

		public void AdjustPitch(float degrees)
		{
			Rotate(Vector3D.UnitX, degrees);
		}

		public void Rotate(Vector3D localAxis, float degrees)
		{
			localAxis = Rotation * localAxis;
			Rotation = Quaternion.Normalize(Quaternion.FromAxisAngle(localAxis, degrees) * Rotation);
		}

		public void AdjustYaw(float degrees)
		{
			Rotate(Vector3D.UnitY, degrees);
		}

		public void AdjustRoll(float degrees)
		{
			Rotate(Vector3D.UnitZ, degrees);
		}

		public override void ResetDefaults() {}

		protected internal override Matrix GetCurrentViewMatrix()
		{
			return Matrix.Invert(Matrix.FromQuaternion(Rotation) *
				Matrix.CreateTranslation(Position.X, Position.Y, Position.Z));
		}
	}
}