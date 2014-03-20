/*using System.Windows;
using System.Windows.Media;
using DeltaEngine.Datatypes;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ParticleEditor
{
	public class SizeGraphGui : GenericGraphGui
	{
		public TimeRangeGraph<Size> CurrentRange { get; private set; }
		private GraphControl graphAxisWidth;
		private GraphControl graphAxisHeight;

		public void SynchronizeToTimeRange(TimeRangeGraph<Size> sizeGraph, string propertyName)
		{
			this.propertyName = propertyName;
			CurrentRange = sizeGraph;
			SetRows(2);
			Children.Clear();
			graphAxisWidth = new GraphControl(Colors.Crimson);
			graphAxisHeight = new GraphControl(Colors.SeaGreen);
			SetRow(graphAxisWidth, 0);
			SetRow(graphAxisHeight, 1);
			Children.Add(graphAxisWidth);
			Children.Add(graphAxisHeight);
			graphAxisWidth.PointAdded += AddPointToOtherFromWidth;
			graphAxisHeight.PointAdded += AddPointToOtherFromHeight;
			RefreshValuesFromRange();
		}

		protected override void RefreshValuesFromRange()
		{
			ClearGraphs();
			for (int i = 0; i < CurrentRange.Values.Length; i++)
			{
				graphAxisWidth.AddPointUnsafe(CurrentRange.Percentages[i], CurrentRange.Values[i].Width);
				graphAxisHeight.AddPointUnsafe(CurrentRange.Percentages[i], CurrentRange.Values[i].Height);
			}
		}

		protected override void ClearGraphs()
		{
			graphAxisWidth.DataPoints.Clear();
			graphAxisHeight.DataPoints.Clear();
		}

		protected void AddPointToOtherFromWidth(int index)
		{
			var interpolation = graphAxisWidth.DataPoints[index].X;
			var addedSize = new Size((float)graphAxisWidth.DataPoints[index].Y,
				CurrentRange.GetInterpolatedValue((float)interpolation).Height);
			graphAxisHeight.AddNewPoint(new Point(interpolation, addedSize.Height));
			CurrentRange.AddValueAt((float)interpolation, addedSize);
		}

		protected void AddPointToOtherFromHeight(int index)
		{
			var interpolation = graphAxisHeight.DataPoints[index].X;
			var addedSize = new Size(CurrentRange.GetInterpolatedValue((float)interpolation).Width,
				(float)graphAxisHeight.DataPoints[index].Y);
			graphAxisWidth.AddNewPoint(new Point(interpolation,
				CurrentRange.GetInterpolatedValue((float)interpolation).Width));
			CurrentRange.AddValueAt((float)interpolation, addedSize);
		}
	}
}*/