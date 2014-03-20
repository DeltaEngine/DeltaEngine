using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class RangeTests
	{
		[Test]
		public void CreateEmptyRange()
		{
			var range = new Range<Vector2D>();
			Assert.AreEqual(Vector2D.Zero, range.Start);
			Assert.AreEqual(Vector2D.Zero, range.End);
		}

		[Test]
		public void CreateRange()
		{
			var range = new Range<Vector3D>(Vector3D.UnitX, Vector3D.UnitY);
			Assert.AreEqual(Vector3D.UnitX, range.Start);
			Assert.AreEqual(Vector3D.UnitY, range.End);
		}

		[Test]
		public void ChangeRange()
		{
			var range = new Range<Vector3D>(Vector3D.UnitX, 2 * Vector3D.UnitX);
			range.Start = Vector3D.UnitY;
			range.End = 2 * Vector3D.UnitY;
			Assert.AreEqual(Vector3D.UnitY, range.Start);
			Assert.AreEqual(2 * Vector3D.UnitY, range.End);
		}

		[Test]
		public void GetRandomValue()
		{
			var range = new Range<Vector3D>(Vector3D.UnitX, 2 * Vector3D.UnitX);
			var random = range.GetRandomValue();
			Assert.IsTrue(random.X >= 1.0f && random.X <= 2.0f);
		}

		[Test]
		public void GetInterpolation()
		{
			var rangeLeft = new Range<Vector3D>(Vector3D.Zero, Vector3D.One);
			var rangeRight = new Range<Vector3D>(Vector3D.One, Vector3D.One * 3);
			var interpolation = rangeLeft.Lerp(rangeRight, 0.5f);
			var expectedInterpolation = new Range<Vector3D>(Vector3D.One * 0.5f, Vector3D.One * 2);
			Assert.AreEqual(expectedInterpolation.Start, interpolation.Start);
			Assert.AreEqual(expectedInterpolation.End, interpolation.End);
		}

		[Test]
		public void ResultOfToStringCanConvertBackVector2D()
		{
			var rangeVector2D = new Range<Vector2D>(Vector2D.One, Vector2D.ScreenRight);
			var converted2D = rangeVector2D.ToString();
			var rangeRetrieved2D = new Range<Vector2D>(converted2D);
			Assert.AreEqual("({1, 1},{1, 0})", converted2D);
			Assert.AreEqual(rangeVector2D.Start, rangeRetrieved2D.Start);
			Assert.AreEqual(rangeVector2D.End, rangeRetrieved2D.End);
		}

		[Test]
		public void ResultOfToStringCanConvertBackVector3D()
		{
			var rangeVector3D = new Range<Vector3D>(Vector3D.Zero, Vector3D.UnitX);
			var converted3D = rangeVector3D.ToString();
			var rangeRetrieved3D = new Range<Vector3D>(converted3D);
			Assert.AreEqual("({0, 0, 0},{1, 0, 0})", converted3D);
			Assert.AreEqual(rangeVector3D.Start, rangeRetrieved3D.Start);
			Assert.AreEqual(rangeVector3D.End, rangeRetrieved3D.End);
		}

		[Test]
		public void ResultOfToStringCanConvertBackVectorColor()
		{
			var rangeColor = new Range<Color>(Color.Red, Color.Black);
			var convertedColors = rangeColor.ToString();
			var rangeRetrieved = new Range<Color>(convertedColors);
			Assert.AreEqual("({R=255, G=0, B=0, A=255},{R=0, G=0, B=0, A=255})",
				convertedColors);
			Assert.AreEqual(rangeColor.Start, rangeRetrieved.Start);
			Assert.AreEqual(rangeColor.End, rangeRetrieved.End);
		}

		[Test]
		public void CreatingFromInvalidStringThrows()
		{
			Assert.Throws<Range<Vector2D>.InvalidStringFormat>(
				() => { new Range<Vector2D>("sdkfzjdk"); });
			Assert.Throws<Range<Vector2D>.InvalidStringFormat>(() =>
			{ new Range<Vector2D>("({1, 1};{1, 0})"); });
			Assert.Throws<Range<Vector3D>.TypeInStringNotEqualToInitializedType>(() =>
			{ new Range<Vector3D>("({1, 1},{1, 0})"); });
		}
	}
}