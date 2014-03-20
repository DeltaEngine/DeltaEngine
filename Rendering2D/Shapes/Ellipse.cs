using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Renders a filled 2D ellipse shape.
	/// </summary>
	public class Ellipse : Polygon2D
	{
		public Ellipse(Vector2D center, float radiusX, float radiusY, Color color)
			: this(Rectangle.FromCenter(center, new Size(2 * radiusX, 2 * radiusY)), color) {}

		public Ellipse(Rectangle drawArea, Color color)
			: base(drawArea, color)
		{
			Start<UpdatePointsIfRadiusChanges>();
		}

		public float RadiusX
		{
			get { return DrawArea.Width / 2.0f; }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(2 * value, drawArea.Height));
			}
		}

		public float RadiusY
		{
			get { return DrawArea.Height / 2.0f; }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(drawArea.Width, 2 * value));
			}
		}

		public float Radius
		{
			get { return MathExtensions.Max(RadiusX, RadiusY); }
			set
			{
				var drawArea = DrawArea;
				DrawArea = Rectangle.FromCenter(drawArea.Center, new Size(2 * value, 2 * value));
			}
		}

		private float lastRadius;

		public class UpdatePointsIfRadiusChanges : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var ellipse in entities.OfType<Ellipse>())
					if (!ellipse.lastRadius.IsNearlyEqual(ellipse.Radius))
					{
						ellipse.lastRadius = ellipse.Radius;
						InitializeVariables(ellipse);
						FormEllipsePoints(ellipse);
					}
			}

			private void InitializeVariables(Ellipse entity)
			{
				var rotation = entity.Rotation;
				rotationSin = MathExtensions.Sin(rotation);
				rotationCos = MathExtensions.Cos(rotation);
				rotationCenter = entity.RotationCenter;
				center = entity.DrawArea.Center;
				if (center != rotationCenter)
					center = center.RotateAround(rotationCenter, rotationSin, rotationCos);
				radiusX = entity.RadiusX;
				radiusY = entity.RadiusY;
				float maxRadius = MathExtensions.Max(radiusX, radiusY);
				pointsCount = GetPointsCount(maxRadius);
				theta = -360.0f / (pointsCount - 1);
			}

			private float rotationSin;
			private float rotationCos;
			private Vector2D center;
			private Vector2D rotationCenter;
			private float radiusX;
			private float radiusY;
			private int pointsCount;

			private static int GetPointsCount(float maxRadius)
			{
				var pointsCount = (int)(MaxPoints * MathExtensions.Max(0.22f + maxRadius / 2, maxRadius));
				return MathExtensions.Max(pointsCount, MinPoints);
			}

			private const int MinPoints = 5;
			private const int MaxPoints = 96;
			private float theta;

			private void FormEllipsePoints(DrawableEntity entity)
			{
				ellipsePoints = entity.Get<List<Vector2D>>();
				ellipsePoints.Clear();
				ellipsePoints.Add(center);
				for (int i = pointsCount - 1; i >= 0; i--)
					FormRotatedEllipsePoint(i);
				entity.SetWithoutInterpolation(ellipsePoints);
			}

			private List<Vector2D> ellipsePoints;

			private void FormRotatedEllipsePoint(int i)
			{
				var ellipsePoint = new Vector2D(radiusX * MathExtensions.Sin(i * theta),
					radiusY * MathExtensions.Cos(i * theta));
				ellipsePoints.Add(center +
					ellipsePoint.RotateAround(Vector2D.Zero, rotationSin, rotationCos));
			}
		}
	}
}