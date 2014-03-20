using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering2D.Graphs.Tests
{
	[Category("Slow")]
	public class RemoveOldestPointsTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[SetUp]
		public void SetUp()
		{
			graph = new Graph(Center)
			{
				Viewport = Rectangle.One,
				MaximumNumberOfPoints = 10,
				NumberOfPercentiles = 2,
				AxesIsVisible = true,
				PercentilesIsVisible = true,
				PercentileLabelsIsVisible = true
			};
			line = graph.CreateLine("", LineColor);
			graph.Add(line);
		}

		private Graph graph;
		private static readonly Rectangle Center = Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f);
		private GraphLine line;
		private static readonly Color LineColor = Color.Blue;

		[Test]
		public void RenderTenPointRandomScrollingGraphOfDollars()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			graph.PercentilePrefix = "$";
			Assert.AreEqual(10, graph.MaximumNumberOfPoints);
			graph.Start<AddValueEverySecond>();
		}

		private class AddValueEverySecond : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				if (Time.CheckEvery(1.0f))
					foreach (Entity entity in entities)
						entity.Get<GraphLine>().AddValue(0.1f, Randomizer.Current.Get());
			}
		}

		[Test, CloseAfterFirstFrame]
		public void AddingPointDoesNotRemoveFirstPointIfUnderTheLimit()
		{
			graph.MaximumNumberOfPoints = 3;
			line.AddPoint(new Vector2D(1, 0));
			line.AddPoint(new Vector2D(2, 0));
			Assert.AreEqual(2, line.points.Count);
			line.AddPoint(new Vector2D(3, 0));
			Assert.AreEqual(3, line.points.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void AddingPointRemovesFirstPointIfOverTheLimit()
		{
			graph.MaximumNumberOfPoints = 3;
			line.AddPoint(new Vector2D(1, 0));
			line.AddPoint(new Vector2D(2, 0));
			line.AddPoint(new Vector2D(3, 0));
			Assert.AreEqual(3, line.points.Count);
			line.AddPoint(new Vector2D(4, 0));
			Assert.AreEqual(3, line.points.Count);
		}
	}
}