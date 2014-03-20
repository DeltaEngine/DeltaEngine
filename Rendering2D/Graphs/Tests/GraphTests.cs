using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Graphs.Tests
{
	public class GraphTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			graph = new Graph(center) { Viewport = largeViewport, AxesIsVisible = true };
			center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.2f);
			largeViewport = new Rectangle(-2.0f, -2.0f, 4.0f, 4.0f);
		}

		private Graph graph;
		private Rectangle center;
		private Rectangle largeViewport;

		[Test]
		public void RenderGraphWithFourLines()
		{
			CreateGraphWithFourLines();
			graph.AxesIsVisible = false;
		}

		private void CreateGraphWithFourLines()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Vector2D(-1.0f, -1.0f));
			line.AddPoint(new Vector2D(0.1f, 0.5f));
			line.AddPoint(new Vector2D(0.5f, 0.2f));
			line.AddPoint(new Vector2D(0.9f, 1.0f));
			line.AddPoint(new Vector2D(1.5f, -2.0f));
		}

		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void RenderResizedGraph()
		{
			CreateGraphWithFourLines();
			graph.DrawArea = Rectangle.HalfCentered;
		}

		[Test]
		public void RenderGraphWithAxes()
		{
			CreateGraphWithFourLines();
		}

		[Test]
		public void RenderOffCenterGraphWithAxesAndClipping()
		{
			CreateGraphWithFourLines();
			graph.Origin = Vector2D.Half;
			graph.Viewport = new Rectangle(0.2f, 0.3f, 1.0f, 1.0f);
		}

		[Test]
		public void RenderFpsWithFivePercentiles()
		{
			graph.Viewport = new Rectangle(0.0f, 0.0f, 10.0f, 60.0f);
			graph.NumberOfPercentiles = 5;
			graph.PercentileSuffix = "%";
			GraphLine line = graph.CreateLine("", LineColor);
			var fps = new FontText(Font.Default, "",
				new Rectangle(0.5f, 0.7f, 1.0f, 0.1f));
			graph.Add(line);
			graph.Add(fps);
			graph.Start<AddValueEveryFrame>();
		}

		private class AddValueEveryFrame : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Entity entity in entities)
				{
					entity.Get<GraphLine>().AddValue(0.1f, GlobalTime.Current.Fps);
					entity.Get<FontText>().Text = GlobalTime.Current.Fps + " fps";
				}
			}
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeAxesVisibility()
		{
			Assert.IsTrue(graph.AxesIsVisible);
			graph.AxesIsVisible = false;
			Assert.IsFalse(graph.AxesIsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentilesVisibility()
		{
			Assert.IsFalse(graph.PercentilesIsVisible);
			graph.PercentilesIsVisible = true;
			graph.NumberOfPercentiles = 2;
			Assert.IsTrue(graph.PercentilesIsVisible);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentileLabelsVisibility()
		{
			Assert.IsFalse(graph.PercentileLabelsIsVisible);
			graph.PercentileLabelsIsVisible = true;
			graph.ArePercentileLabelsInteger = true;
			Assert.IsTrue(graph.PercentileLabelsIsVisible);
		}

		[Test, CloseAfterFirstFrame, Timeout(2000)]
		public void ChangeKeyVisibility()
		{
			graph.CreateLine("TestLine", Color.Red);
			graph.CreateLine("TestLine2", Color.Red);
			Assert.IsFalse(graph.KeyVisibility);
			graph.KeyVisibility = true;
			Assert.IsTrue(graph.KeyVisibility);
			graph.RefreshKey();

		}
		
		[Test, CloseAfterFirstFrame]
		public void ChangeOrigin()
		{
			Assert.AreEqual(Vector2D.Zero, graph.Origin);
			graph.Origin = Origin;
			graph.Origin = Origin;
			Assert.AreEqual(Origin, graph.Origin);
		}

		private static readonly Vector2D Origin = new Vector2D(1.5f, -1.5f);

		[Test, CloseAfterFirstFrame]
		public void ChangeAxisColor()
		{
			Assert.AreEqual(Color.White, graph.AxisColor);
			graph.AxisColor = Color.Blue;
			graph.AxisColor = Color.Blue;
			Assert.AreEqual(Color.Blue, graph.AxisColor);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeViewport()
		{
			graph.Viewport = Rectangle.One;
			Assert.AreEqual(Rectangle.One, graph.Viewport);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeBackgroundColor()
		{
			Assert.AreEqual(Graph.HalfBlack, graph.Color);
			graph.Color = Color.White;
			Assert.AreEqual(Color.White, graph.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeNumberOfPercentiles()
		{
			Assert.AreEqual(0, graph.NumberOfPercentiles);
			graph.NumberOfPercentiles = 2;
			graph.NumberOfPercentiles = 2;
			Assert.AreEqual(2, graph.NumberOfPercentiles);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeMaximumNumberOfPoints()
		{
			Assert.AreEqual(0, graph.MaximumNumberOfPoints);
			graph.MaximumNumberOfPoints = 2;
			graph.MaximumNumberOfPoints = 2;
			Assert.AreEqual(2, graph.MaximumNumberOfPoints);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentileColor()
		{
			Assert.AreEqual(Color.Gray, graph.PercentileColor);
			graph.PercentileColor = Color.White;
			graph.PercentileColor = Color.White;
			Assert.AreEqual(Color.White, graph.PercentileColor);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentileSuffix()
		{
			Assert.AreEqual("", graph.PercentileSuffix);
			graph.PercentileSuffix = "%";
			graph.PercentileSuffix = "%";
			Assert.AreEqual("%", graph.PercentileSuffix);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentilePrefix()
		{
			Assert.AreEqual("", graph.PercentilePrefix);
			graph.PercentilePrefix = "$";
			graph.PercentilePrefix = "$";
			Assert.AreEqual("$", graph.PercentilePrefix);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePercentileLabelColor()
		{
			Assert.AreEqual(Color.White, graph.PercentileLabelColor);
			graph.PercentileLabelColor = Color.Gray;
			graph.PercentileLabelColor = Color.Gray;
			Assert.AreEqual(Color.Gray, graph.PercentileLabelColor);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeArePercentileLabelsInteger()
		{
			Assert.IsFalse(graph.ArePercentileLabelsInteger);
			graph.ArePercentileLabelsInteger = true;
			graph.ArePercentileLabelsInteger = true;
			Assert.IsTrue(graph.ArePercentileLabelsInteger);
		}

		[Test]
		public void HiddenGraphDisplaysNothing()
		{
			graph.NumberOfPercentiles = 5;
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Vector2D(-1.0f, -1.0f));
			line.AddPoint(new Vector2D(0.1f, 0.5f));
			graph.IsVisible = false;
		}

		[Test, CloseAfterFirstFrame]
		public void RemovingGraphLineRemovesItsLines()
		{
			GraphLine line = graph.CreateLine("", LineColor);
			line.AddPoint(new Vector2D(-1.0f, -1.0f));
			line.AddPoint(new Vector2D(0.1f, 0.5f));
			Assert.AreEqual(1, graph.Lines.Count);
			Assert.AreEqual(1, line.lines.Count);
			graph.RemoveLine(line);
			Assert.AreEqual(0, graph.Lines.Count);
			Assert.AreEqual(0, line.lines.Count);
		}

		[Test]
		public void GraphsArePauseable()
		{
			Assert.IsTrue(graph.IsPauseable);
		}

		[Test, CloseAfterFirstFrame]
		public void RenderGraphIncludingKey()
		{
			graph.CreateLine("key", LineColor);
			graph.KeyVisibility = true;
			graph.RefreshKey();
			graph.KeyVisibility = true;
			graph.PercentileLabelsIsVisible = false;
			graph.PercentilesIsVisible = false;
			graph.Origin = graph.Origin;
			AdvanceTimeAndUpdateEntities();
		}

		[Test,CloseAfterFirstFrame]
		public void GraphUpdatePauseable()
		{
			Assert.IsTrue(graph.IsPauseable);
		}
	}
}