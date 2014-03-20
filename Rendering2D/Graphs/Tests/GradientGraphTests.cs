using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Graphs.Tests
{
	internal class GradientGraphTests : TestWithMocksOrVisually
	{
		[Test]
		public void DrawGradientGraph()
		{
			var colors = new List<Color>(new[]
			{
				Color.DarkGreen, Color.Red, Color.Orange, Color.Green, Color.Black, Color.Gold,
				Color.PaleGreen
			});
			var colorRanges = new RangeGraph<Color>(colors);
			new GradientGraph(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f), colorRanges);
		}
		
		[Test]
		public void DrawGraphByAddingNewValues()
		{
			var graph = new GradientGraph(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f));
			graph.AddValueBefore(0, Color.Cyan);
			graph.AddValueAfter(0, Color.Purple);
			graph.SetValue(2, Color.Gold);
			Assert.AreEqual(3, graph.Values.Length);
			Assert.AreEqual(Color.Cyan, graph.Values[0]);
			Assert.AreEqual(Color.Purple, graph.Values[1]);
			Assert.AreEqual(Color.Gold, graph.Values[2]);
		}

		[Test, CloseAfterFirstFrame]
		public void AddColorsToGradient()
		{
			var colors = new List<Color>(new[] { Color.DarkGreen, Color.Red, Color.Orange, Color.Green});
			var initiallySetColors = new List<Color>(new[] { colors[0], colors[2] });
			var colorRanges = new RangeGraph<Color>(initiallySetColors);
			var gradient = new GradientGraph(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f), colorRanges);
			gradient.AddValueAfter(1,colors[3]);
			gradient.AddValueBefore(1,colors[1]);
			Assert.AreEqual(colors.ToArray(), gradient.Values);
		}

		[Test, CloseAfterFirstFrame]
		public void SetColors()
		{
			var gradient = new GradientGraph(new Rectangle(0.1f, 0.4f, 0.8f, 0.2f));
			gradient.SetValue(0, Color.Red);
			gradient.SetValue(1, Color.Orange);
			Assert.AreEqual(new[]{Color.Red, Color.Orange}, gradient.Values);
		}
	}
}