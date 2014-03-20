using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace DeltaEngine.Rendering2D.Graphs.Tests
{
	public class AutogrowViewportTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			new FilledRect(Rectangle.One, Color.Gray) { RenderLayer = int.MinValue };
			graph = new Graph(Center) { AxesIsVisible = true, IsAutogrowing = true };
			line = graph.CreateLine("One", LineColor);
		}

		private Graph graph;
		private static Rectangle Center
		{
			get { return Rectangle.FromCenter(0.5f, 0.5f, 0.4f, 0.3f); }
		}
		private GraphLine line;
		private static Color LineColor
		{
			get { return Color.Blue; }
		}

		[Test]
		public void RenderGraph()
		{
			line.AddPoint(new Vector2D(-1.0f, -1.0f));
			line.AddPoint(new Vector2D(0.0f, 0.5f));
			line.AddPoint(new Vector2D(1.0f, 1.0f));
			line.AddPoint(new Vector2D(1.5f, -2.0f));
		}

		[Test]
		public void RenderRandomGraph()
		{
			graph.Add(line);
			graph.Start<AddValueEverySecond>();
		}

		private class AddValueEverySecond : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				if (Time.CheckEvery(1.0f))
					//ncrunch: no coverage start
					foreach (Entity entity in entities)
						entity.Get<GraphLine>().AddValue(Randomizer.Current.Get());
				//ncrunch: no coverage end
			}
		}

		[Test, Timeout(2000)]
		public void RenderFps()
		{
			graph.Add(line);
			graph.Add(new FontText(Font.Default, "", new Rectangle(0.5f, 0.7f, 0.2f, 0.1f)));
			graph.Start<AddFpsEverySecond>();
		}

		private class AddFpsEverySecond : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				if (!Time.CheckEvery(1.0f))
					return;
				//ncrunch: no coverage start
				foreach (Entity entity in entities)
				{
					entity.Get<GraphLine>().AddValue(0.1f, GlobalTime.Current.Fps);
					entity.Get<FontText>().Text = GlobalTime.Current.Fps + " fps";
				}
				//ncrunch: no coverage end 
			}
		}

		[Test]
		public void RenderNumberOfEntities()
		{
			graph.Viewport = new Rectangle(-1, -1, 1, 1);
			graph.Add(line);
			graph.Add(new FontText(Font.Default, "", new Rectangle(0.5f, 0.7f, 0.2f, 0.1f)));
			graph.Start<AddNumberOfEntitiesEverySecond>();
		}

		private class AddNumberOfEntitiesEverySecond : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				if (!Time.CheckEvery(1.0f))
					return;
				//ncrunch: no coverage start
				foreach (Entity entity in entities)
				{
					entity.Get<GraphLine>().AddValue(EntitiesRunner.Current.NumberOfEntities);
					entity.Get<FontText>().Text = EntitiesRunner.Current.NumberOfEntities + " entities";
				}
				//ncrunch: no coverage end
			}
		}

		[Test]
		public void RenderTwoRandomLinesWithKey()
		{
			GraphLine line2 = graph.CreateLine("Two", Color.Red);
			graph.Add(new List<GraphLine> { line, line2 });
			graph.Start<AddTwoRandomValuesEverySecond>();
		}

		private class AddTwoRandomValuesEverySecond : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				if (Time.CheckEvery(1.0f))
					//ncrunch: no coverage start
					foreach (GraphLine line in entities.SelectMany(entity => entity.Get<List<GraphLine>>()))
						line.AddValue(Randomizer.Current.Get());
				//ncrunch: no coverage end
			}
		}

		[Test, CloseAfterFirstFrame]
		public void ViewportStartsZero()
		{
			Assert.AreEqual(Rectangle.Zero, graph.Viewport);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenOnlyOnePointHasBeenAddedViewportPositionsToThatPoint()
		{
			line.AddPoint(new Vector2D(1.0f, -2.5f));
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(new Rectangle(1.0f, -2.5f, 0.0f, 0.0f), graph.Viewport);
		}

		[Test, CloseAfterFirstFrame]
		public void WhenMultiplePointsHaveBeenAddedViewportPositionsAndSizesToContainThem()
		{
			line.AddPoint(new Vector2D(0.0f, -2.5f));
			line.AddPoint(new Vector2D(2.0f, -1.0f));
			line.AddPoint(new Vector2D(-1.0f, -3.0f));
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(graph.Viewport.IsNearlyEqual(new Rectangle(-1.15f, -3.1f, 3.3f, 2.2f)));
		}

		[Test, CloseAfterFirstFrame]
		public void IfViewportDoesNotNeedToChangeToAccomodateNewPointsItDoesnt()
		{
			line.AddPoint(new Vector2D(0, -2.5f));
			line.AddPoint(new Vector2D(2, -1.0f));
			line.AddPoint(new Vector2D(1, -1.5f));
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(graph.Viewport.IsNearlyEqual(new Rectangle(-0.1f, -2.575f, 2.2f, 1.65f)));
		}
	}
}