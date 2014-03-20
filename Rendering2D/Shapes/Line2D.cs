using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Sets up an Entity that can be used in 2D line rendering.
	/// </summary>
	public class Line2D : Entity2D
	{
		public Line2D(Vector2D startPoint, Vector2D endPoint, Color color)
			: base(Rectangle.FromPoints(new List<Vector2D> { startPoint, endPoint }))
		{
			Add(new List<Vector2D> { startPoint, endPoint });
			Initialize(color);
		}

		private void Initialize(Color color)
		{
			Color = color;
			OnDraw<Line2DRenderer>();
		}

		//ncrunch: no coverage start
		public Line2D(List<Vector2D> points, Color color)
			: base(Rectangle.FromPoints(points))
		{
			Add(points);
			Initialize(color);
		} //ncrunch: no coverage end

		public Line2D(Rectangle rectangle, Color color)
			: base(rectangle)
		{
			Add(new List<Vector2D>
			{
				rectangle.TopLeft,
				rectangle.TopRight,
				rectangle.TopRight,
				rectangle.BottomRight,
				rectangle.BottomRight,
				rectangle.BottomLeft,
				rectangle.BottomLeft,
				rectangle.TopLeft
			});
			Initialize(color);
		}

		public List<Vector2D> Points
		{
			get { return Get<List<Vector2D>>(); }
			set
			{
				Set(value);
				base.DrawArea = Rectangle.FromPoints(value);
			}
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

		public Vector2D StartPoint
		{
			get { return Points[0]; }
			set
			{
				var points = Points;
				points[0] = value;
				base.DrawArea = Rectangle.FromPoints(points);
			}
		}

		public Vector2D EndPoint
		{
			get
			{
				var points = Points;
				return points[points.Count - 1];
			}
			set
			{
				var points = Points;
				points[points.Count - 1] = value;
				base.DrawArea = Rectangle.FromPoints(points);
			}
		}

		public Line2D AddLine(Vector2D startPoint, Vector2D endPoint)
		{
			var points = Points;
			points.Add(startPoint);
			points.Add(endPoint);
			base.DrawArea = Rectangle.FromPoints(points);
			return this;
		}

		public Line2D ExtendLine(Vector2D nextPoint)
		{
			var points = Points;
			points.Add(points[points.Count - 1]);
			points.Add(nextPoint);
			base.DrawArea = Rectangle.FromPoints(points);
			return this;
		}

		public void Clip(Rectangle clippingBounds)
		{
			var line = new ClippedLine(StartPoint, EndPoint, clippingBounds);
			StartPoint = line.StartPoint;
			EndPoint = line.EndPoint;
			IsVisible = line.IsVisible;
		}
	}
}