using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering2D.Graphs
{
	/// <summary>
	/// Renders a set of axes at the origin.
	/// </summary>
	internal class RenderAxes
	{
		public void Refresh(Graph graph)
		{
			if (graph.IsVisible && IsVisible)
				ShowAxes(graph);
			else
				HideAxes();
		}

		public bool IsVisible { get; set; }

		private void ShowAxes(Graph graph)
		{
			renderLayer = graph.RenderLayer + RenderLayerOffset;
			viewport = graph.Viewport;
			drawArea = graph.DrawArea;
			clippingBounds = Rectangle.FromCorners(
				ToQuadratic(viewport.BottomLeft, viewport, drawArea),
				ToQuadratic(viewport.TopRight, viewport, drawArea));
			Vector2D origin = graph.Origin;
			SetAxis(XAxis, ToQuadratic(new Vector2D(viewport.Left, origin.Y), viewport, drawArea),
				ToQuadratic(new Vector2D(viewport.Right, origin.Y), viewport, drawArea));
			SetAxis(YAxis, ToQuadratic(new Vector2D(origin.X, viewport.Top), viewport, drawArea),
				ToQuadratic(new Vector2D(origin.X, viewport.Bottom), viewport, drawArea));
		}

		private int renderLayer;
		private const int RenderLayerOffset = 2;
		private Rectangle viewport;
		private Rectangle drawArea;
		private Rectangle clippingBounds;

		public readonly Line2D XAxis = new Line2D(Vector2D.Zero, Vector2D.Zero, Color.White)
		{
			IsVisible = false
		};

		public readonly Line2D YAxis = new Line2D(Vector2D.Zero, Vector2D.Zero, Color.White)
		{
			IsVisible = false
		};

		private static Vector2D ToQuadratic(Vector2D point, Rectangle viewport, Rectangle drawArea)
		{
			float borderWidth = viewport.Width * Graph.Border;
			float borderHeight = viewport.Height * Graph.Border;
			float x = (point.X - viewport.Left + borderWidth) / (viewport.Width + 2 * borderWidth);
			float y = (point.Y - viewport.Top + borderHeight) / (viewport.Height + 2 * borderHeight);
			return new Vector2D(drawArea.Left + x * drawArea.Width, drawArea.Bottom - y * drawArea.Height);
		}

		private void SetAxis(Line2D axis, Vector2D startPoint, Vector2D endPoint)
		{
			axis.StartPoint = startPoint;
			axis.EndPoint = endPoint;
			axis.RenderLayer = renderLayer;
			axis.Clip(clippingBounds);
		}

		internal void HideAxes()
		{
			XAxis.IsVisible = false;
			YAxis.IsVisible = false;
		}
	}
}