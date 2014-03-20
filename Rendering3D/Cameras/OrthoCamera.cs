using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Graphics;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering3D.Cameras
{
	/// <summary>
	/// Orthogonal 3D camera.
	/// </summary>
	public class OrthoCamera : TargetedCamera
	{
		public OrthoCamera(Device device, Window window, ScreenSpace screenSpace)
			: base(device, window)
		{
			this.screenSpace = screenSpace;
			ZoomLevel = 1.0f;
		}

		private ScreenSpace screenSpace;

		public float ZoomLevel
		{
			get { return zoomLevel; }
			set
			{
				zoomLevel = value;
				UpdateCameraProjectionSize();
			}
		}
		private float zoomLevel;

		private void UpdateCameraProjectionSize()
		{
			var width = screenSpace.Viewport.Width;
			var height = screenSpace.Viewport.Height;
			size = new Size(width, height);
			size /= zoomLevel;
			ForceProjectionMatrixUpdate();
		}

		private Size size;

		public void Zoom(float zoom)
		{
			var totalZoom = ZoomLevel + (zoom * ZoomSmoothFactor);
			if (totalZoom >= MaxZoom || totalZoom <= MinZoom)
				return;
			ZoomLevel = totalZoom;
		}

		public float ZoomSmoothFactor { get; set; }
		public float MaxZoom { get; set; }
		public float MinZoom { get; set; }

		public override Vector3D Position
		{
			get { return base.Position; }
			set
			{
				base.Position = value;
				ForceProjectionMatrixUpdate();
			}
		}

		protected override Matrix GetCurrentProjectionMatrix()
		{
			return Matrix.CreateOrthoProjection(size, NearPlane, FarPlane);
		}

		public override void ResetDefaults()
		{
			screenSpace = ScreenSpace.Current;
			ZoomLevel = 1.0f;
		}

		protected override void OnViewportSizeChanged(Size newSize)
		{
			base.OnViewportSizeChanged(newSize);
			UpdateCameraProjectionSize();
		}

		public void ViewPanning(Vector2D horizontalMoveDistance)
		{
			float rotationAroundZ = -(Target - Position).GetVector2D().GetRotation();
			var moveDistance3D = new Vector3D(horizontalMoveDistance).RotateAround(Vector3D.UnitZ,
				rotationAroundZ);
			Target += moveDistance3D;
			Position += moveDistance3D;
		}
	}
}