using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Shapes
{
	public class Grid2D : Entity2D
	{
		public Grid2D(Size dimension, Vector2D offset, float gridScale = 1.0f)
		{
			this.dimension = dimension;
			this.offset = offset;
			this.gridScale = gridScale;
			halfGridSize = dimension / 2;
			lines = new List<Line2D>();
			DrawGrid();
		}

		private Size dimension;
		private readonly Vector2D offset;
		private float gridScale;
		internal readonly List<Line2D> lines = new List<Line2D>();
		private Size halfGridSize;

		private void DrawGrid()
		{
			CreateHorizontalLines();
			CreateVerticalLines();
		}

		private void CreateHorizontalLines()
		{
			for (float y = -halfGridSize.Height; y < halfGridSize.Height + 1; y++)
				lines.Add(new Line2D(offset + new Vector2D(-halfGridSize.Width, y) * gridScale,
				 offset + new Vector2D(halfGridSize.Width, y) * gridScale, Color.White));
		}

		private void CreateVerticalLines()
		{
			for (float x = -halfGridSize.Width; x < halfGridSize.Width + 1; x++)
				lines.Add(new Line2D(offset + new Vector2D(x, -halfGridSize.Height) * gridScale,
					offset + new Vector2D(x, halfGridSize.Height) * gridScale, Color.White));
		}

		public Size Dimension
		{
			get { return dimension; }
			set
			{
				dimension = value;
				DrawGrid();
			}
		}

		public float GridScale
		{
			get { return gridScale; }
			set
			{
				gridScale = value;
				DrawGrid();
			}
		}

		public override bool IsActive
		{
			set
			{
				base.IsActive = value;
				foreach (Line2D line in lines)
					line.IsActive = value;
			}
		}

		//ncrunch: no coverage start
		public Vector2D GetTopLeft()
		{
			return lines[0].StartPoint;
		}

		public Size GetSize()
		{
			return (Size)(lines[lines.Count - 1].EndPoint - lines[0].StartPoint);
		}
	}
}