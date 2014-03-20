using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	internal class RangeGraphTests
	{
		[Test]
		public void SingleConstructor()
		{
			var rangeGraph = new RangeGraph<Vector2D>(Vector2D.UnitX);
			Assert.AreEqual(Vector2D.UnitX, rangeGraph.Start);
			Assert.AreEqual(Vector2D.UnitX, rangeGraph.End);
		}

		[Test]
		public void MinMaxConstructor()
		{
			var rangeGraph = new RangeGraph<Vector2D>(Vector2D.UnitX, Vector2D.UnitY);
			Assert.AreEqual(Vector2D.UnitX, rangeGraph.Start);
			Assert.AreEqual(Vector2D.UnitY, rangeGraph.End);
		}

		[Test]
		public void RangeConstructor()
		{
			var colors = new List<Color> { Color.Red, Color.Orange, Color.Yellow };
			var rangeGraph = new RangeGraph<Color>(colors);
			Assert.AreEqual(colors[0], rangeGraph.Start);
			Assert.AreEqual(colors[2], rangeGraph.End);
		}

		[Test]
		public void SetStart()
		{
			var rangeGraph = new RangeGraph<Vector2D>(Vector2D.UnitX, Vector2D.UnitY);
			rangeGraph.Start = Vector2D.One;
			Assert.AreEqual(Vector2D.One, rangeGraph.Start);
		}

		[Test]
		public void SetEnd()
		{
			var rangeGraph = new RangeGraph<Vector2D>(Vector2D.UnitX, Vector2D.UnitY);
			rangeGraph.End = Vector2D.One;
			Assert.AreEqual(Vector2D.One, rangeGraph.End);
		}

		[Test]
		public void GraphOfRanges()
		{
			var ranges = new[]
			{
				new Range<Color>(Color.Red, Color.Orange), new Range<Color>(Color.Orange, Color.Gold),
				new Range<Color>(Color.Gold, Color.Yellow)
			};
			var rangesGraph = new RangeGraph<Range<Color>>(ranges[0], ranges[2]);
			rangesGraph.AddValueAfter(0, ranges[1]);
			Assert.AreEqual(ranges, rangesGraph.Values);
			Assert.IsTrue(rangesGraph.ToString().StartsWith("("));
		}

		[Test]
		public void GetTrivialInterpolation()
		{
			var points = new List<Vector2D>(new[] { Vector2D.One });
			var graph = new RangeGraph<Vector2D>(points);
			var interpolatedPointMiddle = graph.GetInterpolatedValue(0.3f);
			var expectedPointMiddle = Vector2D.One;
			Assert.AreEqual(expectedPointMiddle, interpolatedPointMiddle);
		}

		[Test]
		public void GetInterpolation()
		{
			var points =
				new List<Vector2D>(new[]
				{ Vector2D.Zero, Vector2D.UnitX, Vector2D.UnitY, Vector2D.UnitY, Vector2D.One });
			var graph = new RangeGraph<Vector2D>(points);
			var interpolatedPointMiddle = graph.GetInterpolatedValue(0.3f);
			var expectedPointMiddle = points[1].Lerp(points[2], 4 * 0.3f - 1);
			var interpolatedPointEnd = graph.GetInterpolatedValue(1.0f);
			var expectedPointEnd = points[4];
			var interpolatedPointStart = graph.GetInterpolatedValue(0.0f);
			var expectedPointStart = points[0];
			Assert.AreEqual(expectedPointMiddle, interpolatedPointMiddle);
			Assert.AreEqual(expectedPointStart, interpolatedPointStart);
			Assert.AreEqual(expectedPointEnd, interpolatedPointEnd);
		}

		[Test]
		public void InsertValueBeforeIndexRightOfMax()
		{
			var colors = new[] { Color.Red, Color.Orange, Color.Yellow };
			var colorGraph = new RangeGraph<Color>(colors[0], colors[1]);
			colorGraph.AddValueBefore(5, colors[2]);
			Assert.AreEqual(colors, colorGraph.Values);
		}

		[Test]
		public void InsertValueAfterIndexLeftOfMin()
		{
			var colors = new[] { Color.LightBlue, Color.Cyan, Color.Green };
			var colorGraph = new RangeGraph<Color>(colors[1], colors[2]);
			colorGraph.AddValueAfter(-7, colors[0]);
			Assert.AreEqual(colors, colorGraph.Values);
		}

		[Test]
		public void SetValuesToSeveralGraphTypes()
		{
			var vectorGraph = new RangeGraph<Vector3D>(Vector3D.Zero, Vector3D.UnitZ);
			var colorGraph = new RangeGraph<Color>(Color.Red, Color.Orange);
			colorGraph.SetValue(1, Color.Gold);
			Assert.AreEqual(Vector3D.Zero, vectorGraph.Start);
			Assert.AreEqual(Vector3D.UnitZ, vectorGraph.Values[1]);
			Assert.AreEqual(Color.Gold, colorGraph.End);
		}

		[Test]
		public void SettingValueRightOfAllIndicesExpands()
		{
			var rectangles = new[] { Rectangle.One, Rectangle.Zero, Rectangle.One };
			var rectGraph = new RangeGraph<Rectangle>(rectangles[0], rectangles[1]);
			rectGraph.SetValue(3, rectangles[2]);
			Assert.AreEqual(new[] { Rectangle.One, Rectangle.Zero, Rectangle.One }, rectGraph.Values);
		}

		[Test]
		public void SettingValueLeftOfAllIndicesExpands()
		{
			var rectangles = new[] { Rectangle.One, Rectangle.Zero, Rectangle.One };
			var rectGraph = new RangeGraph<Rectangle>(rectangles[1], rectangles[2]);
			rectGraph.SetValue(-1, rectangles[0]);
			Assert.AreEqual(rectangles, rectGraph.Values);
		}

		[Test]
		public void InsertValuesInTheMiddle()
		{
			var points = new[]
			{ Vector2D.Zero, Vector2D.UnitX, Vector2D.UnitY, Vector2D.UnitY, Vector2D.One };
			var pointGraph = new RangeGraph<Vector2D>(points[0], points[4]);
			pointGraph.AddValueAfter(0, points[2]);
			pointGraph.AddValueAfter(1, points[3]);
			pointGraph.AddValueBefore(1, points[1]);
			Assert.AreEqual(points, pointGraph.Values);
		}

		[Test]
		public void ConvertToStringAndBackVector2D()
		{
			var rangeGraph = new RangeGraph<Vector2D>(Vector2D.UnitX, Vector2D.UnitY);
			var stringGraph = rangeGraph.ToString();
			var retrievedRangeGraph = new RangeGraph<Vector2D>(stringGraph);
			Assert.AreEqual("({1, 0}, {0, 1})", stringGraph);
			Assert.AreEqual(rangeGraph.Values, retrievedRangeGraph.Values);
		}

		[Test]
		public void ConvertToStringAndBackColors()
		{
			var rangeGraph = new RangeGraph<Color>(new[] { Color.Red, Color.Black, Color.Green });
			var stringGraph = rangeGraph.ToString();
			var retrievedGraph = new RangeGraph<Color>(stringGraph);
			Assert.AreEqual(
				"({R=255, G=0, B=0, A=255}, {R=0, G=0, B=0, A=255}, {R=0, G=255, B=0, A=255})", stringGraph);
			Assert.AreEqual(rangeGraph.Values, retrievedGraph.Values);
		}

		[Test]
		public void ConvertingFromInvalidStringThrows()
		{
			var rangeGraph = new RangeGraph<Color>(new[] { Color.Red, Color.Black, Color.Green });
			var stringGraph = rangeGraph.ToString();
			Assert.Throws<Range<Vector3D>.TypeInStringNotEqualToInitializedType>(
				() => { new RangeGraph<Vector3D>(stringGraph); });
			Assert.Throws<Range<Vector3D>.InvalidStringFormat>(
				() => { new RangeGraph<Vector3D>("rect, 123, que?"); });
		}
	}
}