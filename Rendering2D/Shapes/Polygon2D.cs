using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// A 2D shape to be rendered defined by its border points, will be rendered with a filled color.
	/// </summary>
	public class Polygon2D : Entity2D
	{
		public Polygon2D(Rectangle drawArea, Color color)
			: base(drawArea)
		{
			Color = color;
			Add(new List<Vector2D>());
			OnDraw<DrawPolygon2D>();
		}

		public List<Vector2D> Points
		{
			get { return Get<List<Vector2D>>(); }
			set
			{ //ncrunch: no coverage start
				Set(value);
				base.DrawArea = Rectangle.FromPoints(value);
			} //ncrunch: no coverage end
		}

		public override Rectangle DrawArea
		{
			get { return base.DrawArea; }
			set
			{
				var lastDrawArea = base.DrawArea;
				base.DrawArea = value;
				if (Contains<List<Vector2D>>())
					UpdatePointsAfterDrawAreaChanges(lastDrawArea, value);
			}
		}

		private void UpdatePointsAfterDrawAreaChanges(Rectangle lastDrawArea, Rectangle drawArea)
		{
			List<Vector2D> points = Points;
			for (int i = 0; i < points.Count; i++)
			{
				var relativePosition = new Vector2D(
					(points[i].X - lastDrawArea.Left) / lastDrawArea.Width,
					(points[i].Y - lastDrawArea.Top) / lastDrawArea.Height);
				points[i] = new Vector2D(drawArea.Left + relativePosition.X * drawArea.Width,
					drawArea.Top + relativePosition.Y * drawArea.Height);
			}
		}
	}
}