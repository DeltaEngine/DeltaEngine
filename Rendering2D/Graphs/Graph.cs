using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering2D.Graphs
{
	/// <summary>
	/// Renders a graph with one or more lines at a given scale. Various logic can be turned on and
	/// off including autogrowing, auto-pruning and rendering axes and percentiles.
	/// </summary>
	public class Graph : FilledRect, Updateable
	{
		public Graph(Rectangle drawArea)
			: base(drawArea, HalfBlack) {}

		internal static Color HalfBlack
		{
			get { return new Color(0, 0, 0, 0.75f); }
		}

		public void Update()
		{
			if (DidFootprintChange)
				RefreshAll();
		}

		private void RefreshAll()
		{
			renderKey.Refresh(this);
			renderAxes.Refresh(this);
			renderPercentiles.Refresh(this);
			renderPercentileLabels.Refresh(this);
			foreach (GraphLine line in Lines)
				line.Refresh();
		}

		private readonly RenderKey renderKey = new RenderKey();
		private readonly RenderAxes renderAxes = new RenderAxes { IsVisible = false };
		internal readonly List<GraphLine> Lines = new List<GraphLine>();

		private readonly RenderPercentiles renderPercentiles = new RenderPercentiles
		{
			IsVisible = false
		};

		private readonly RenderPercentileLabels renderPercentileLabels = new RenderPercentileLabels
		{
			IsVisible = false
		};

		public override void ToggleVisibility()
		{
			base.ToggleVisibility();
			RefreshAll();
		}

		public GraphLine CreateLine(string key, Color color)
		{
			var line = new GraphLine(this) { Key = key, Color = color };
			Lines.Add(line);
			renderKey.Refresh(this);
			return line;
		}

		public void RemoveLine(GraphLine line)
		{
			line.Clear();
			Lines.Remove(line);
			renderKey.Refresh(this);
		}

		internal void AddPoint(Vector2D point)
		{
			removeOldestPoints.Process(this);
			if (IsAutogrowing)
				autogrowViewport.ProcessAddedPoint(this, point);
		}

		public bool IsAutogrowing { get; set; }
		private readonly AutogrowViewport autogrowViewport = new AutogrowViewport();

		internal void RefreshKey()
		{
			renderKey.Refresh(this);
		}

		public bool AxesIsVisible
		{
			get { return renderAxes.IsVisible; }
			set
			{
				if (renderAxes.IsVisible == value)
					return; //ncrunch: no coverage
				renderAxes.IsVisible = value;
				renderAxes.Refresh(this);
			}
		}

		public bool PercentilesIsVisible
		{
			get { return renderPercentiles.IsVisible; }
			set
			{
				if (renderPercentiles.IsVisible == value)
					return;
				renderPercentiles.IsVisible = value;
				renderPercentiles.Refresh(this);
			}
		}

		public bool PercentileLabelsIsVisible
		{
			get { return renderPercentileLabels.IsVisible; }
			set
			{
				if (renderPercentileLabels.IsVisible == value)
					return;
				renderPercentileLabels.IsVisible = value;
				renderPercentileLabels.Refresh(this);
			}
		}

		public bool KeyVisibility
		{
			get { return renderKey.IsVisible; }
			set
			{
				if (renderKey.IsVisible == value)
					return;
				renderKey.IsVisible = value;
				renderKey.Refresh(this);
			}
		}

		public Rectangle Viewport
		{
			get { return viewport; }
			set
			{
				if (viewport == value)
					return;
				viewport = value;
				renderAxes.Refresh(this);
				renderPercentileLabels.Refresh(this);
				foreach (GraphLine line in Lines)
					line.Refresh();
			}
		}
		private Rectangle viewport;

		public Color AxisColor
		{
			get { return renderAxes.XAxis.Color; }
			set
			{
				renderAxes.XAxis.Color = value;
				renderAxes.YAxis.Color = value;
			}
		}

		public Vector2D Origin
		{
			get { return origin; }
			set
			{
				if (origin == value)
					return;
				origin = value;
				renderAxes.Refresh(this);
			}
		}
		private Vector2D origin;

		public int MaximumNumberOfPoints
		{
			get { return removeOldestPoints.MaximumNumberOfPoints; }
			set
			{
				if (removeOldestPoints.MaximumNumberOfPoints == value)
					return;
				removeOldestPoints.MaximumNumberOfPoints = value;
				removeOldestPoints.Process(this);
			}
		}

		private readonly RemoveOldestPoints removeOldestPoints = new RemoveOldestPoints();

		public int NumberOfPercentiles
		{
			get { return renderPercentiles.NumberOfPercentiles; }
			set
			{
				if (renderPercentiles.NumberOfPercentiles == value)
					return;
				renderPercentiles.NumberOfPercentiles = value;
				renderPercentileLabels.NumberOfPercentiles = value;
				renderPercentiles.Refresh(this);
				renderPercentileLabels.Refresh(this);
			}
		}

		public Color PercentileColor
		{
			get { return renderPercentiles.PercentileColor; }
			set
			{
				if (renderPercentiles.PercentileColor == value)
					return;
				renderPercentiles.PercentileColor = value;
				renderPercentiles.Refresh(this);
			}
		}

		public string PercentilePrefix
		{
			get { return renderPercentileLabels.PercentilePrefix; }
			set
			{
				if (renderPercentileLabels.PercentilePrefix == value)
					return;
				renderPercentileLabels.PercentilePrefix = value;
				renderPercentileLabels.Refresh(this);
			}
		}

		public string PercentileSuffix
		{
			get { return renderPercentileLabels.PercentileSuffix; }
			set
			{
				if (renderPercentileLabels.PercentileSuffix == value)
					return;
				renderPercentileLabels.PercentileSuffix = value;
				renderPercentileLabels.Refresh(this);
			}
		}

		public Color PercentileLabelColor
		{
			get { return renderPercentileLabels.PercentileLabelColor; }
			set
			{
				if (renderPercentileLabels.PercentileLabelColor == value)
					return;
				renderPercentileLabels.PercentileLabelColor = value;
				renderPercentileLabels.Refresh(this);
			}
		}

		public bool ArePercentileLabelsInteger
		{
			get { return renderPercentileLabels.ArePercentileLabelsInteger; }
			set
			{
				if (renderPercentileLabels.ArePercentileLabelsInteger == value)
					return;
				renderPercentileLabels.ArePercentileLabelsInteger = value;
				renderPercentileLabels.Refresh(this);
			}
		}

		internal const float Border = 0.025f;
	}
}