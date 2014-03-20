/*using System.Collections.Generic;
using System.Windows;
using DeltaEngine.Datatypes;
using NUnit.Framework;
using Size = DeltaEngine.Datatypes.Size;

namespace DeltaEngine.Editor.ParticleEditor.Tests
{
	[RequiresSTA]
	public class GraphGuiTests
	{
		//ncrunch: no coverage start
		[Test, Category("Slow"), Category("WPF")]
		public void ShowVectorGraphGuiInWindow()
		{
			var vectors = new List<Vector3D>(new [] {Vector3D.Zero, Vector3D.UnitZ, Vector3D.Up, Vector3D.UnitX, Vector3D.UnitZ, Vector3D.Backward});
			var graphGui = new VectorGraphGui();
			var range = new TimeRangeGraph<Vector3D>(vectors);
			graphGui.SynchronizeToTimeRange(range, "range");
			var window = new Window
			{
				Title = "WPF Test - UserControl VectorGraphGui",
				Content = graphGui
			};
			window.ShowDialog();
		}

		[Test, Category("Slow"), Category("WPF")]
		public void ShowSizeGraphGuiInWindow()
		{
			var sizes =
				new List<Size>(new[] { new Size(0.2f, 0.8f), new Size(0.4f, 0.4f), new Size(0.2f, 0.8f) });
			var graphGui = new SizeGraphGui();
			var range = new TimeRangeGraph<Size>(sizes);
			graphGui.SynchronizeToTimeRange(range, "range");
			var window = new Window
			{
				Title = "WPF Test - UserControl SizeGraphGui",
				Content = graphGui
			};
			window.ShowDialog();
		}
	}
}*/