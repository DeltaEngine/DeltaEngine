using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace $safeprojectname$
{
	internal class FloodFiller
	{
		public FloodFiller(Color[,] colors)
		{
			this.colors = colors;
			width = colors.GetLength(0);
			height = colors.GetLength(1);
		}

		private readonly Color[,] colors;
		private readonly int width;
		private readonly int height;

		public void SetColor(int x, int y, Color color)
		{
			oldColor = colors[x, y];
			newColor = color;
			processedPoints.Clear();
			if (oldColor == newColor)
				return;
			unprocessedPoints.Clear();
			unprocessedPoints.Add(new Vector2D(x, y));
			while (unprocessedPoints.Any())
				ProcessNextUnprocessedPoint();
		}

		private Color oldColor;
		private Color newColor;
		private readonly HashSet<Vector2D> processedPoints = new HashSet<Vector2D>();
		private readonly HashSet<Vector2D> unprocessedPoints = new HashSet<Vector2D>();

		private void ProcessNextUnprocessedPoint()
		{
			var point = unprocessedPoints.First();
			unprocessedPoints.Remove(point);
			processedPoints.Add(point);
			colors[(int)point.X, (int)point.Y] = newColor;
			foreach (var direction in Directions)
				ProcessNeighbour(point, direction);
		}

		private static IEnumerable<Vector2D> Directions
		{
			get { return new[] { -Vector2D.UnitX, Vector2D.UnitX, -Vector2D.UnitY, Vector2D.UnitY }; }
		}

private void ProcessNeighbour(Vector2D point, Vector2D direction)
		{
			var x = (int)point.X + (int)direction.X;
			var y = (int)point.Y + (int)direction.Y;
			if (x >= 0 && x < width && y >= 0 && y < height)
				ProcessValidNeighbour(new Vector2D(x, y), colors[x, y]);
		}

		private void ProcessValidNeighbour(Vector2D point, Color color)
		{
			if (!processedPoints.Contains(point) && color == oldColor)
				unprocessedPoints.Add(point);
		}

		public int ProcessedCount
		{
			get { return processedPoints.Count; }
		}
	}
}