/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Graph GUI for modification of a TimeRangeGraph containing Vector3D.
	/// It contains three linegraphs that are synchronized to the range of vectors by events
	/// </summary>
	public class VectorGraphGui : GenericGraphGui
	{
		public void SynchronizeToTimeRange(TimeRangeGraph<Vector3D> vectorRange, string propertyName)
		{
			this.propertyName = propertyName;
			CurrentRange = vectorRange;
			SetRows(3);
			Children.Clear();
			graphAxisX = new GraphControl(Colors.Crimson);
			graphAxisY = new GraphControl(Colors.SeaGreen);
			graphAxisZ = new GraphControl(Colors.RoyalBlue);
			Grid.SetRow(graphAxisX, 0);
			Grid.SetRow(graphAxisY, 1);
			Grid.SetRow(graphAxisZ, 2);
			Children.Add(graphAxisX);
			Children.Add(graphAxisY);
			Children.Add(graphAxisZ);
			graphAxisX.PointAdded += AddPointToOtherFromX;
			graphAxisY.PointAdded += AddPointToOtherFromY;
			graphAxisZ.PointAdded += AddPointToOtherFromZ;
			RefreshValuesFromRange();
		}

		public TimeRangeGraph<Vector3D> CurrentRange { get; private set; }
		private GraphControl graphAxisX;
		private GraphControl graphAxisY;
		private GraphControl graphAxisZ;

		protected override void RefreshValuesFromRange()
		{
			ClearGraphs();
			for (int i = 0; i < CurrentRange.Values.Length; i++)
			{
				graphAxisX.AddPointUnsafe(CurrentRange.Percentages[i], CurrentRange.Values[i].X);
				graphAxisY.AddPointUnsafe(CurrentRange.Percentages[i], CurrentRange.Values[i].Y);
				graphAxisZ.AddPointUnsafe(CurrentRange.Percentages[i], CurrentRange.Values[i].Z);
			}
		}

		protected override void ClearGraphs()
		{
			graphAxisX.DataPoints.Clear();
			graphAxisY.DataPoints.Clear();
			graphAxisZ.DataPoints.Clear();
		}

		private void AddPointToOtherFromX(int index)
		{
			var interpolation = graphAxisX.DataPoints[index].X;
			var addedVector = CurrentRange.GetInterpolatedValue((float)interpolation);
			addedVector.X = (float)graphAxisX.DataPoints[index].Y;
			graphAxisY.AddNewPoint(new Point(interpolation, addedVector.Y));
			graphAxisZ.AddNewPoint(new Point(interpolation, addedVector.Z));
			CurrentRange.AddValueAt((float)interpolation, addedVector);
		}

		private void AddPointToOtherFromY(int index)
		{
			var interpolation = graphAxisY.DataPoints[index].X;
			var addedVector = CurrentRange.GetInterpolatedValue((float)interpolation);
			addedVector.Y = (float)graphAxisY.DataPoints[index].Y;
			graphAxisX.AddNewPoint(new Point(interpolation, addedVector.X));
			graphAxisZ.AddNewPoint(new Point(interpolation, addedVector.Z));
			CurrentRange.AddValueAt((float)interpolation, addedVector);
		}

		private void AddPointToOtherFromZ(int index)
		{
			var interpolation = graphAxisZ.DataPoints[index].X;
			var addedVector = CurrentRange.GetInterpolatedValue((float)interpolation);
			addedVector.Z = (float)graphAxisZ.DataPoints[index].Y;
			graphAxisX.AddNewPoint(new Point(interpolation, addedVector.X));
			graphAxisY.AddNewPoint(new Point(interpolation, addedVector.Y));
			CurrentRange.AddValueAt((float)interpolation, addedVector);
		}
	}
}*/