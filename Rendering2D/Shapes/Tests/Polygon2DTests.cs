using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	public class Polygon2DTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void NewPolygon()
		{
			var polygon = new Polygon2D(Rectangle.One, Color.White);
			polygon.Points.AddRange(new[] { Vector2D.Zero, Vector2D.One, Vector2D.UnitY });
			Assert.AreEqual(Rectangle.One, polygon.DrawArea);
			Assert.AreEqual(Color.White, polygon.Color);
		}

		[Test, CloseAfterFirstFrame]
		public void ChangePolygonAfterFirstFrame()
		{
			var polygon = new Polygon2D(Rectangle.One, Color.White);
			polygon.Points.AddRange(new[] { Vector2D.Zero, Vector2D.One, Vector2D.UnitY });
			AdvanceTimeAndUpdateEntities();
			polygon.Points.Add(Vector2D.Zero);
			AdvanceTimeAndUpdateEntities();
		}

		[Test, CloseAfterFirstFrame]
		public void ChangeOutlineColor()
		{
			var polygon = new Polygon2D(Rectangle.One, Color.Red);
			polygon.Add(new OutlineColor(Color.Blue));
			Assert.AreEqual(Color.Blue, polygon.Get<OutlineColor>().Value);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void RenderEllipseOutline()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Blue);
			ellipse.Add(new OutlineColor(Color.Red));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
		}

		[Test, CloseAfterFirstFrame]
		public void RenderingPolygonWithNoPointsDoesNotError()
		{
			var ellipse = new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Blue);
			var points = ellipse.Get<List<Vector2D>>();
			points.Clear();
			ellipse.Remove<Ellipse.UpdatePointsIfRadiusChanges>();
			ellipse.Add(new OutlineColor(Color.Red));
			ellipse.OnDraw<DrawPolygon2DOutlines>();
		}

		[Test]
		public void NoErrorIfThereAreNoVertices()
		{
			var entity = new Entity2D(Rectangle.One);
			entity.Add(new OutlineColor(Color.Red));
			Assert.DoesNotThrow(entity.OnDraw<DrawPolygon2DOutlines>);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawComplexPolygon()
		{
			CreatePolygon(Vector2D.Half, 0.25f, Color.White);
		}

		private static void CreatePolygon(Vector2D position, float radius, Color color)
		{
			var polygon = new Polygon2D(Rectangle.FromCenter(position, new Size(radius)), color);
			var points = new List<Vector2D> { polygon.Center };
			for (int num = 0; num <= 500; num++)
				points.Add(polygon.Center +
					new Vector2D(radius * MathExtensions.Sin(num * 360.0f / 500.0f),
						radius * MathExtensions.Cos(num * 360.0f / 500.0f)));
			polygon.Points.AddRange(points);
		}

		[Test, ApproveFirstFrameScreenshot]
		public void DrawTwoComplexPolygons()
		{
			CreatePolygon(new Vector2D(0.25f, 0.5f), 0.2f, Color.Green);
			CreatePolygon(new Vector2D(0.75f, 0.5f), 0.2f, Color.Red);
		}

		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void DrawFourHundredComplexPolygons()
		{
			// 20*20=400 * 500 polygons each is 200000 polygons (needs to be rendered in batches)
			for (int y = 0; y < 20; y++)
				for (int x = 0; x < 20; x++)
					CreatePolygon(new Vector2D(0.025f, 0.025f) + new Vector2D(x / 20.0f, y / 20.0f), 0.025f,
						Color.GetRandomColor());
		}
	}
}