using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Shapes;

namespace DeltaEngine.Editor.LevelEditor
{
	public class CameraOrientationGizmo
	{
		public CameraOrientationGizmo()
		{
			x = new Line3D(Vector3D.Zero, Vector3D.UnitX, Color.Red);
			y = new Line3D(Vector3D.Zero, Vector3D.UnitY, Color.Green);
			z = new Line3D(Vector3D.Zero, Vector3D.UnitZ, Color.Blue);
			isVisible = true;
		}

		private readonly Line3D x;
		private readonly Line3D y;
		private readonly Line3D z;
		private bool isVisible;

		public bool IsVisible
		{
			get { return isVisible; }
			set
			{
				SetVisibility(value);
				isVisible = value;
			}
		}

		private void SetVisibility(bool setVisibility)
		{
			x.IsActive = setVisibility;
			y.IsActive = setVisibility;
			z.IsActive = setVisibility;
		}
	}
}