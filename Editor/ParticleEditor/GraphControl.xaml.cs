/*using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Auxiliary;
using Microsoft.Research.DynamicDataDisplay.Navigation;

namespace DeltaEngine.Editor.ParticleEditor
{
	public partial class GraphControl
	{
		public GraphControl(Color lineColor)
		{
			InitializeComponent();
			Height = 128;
			LineColor = lineColor;
			DataPoints = new PointCollection();
			Loaded += OnLoaded;
		}

		public GraphControl(Color lineColor, PointCollection dataPoints)
		{
			InitializeComponent();
			Height = 128;
			LineColor = lineColor;
			DataPoints = dataPoints;
			Loaded += OnLoaded;
		}

		public PointCollection DataPoints { get; set; }
		public Color LineColor { get; set; }

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			SetupPlotter();
			AddIntervalLimiters();
			CreateLineGraph();
		}

		private void SetupPlotter()
		{
			Plotter.MouseLeftButtonDown += PlotterMouseLeftButtonDown;
			Plotter.MouseLeftButtonUp += PlotterOnMouseLeftButtonUp;
			Plotter.AddChild(new CursorCoordinateGraph { LineStrokeThickness = 0.4 });
			Plotter.Viewport.Visible = new DataRect(0, 0, 1, 1);
			Plotter.Viewport.ClipToBoundsEnlargeFactor = 1.0f;
			SetupRemovePointInContextMenu();
		}

		private void PlotterMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
		{
			remStartClickPosition = args.GetPosition(Plotter);
		}

		private Point remStartClickPosition;

		private void PlotterOnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
		{
			if (args.ClickCount == 1 && args.GetPosition(Plotter) == remStartClickPosition)
				AddNewPoint(ToDataPosition(remStartClickPosition));
		}

		internal void AddNewPoint(Point newPoint)
		{
			if (FindNearestPointOnlyX(newPoint, 0.025f) ||
				DataPoints.Count > 0 && newPoint.X < DataPoints[0].X)
				return;
			for (int num = 1; num < DataPoints.Count; num++)
				if (newPoint.X < DataPoints[num].X)
				{
					DataPoints.Insert(num, newPoint);
					InvokePointAdded(num);
					return;
				}
			DataPoints.Add(newPoint);
			InvokePointAdded(DataPoints.Count);
		}

		private void InvokePointAdded(int insertedIndex)
		{
			if (PointAdded != null)
				PointAdded(insertedIndex);
		}

		public event Action<int> PointAdded;

		private bool FindNearestPointOnlyX(Point dataPosition, float distance)
		{
			float smallestDistance = distance;
			foreach (var point in DataPoints)
			{
				var xDiff = (float)Math.Abs(point.X - dataPosition.X);
				if (xDiff >= smallestDistance)
					continue;
				smallestDistance = xDiff;
			}
			return smallestDistance < distance;
		}

		private bool FindNearestPoint(Point dataPosition, float distance)
		{
			float smallestSquareDistance = distance * distance;
			foreach (var point in DataPoints)
			{
				var xDiff = (float)(point.X - dataPosition.X);
				var yDiff = (float)(point.Y - dataPosition.Y);
				var squareDistance = xDiff * xDiff + yDiff * yDiff;
				if (squareDistance >= smallestSquareDistance)
					continue;
				smallestSquareDistance = squareDistance;
				nearestPoint = point;
			}
			return smallestSquareDistance < distance * distance;
		}

		private Point nearestPoint;

		private Point ToDataPosition(Point plotterScreenPosition)
		{
			plotterScreenPosition.X -= Plotter.MainVerticalAxis.ActualWidth;
			return Plotter.Transform.ScreenToData(plotterScreenPosition);
		}

		private void SetupRemovePointInContextMenu()
		{
			var removePoint = new MenuItem
			{
				Header = "Remove Point",
				ToolTip = "Remove nearest point",
				Icon = new Image { Source = DefaultContextMenu.LoadIcon("RemoveIcon") },
				Command = new RelayCommand(RemovePoint, IsContextMenuPointNearby),
				CommandTarget = Plotter
			};
			var keyBinding = new KeyBinding(new RelayCommand(RemovePoint, IsMousePointNearby),
				Key.Delete, ModifierKeys.None);
			InputBindings.Add(keyBinding);
			removePoint.InputGestureText = "Del";
			Plotter.DefaultContextMenu.StaticMenuItems.Insert(0, removePoint);
		}

		private void RemovePoint()
		{
			if (nearestPoint != new Point())
				DataPoints.Remove(nearestPoint);
			nearestPoint = new Point();
		}

		//public event Action<int> PointRemoved;

		private bool IsContextMenuPointNearby()
		{
			return FindNearestPointFromScreenPoint(Plotter.DefaultContextMenu.MousePositionOnClick);
		}

		private bool IsMousePointNearby()
		{
			return FindNearestPointFromScreenPoint(Mouse.GetPosition(Plotter));
		}

		private bool FindNearestPointFromScreenPoint(Point clickPosition)
		{
			return FindNearestPoint(ToDataPosition(clickPosition), 0.15f);
		}

		private void AddIntervalLimiters()
		{
			Plotter.AddChild(new VerticalLine(0.0f)
			{
				StrokeThickness = 0.75,
				Stroke = new SolidColorBrush(Colors.LightSkyBlue)
			});
			Plotter.AddChild(new VerticalLine(1.0f)
			{
				StrokeThickness = 0.75,
				Stroke = new SolidColorBrush(Colors.LightSkyBlue)
			});
		}

		private void CreateLineGraph()
		{
			editor = new PolylineEditor();
			editor.Polyline = new ViewportPolyline();
			editor.Polyline.Stroke = new SolidColorBrush(LineColor);
			editor.Polyline.StrokeThickness = 1;
			editor.Polyline.Points = DataPoints;
			editor.AddToPlotter(Plotter);
		}

		private PolylineEditor editor;

		public void AddPointUnsafe(double x, double y)
		{
			DataPoints.Add(new Point(x, y));
		}
	}
}*/