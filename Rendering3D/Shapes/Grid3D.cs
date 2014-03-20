using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering3D.Shapes
{
	public class Grid3D : Entity3D
	{
		public Grid3D(Size dimension, float gridScale = 1.0f)
			: this(Vector3D.Zero, dimension, gridScale) {}

		public Grid3D(Vector3D center, Size dimension, float gridScale = 1.0f)
			: base(center)
		{
			this.dimension = dimension;
			this.gridScale = gridScale;
			lines = new List<Line3D>();
			DrawGrid();
		}

		private Size dimension;
		private float gridScale;
		internal readonly List<Line3D> lines = new List<Line3D>();

		private void DrawGrid()
		{
			CreateVerticalLines();
			CreateHorizontalLines();
		}

		private void CreateHorizontalLines()
		{
			var initialPoint = Position - new Vector3D(dimension / 2);
			for (int y = 0; y < dimension.Height + 1; y++)
				lines.Add(new Line3D((initialPoint + new Vector3D(0, y, 0)) * gridScale,
					(initialPoint + new Vector3D(dimension.Width, y, 0)) * gridScale, Color.White));
		}

		private void CreateVerticalLines()
		{
			var initialPoint = Position - new Vector3D(dimension / 2);
			for (int x = 0; x < dimension.Width + 1; x++)
				lines.Add(new Line3D((initialPoint + new Vector3D(x, 0, 0)) * gridScale,
					(initialPoint + new Vector3D(x, dimension.Height, 0)) * gridScale, Color.White));
		}

		public float GridScale
		{
			get { return gridScale; }
			set
			{
				gridScale = value;
				foreach (var line in lines)
					line.Dispose();
				DrawGrid();
			}
		}

		public Size Dimension
		{
			get { return dimension; }
			set
			{
				dimension = value;
				foreach (var line in lines)
					line.Dispose();
				DrawGrid();
			}
		}

		public new Vector3D Position
		{
			get { return base.Position; }
			set
			{
				var difference = value - base.Position;
				foreach (var line in lines)
				{
					line.StartPoint += difference;
					line.EndPoint += difference;
				}
				base.Position = value;
			}
		}

		public override bool IsActive
		{
			set
			{
				base.IsActive = value;
				foreach (Line3D line in lines)
					line.IsActive = value;
			}
		}

		//ncrunch: no coverage start
		public override void ToggleVisibility()
		{
			base.ToggleVisibility();
			foreach (Line3D line in lines)
				line.IsVisible = IsVisible;
		}
	}
}