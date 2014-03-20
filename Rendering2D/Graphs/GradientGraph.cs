using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Rendering2D.Graphs
{
	public class GradientGraph : Entity2D
	{
		public GradientGraph(Rectangle drawArea)
			: this(drawArea, new RangeGraph<Color>(Color.White)) {}

		public GradientGraph(Rectangle drawArea, RangeGraph<Color> colorRanges)
			: base(drawArea)
		{
			colorIntervals = colorRanges;
			UpdateDrawingToRanges();
		}

		private GradientFilledRect[] drawPartitions;
		private readonly RangeGraph<Color> colorIntervals;

		public void AddValueAfter(int index, Color value)
		{
			colorIntervals.AddValueAfter(index,value);
			UpdateDrawingToRanges();
		}

		public void AddValueBefore(int index, Color value)
		{
			colorIntervals.AddValueBefore(index, value);
			UpdateDrawingToRanges();
		}

		public void SetValue(int index, Color value)
		{
			colorIntervals.SetValue(index, value);
			UpdateDrawingToRanges();
		}

		public Color[] Values { get { return colorIntervals.Values; } }

		private void UpdateDrawingToRanges()
		{
			if (drawPartitions != null)
				foreach (var gradientFilledRect in drawPartitions)
					gradientFilledRect.IsActive = false;
			var numberOfRects = colorIntervals.Values.Length - 1;
			var partsWidth = DrawArea.Width / numberOfRects;
			drawPartitions = new GradientFilledRect[numberOfRects];
			CreateGradientRectangles(numberOfRects, partsWidth);
			CreateLineMarkers(partsWidth);
		}

		private void CreateGradientRectangles(int numberOfRects, float partsWidth)
		{
			for (int i = 0; i < numberOfRects; i++)
				drawPartitions[i] = new GradientFilledRect(CreatePartDrawAreaForIndex(i, partsWidth),
					colorIntervals.Values[i], colorIntervals.Values[i + 1]) { RenderLayer = RenderLayer };
		}

		private Rectangle CreatePartDrawAreaForIndex(int index, float partsWidth)
		{
			return new Rectangle(DrawArea.Left + index * partsWidth, DrawArea.Top, partsWidth,
				DrawArea.Height);
		}

		private void CreateLineMarkers(float partsWidth)
		{
			if (lineMarkers != null)
				foreach (var lineMarker in lineMarkers)
					lineMarker.IsActive = false;

			var numberOfLines = colorIntervals.Values.Length;
			lineMarkers = new FilledRect[numberOfLines];
			for (int i = 0; i < numberOfLines; i++)
				lineMarkers[i] = new FilledRect(CreateMarkerDrawAreaForIndex(i, partsWidth), Color.Gray)
				{
					RenderLayer = RenderLayer + 1
				};
		}

		private Rectangle CreateMarkerDrawAreaForIndex(int index, float partsWidth)
		{
			var halfWidth = DrawArea.Width / 100;
			return new Rectangle(DrawArea.Left + index* partsWidth - halfWidth, DrawArea.Top,halfWidth*2,DrawArea.Height);
		}

		private FilledRect[] lineMarkers;
	}
}