using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Rendering2D.Graphs
{
	/// <summary>
	/// Labels at fixed vertical intervals to the right of the graph - eg. if there were five
	/// percentiles there's be six lines at 0%, 20%, 40%, 60%, 80% and 100% of the maximum value.
	/// </summary>
	internal class RenderPercentileLabels
	{
		public void Refresh(Graph graph)
		{
			ClearOldPercentileLabels();
			if (graph.IsVisible && IsVisible)
				CreateNewPercentileLabels(graph);
		}

		public bool IsVisible { get; set; }

		private void ClearOldPercentileLabels()
		{
			foreach (FontText percentileLabel in PercentileLabels)
				percentileLabel.IsActive = false;
			PercentileLabels.Clear();
		}

		public readonly List<FontText> PercentileLabels = new List<FontText>();

		private void CreateNewPercentileLabels(Graph graph)
		{
			for (int i = 0; i <= NumberOfPercentiles; i++)
				CreatePercentileLabel(graph, i);
		}

		public int NumberOfPercentiles { get; set; }

		private void CreatePercentileLabel(Graph graph, int index)
		{
			PercentileLabels.Add(new FontText(Font.Default, GetPercentileLabelText(graph, index),
				GetPercentileLabelDrawArea(graph, index))
			{
				RenderLayer = graph.RenderLayer + RenderLayerOffset,
				Color = PercentileLabelColor,
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
			});
		}

		private string GetPercentileLabelText(Graph graph, int index)
		{
			float value = graph.Viewport.Top + index * graph.Viewport.Height / NumberOfPercentiles;
			if (ArePercentileLabelsInteger)
				value = (int)value;
			return PercentilePrefix + value + PercentileSuffix;
		}

		private Rectangle GetPercentileLabelDrawArea(Entity2D graph, int index)
		{
			float borderWidth = graph.DrawArea.Width * Graph.Border;
			float x = graph.DrawArea.Right + 2 * borderWidth;
			float borderHeight = graph.DrawArea.Height * Graph.Border;
			float interval = (graph.DrawArea.Height - 2 * borderHeight) / NumberOfPercentiles;
			float bottom = graph.DrawArea.Bottom - borderHeight;
			float y = bottom - index * interval;
			return new Rectangle(x, y - interval / 2, 1.0f, interval);
		}

		public bool ArePercentileLabelsInteger;
		public string PercentilePrefix = "";
		public string PercentileSuffix = "";
		public Color PercentileLabelColor = Color.White;
		private const int RenderLayerOffset = 2;
	}
}