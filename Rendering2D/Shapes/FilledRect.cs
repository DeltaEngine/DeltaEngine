using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// A filled solid color 2D rectangle to be rendered.
	/// </summary>
	public class FilledRect : Polygon2D
	{
		public FilledRect(Rectangle drawArea, Color color)
			: base(drawArea, color)
		{
			UpdateCorners();
		}

		private void UpdateCorners()
		{
			Points.AddRange(DrawArea.GetRotatedRectangleCorners(RotationCenter, Rotation));
		}

		protected override void OnRotationChanged()
		{
			Points.Clear();
			UpdateCorners();
		}
	}
}