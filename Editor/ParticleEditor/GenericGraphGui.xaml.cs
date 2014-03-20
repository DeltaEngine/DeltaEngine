using System.Windows.Controls;
using DeltaEngine.Datatypes;
using SysColors = System.Windows.Media.Colors;

namespace DeltaEngine.Editor.ParticleEditor
{
	/// <summary>
	/// Interaction logic for GenericGraphGui.xaml
	/// </summary>
	public partial class GenericGraphGui
	{
		public GenericGraphGui()
		{
			InitializeComponent();
		}

		/*public void SynchronizeToVectorGraph(TimeRangeGraph<Vector3D> vectorGraph,
			string propertyName)
		{
			SetRows(3);
			GraphContainer.Children.Clear();
			var graphAxisX = new GraphControl(SysColors.Crimson);
			var graphAxisY = new GraphControl(SysColors.SeaGreen);
			var graphAxisZ = new GraphControl(SysColors.RoyalBlue);
			Grid.SetRow(graphAxisX, 0);
			Grid.SetRow(graphAxisY, 1);
			Grid.SetRow(graphAxisZ, 2);
			GraphContainer.Children.Add(graphAxisX);
			GraphContainer.Children.Add(graphAxisY);
			GraphContainer.Children.Add(graphAxisZ);
		}

		public void SynchronizeToSizeGraph(TimeRangeGraph<Size> sizeGraph)
		{
			SetRows(2);
			GraphContainer.Children.Clear();
			var graphAxisWidth = new GraphControl(SysColors.Crimson);
			var graphAxisHeight = new GraphControl(SysColors.SeaGreen);
			GraphContainer.Children.Add(graphAxisWidth);
			GraphContainer.Children.Add(graphAxisHeight);
		}

		private void SetRows(int numberOfRows)
		{
			GraphContainer.RowDefinitions.Clear();
			for (int i = 0; i < numberOfRows; i++)
				GraphContainer.RowDefinitions.Add(new RowDefinition());
		}

		private void RefreshValues() {}*/
	}
}