using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// Like LookAtCamera but uses Orthographic projection and the look direction can never
	/// be changed from how it was set in the constructor. Moving forwards and backwards has no
	/// meaning for Orthographic, instead Zooming rescales the world
	/// </summary>
	public sealed class IsometricCamera : TargetedCamera
	{
		public IsometricCamera(Device device, Window window, Vector3D lookDirection)
			: base(device, window)
		{
			base.Position = -lookDirection;
			ResetZoom();
			leftDirection = Vector3D.Normalize(Vector3D.Cross(lookDirection, -UpDirection));
		}

		public IsometricCamera(Device device, Window window)
			: base(device, window)
		{
			base.Position = -Vector3D.UnitY;
			ResetZoom();
			leftDirection = Vector3D.Normalize(Vector3D.Cross(Vector3D.UnitY, -UpDirection));
		}

		private Vector3D leftDirection;

		public void ResetZoom()
		{
			ZoomScale = 1.0f;
			ForceProjectionMatrixUpdate();
		}

		public float ZoomScale { get; private set; }

		protected override Matrix GetCurrentProjectionMatrix()
		{
			return Matrix.CreateOrthoProjection(window.ViewportPixelSize * ZoomScale, NearPlane,
				FarPlane);
		}

		public void MoveUp(float distance)
		{
			Position += UpDirection * distance;
		}

		public override Vector3D Position
		{
			get { return base.Position; }
			set
			{
				base.Target += value - base.Position;
				base.Position = value;
			}
		}

		public override Vector3D Target
		{
			get { return base.Target; }
			set
			{
				base.Position += value - base.Target;
				base.Target = value;
			}
		}

		public void MoveDown(float distance)
		{
			Position -= UpDirection * distance;
		}

		public void MoveLeft(float distance)
		{
			Position += leftDirection * distance;
		}

		public void MoveRight(float distance)
		{
			Position -= leftDirection * distance;
		}

		public void Zoom(float amount)
		{
			ZoomScale /= amount;
			ForceProjectionMatrixUpdate();
		}

		public override void ResetDefaults()
		{
			base.Position = -Vector3D.UnitY;
			ResetZoom();
			leftDirection = Vector3D.Normalize(Vector3D.Cross(Vector3D.UnitY, -UpDirection));
		}
	}
}