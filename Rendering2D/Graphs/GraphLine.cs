using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering2D.Graphs
{
	/// <summary>
	/// A line on a graph: consists of a list of points and a list of lines connecting them.
	/// </summary>
	public class GraphLine
	{
		internal GraphLine(Graph graph)
		{
			this.graph = graph;
		}

		internal readonly Graph graph;

		public void AddValue(float value)
		{
			AddValue(1.0f, value);
		}

		public void AddValue(float interval, float value)
		{
			float x = points.Count == 0 ? -interval : points[points.Count - 1].X;
			AddPoint(new Vector2D(x + interval, value));
		}

		internal readonly List<Vector2D> points = new List<Vector2D>();

		public void AddPoint(Vector2D point)
		{
			viewport = graph.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			InsertPointIntoSequence(point);
			graph.AddPoint(point);
		}

		private Rectangle viewport;
		private Rectangle clippingBounds;
		private Rectangle drawArea;

		private static Vector2D ToQuadratic(Vector2D point, Rectangle viewport, Rectangle drawArea)
		{
			float borderWidth = viewport.Width * Graph.Border;
			float borderHeight = viewport.Height * Graph.Border;
			float x = (point.X - viewport.Left + borderWidth) / (viewport.Width + 2 * borderWidth);
			float y = (point.Y - viewport.Top + borderHeight) / (viewport.Height + 2 * borderHeight);
			return new Vector2D(drawArea.Left + x * drawArea.Width, drawArea.Bottom - y * drawArea.Height);
		}

		private void InsertPointIntoSequence(Vector2D point)
		{
			if (points.Count == 0 || points[points.Count - 1].X <= point.X)
				AddPointToEnd(point);
			else
				AddPointToMiddle(point);
		}

		private void AddPointToEnd(Vector2D point)
		{
			points.Add(point);
			if (points.Count <= 1)
				return;
			var line = new Line2D(ToQuadratic(points[points.Count - 2], viewport, drawArea),
				ToQuadratic(point, viewport, drawArea), Color);
			line.Clip(clippingBounds);
			lines.Add(line);
		}

		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				foreach (Line2D line in lines)
					line.Color = color;
				graph.RefreshKey();
			}
		}

		private Color color = Color.Blue;
		internal readonly List<Line2D> lines = new List<Line2D>();

		private void AddPointToMiddle(Vector2D point)
		{
			for (int index = 0; index < points.Count; index++)
				if (points[index].X > point.X)
				{
					InsertPointAt(point, index);
					return;
				}
		}

		private void InsertPointAt(Vector2D point, int index)
		{
			if (index > 0)
				MoveLineEndpoint(point, index);
			var line = new Line2D(ToQuadratic(point, viewport, drawArea),
				ToQuadratic(points[index], viewport, drawArea), Color);
			line.Clip(clippingBounds);
			lines.Insert(index, line);
			points.Insert(index, point);
		}

		private void MoveLineEndpoint(Vector2D point, int index)
		{
			Line2D line = lines[index - 1];
			line.EndPoint = ToQuadratic(point, viewport, drawArea);
			line.Clip(clippingBounds);
		}

		public void RemoveAt(int index)
		{
			if (index > 0 && index < points.Count - 1)
				lines[index - 1].EndPoint = lines[index].EndPoint;
			if (index <= lines.Count - 1)
				RemoveLine(index);
			else
				RemoveLine(index - 1);
			points.RemoveAt(index);
		}

		private void RemoveLine(int index)
		{
			lines[index].IsActive = false;
			lines.RemoveAt(index);
		}

		public void Clear()
		{
			points.Clear();
			foreach (Line2D line in lines)
				line.IsActive = false;
			lines.Clear();
		}

		public void Refresh()
		{
			viewport = graph.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			for (int i = 1; i < points.Count; i++)
				UpdateLine(i);
			if (lines.Count > 0)
				for (int i = lines.Count - 1; i >= points.Count - 1; i--)
					RemoveLine(i); //ncrunch: no coverage
		}

		private void UpdateLine(int i)
		{
			Line2D line = lines[i - 1];
			line.StartPoint = ToQuadratic(points[i - 1], viewport, drawArea);
			line.EndPoint = ToQuadratic(points[i], viewport, drawArea);
			line.RenderLayer = graph.RenderLayer + RenderLayerOffset;
			line.Clip(clippingBounds);
			if (!graph.IsVisible)
				line.IsVisible = false;
		}

		private const int RenderLayerOffset = 3;

		public string Key
		{
			get { return key; }
			set
			{
				key = value;
				graph.RefreshKey();
			}
		}

		private string key = "";
	}
}