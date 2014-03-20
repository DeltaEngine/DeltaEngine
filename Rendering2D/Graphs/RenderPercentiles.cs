using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering2D.Graphs
{
	/// <summary>
	/// Horizontal lines at fixed intervals - eg. if there were five percentiles there's be 
	/// six lines at 0%, 20%, 40%, 60%, 80% and 100% of the maximum value.
	/// </summary>
	internal class RenderPercentiles
	{
		public void Refresh(Graph graph)
		{
			ClearOldPercentiles();
			if (graph.IsVisible && IsVisible)
				CreateNewPercentiles(graph);
		}

		public bool IsVisible { get; set; }

		private void ClearOldPercentiles()
		{
			foreach (Line2D percentile in Percentiles)
			{
				percentile.IsVisible = false;
				line2DPool.Add(percentile);
			}
			Percentiles.Clear();
		}

		public readonly List<Line2D> Percentiles = new List<Line2D>();
		private readonly List<Line2D> line2DPool = new List<Line2D>();

		private void CreateNewPercentiles(Graph graph)
		{
			for (int i = 0; i <= NumberOfPercentiles; i++)
				CreatePercentile(graph, i);
		}

		public int NumberOfPercentiles { get; set; }

		private void CreatePercentile(Graph graph, int index)
		{
			Line2D percentile = CreateBlankPercentile();
			percentile.StartPoint = GetPercentileStartPoint(graph, index);
			percentile.EndPoint = GetPercentileEndPoint(graph, index);
			percentile.Color = PercentileColor;
			percentile.RenderLayer = graph.RenderLayer + RenderLayerOffset;
			percentile.IsVisible = true;
			Percentiles.Add(percentile);
		}

		public Color PercentileColor = Color.Gray;
		private const int RenderLayerOffset = 1;

		private Line2D CreateBlankPercentile()
		{
			Line2D percentile;
			if (line2DPool.Count > 0)
			{
				percentile = line2DPool[0];
				line2DPool.RemoveAt(0);
			}
			else
				percentile = new Line2D(Vector2D.Zero, Vector2D.Zero, Color.Black);
			return percentile;
		}

		private Vector2D GetPercentileStartPoint(Entity2D graph, int index)
		{
			float borderWidth = graph.DrawArea.Width * Graph.Border;
			float startX = graph.DrawArea.Left + borderWidth;
			return new Vector2D(startX, GetPercentileYCoordinate(graph, index));
		}

		private float GetPercentileYCoordinate(Entity2D graph, int index)
		{
			float borderHeight = graph.DrawArea.Height * Graph.Border;
			float interval = (graph.DrawArea.Height - 2 * borderHeight) / NumberOfPercentiles;
			float bottom = graph.DrawArea.Bottom - borderHeight;
			return bottom - index * interval;
		}

		private Vector2D GetPercentileEndPoint(Entity2D graph, int index)
		{
			float borderWidth = graph.DrawArea.Width * Graph.Border;
			float endX = graph.DrawArea.Right - borderWidth;
			return new Vector2D(endX, GetPercentileYCoordinate(graph, index));
		}
	}
}