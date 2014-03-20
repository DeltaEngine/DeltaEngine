using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// Basis for all cameras which point towards a target
	/// </summary>
	public abstract class TargetedCamera : Camera
	{
		protected TargetedCamera(Device device, Window window)
			: base(device, window) {}

		protected internal override Matrix GetCurrentViewMatrix()
		{
			return Matrix.CreateLookAt(Position, Target, UpDirection);
		}

		public virtual Vector3D Target
		{
			get { return GetFinalTargetPosition(); }
			set { targetPosition = value; }
		}

		private Vector3D targetPosition;

		protected virtual Vector3D GetFinalTargetPosition()
		{
			return targetPosition;
		}

		protected static readonly Vector3D UpDirection = Vector3D.UnitZ;
	}
}