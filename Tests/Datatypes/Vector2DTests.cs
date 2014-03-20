using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class Vector2DTests
	{
		[SetUp]
		public void SetUp()
		{
			v1 = new Vector2D(1, 2);
			v2 = new Vector2D(3, -4);
		}

		private Vector2D v1;
		private Vector2D v2;

		[Test]
		public void Create()
		{
			const float X = 3.51f;
			const float Y = 0.23f;
			var v = new Vector2D(X, Y);
			Assert.AreEqual(v.X, X);
			Assert.AreEqual(v.Y, Y);
		}

		[Test]
		public void Statics()
		{
			Assert.AreEqual(new Vector2D(0, 0), Vector2D.Zero);
			Assert.AreEqual(new Vector2D(1, 1), Vector2D.One);
			Assert.AreEqual(new Vector2D(0.5f, 0.5f), Vector2D.Half);
			Assert.AreEqual(new Vector2D(1, 0), Vector2D.UnitX);
			Assert.AreEqual(new Vector2D(0, 1), Vector2D.UnitY);
			Assert.AreEqual(new Vector2D(1, 0), Vector2D.ScreenRight);
			Assert.AreEqual(new Vector2D(-1, 0), Vector2D.ScreenLeft);
			Assert.AreEqual(new Vector2D(0, -1), Vector2D.ScreenUp);
			Assert.AreEqual(new Vector2D(0, 1), Vector2D.ScreenDown);
			Assert.AreEqual(8, Vector2D.SizeInBytes);
		}

		[Test]
		public void ChangePoint()
		{
			var v = new Vector2D(1.0f, 1.0f) { X = 2.1f, Y = 2.1f };
			Assert.AreEqual(2.1f, v.X);
			Assert.AreEqual(2.1f, v.Y);
		}

		[Test]
		public void Addition()
		{
			Assert.AreEqual(new Vector2D(4, -2), v1 + v2);
		}

		[Test]
		public void Subtraction()
		{
			Assert.AreEqual(new Vector2D(-2, 6), v1 - v2);
		}

		[Test]
		public void Negation()
		{
			Assert.AreEqual(-v1, new Vector2D(-1, -2));
		}

		[Test]
		public void Multiplication()
		{
			var v = new Vector2D(2, 4);
			const float F = 1.5f;
			var s = new Size(F);
			Assert.AreEqual(new Vector2D(3, 6), v * F);
			Assert.AreEqual(new Vector2D(3, 6), v * s);
		}

		[Test]
		public void Division()
		{
			var v = new Vector2D(2, 4);
			const float F = 2f;
			var s = new Size(F);
			Assert.AreEqual(new Vector2D(1, 2), v / F);
			Assert.AreEqual(new Vector2D(1, 2), v / s);
		}

		[Test]
		public void Equals()
		{
			Assert.AreNotEqual(v1, v2);
			Assert.AreEqual(v1, new Vector2D(1, 2));
			Assert.IsTrue(v1 == new Vector2D(1, 2));
			Assert.IsTrue(v1 != v2);
			Assert.IsTrue(v1.Equals((object)new Vector2D(1, 2)));
		}

		[Test]
		public void NearlyEquals()
		{
			Assert.IsTrue(v1.IsNearlyEqual(new Vector2D(1.000001f, 1.99999f)));
			Assert.IsFalse(v1.IsNearlyEqual(new Vector2D(1, 2.1f)));
		}

		[Test]
		public void ImplicitCastFromSize()
		{
			var v = new Vector2D(1, 2);
			var s = new Size(1, 2);
			Vector2D addition = v + s;
			Assert.AreEqual(new Vector2D(2, 4), addition);
		}

		[Test]
		public void DistanceTo()
		{
			var zero = new Vector2D();
			var v = new Vector2D(3, 4);
			Assert.AreEqual(5, zero.DistanceTo(v));
			Assert.AreEqual(0, zero.DistanceTo(zero));
		}

		[Test]
		public void DistanceToSquared()
		{
			var zero = new Vector2D();
			var v = new Vector2D(3, 4);
			Assert.AreEqual(25, zero.DistanceToSquared(v));
			Assert.AreEqual(0, zero.DistanceToSquared(zero));
		}

		[Test]
		public void DirectionTo()
		{
			Assert.AreEqual(new Vector2D(2, -6), v1.DirectionTo(v2));
		}

		[Test]
		public void Length()
		{
			Assert.AreEqual(5, new Vector2D(-3, 4).Length);
			Assert.AreEqual(13, new Vector2D(5, -12).Length);
		}

		[Test]
		public void LengthSquared()
		{
			Assert.AreEqual(5, new Vector2D(1, -2).LengthSquared);
			Assert.AreEqual(8.5f, new Vector2D(-1.5f, 2.5f).LengthSquared);
		}

		[Test]
		public void GetRotation()
		{
			Assert.AreEqual(0, new Vector2D(1, 0).GetRotation());
			Assert.AreEqual(90, new Vector2D(0, 1).GetRotation());
			Assert.AreEqual(180, new Vector2D(-1, 0).GetRotation());
			Assert.AreEqual(-90, new Vector2D(0, -1).GetRotation());
			Assert.AreEqual(45, new Vector2D(1, 1).GetRotation());
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var vector2DValues = new Dictionary<Vector2D, int> { { v1, 1 }, { v2, 2 } };
			Assert.IsTrue(vector2DValues.ContainsKey(v1));
			Assert.IsTrue(vector2DValues.ContainsKey(v2));
			Assert.IsFalse(vector2DValues.ContainsKey(new Vector2D(5, 6)));
		}

		[Test]
		public void PointToString()
		{
			Assert.AreEqual("3, 4", new Vector2D(3, 4).ToString());
		}

		[Test]
		public void PointToStringAndFromString()
		{
			var v = new Vector2D(2.23f, 3.45f);
			string pointString = v.ToString();
			Assert.AreEqual(v, new Vector2D(pointString));
			Assert.Throws<Vector2D.InvalidNumberOfComponents>(() => new Vector2D("0.0"));
		}

		[Test]
		public void ReflectIfHittingBorderAtCorners()
		{
			var direction = Vector2D.One;
			var borders = Rectangle.One;
			var bottomRightArea = new Rectangle(Vector2D.One, Size.Half);
			direction.ReflectIfHittingBorder(bottomRightArea, borders);
			Assert.AreEqual(-Vector2D.One, direction);
			var topLeftArea = new Rectangle(Vector2D.Zero, Size.Zero);
			direction.ReflectIfHittingBorder(topLeftArea, borders);
			Assert.AreEqual(Vector2D.One, direction);
		}

		[Test]
		public void PointInsideBordersHasNoReflection()
		{
			var direction = Vector2D.One;
			var areaInsideBorders = new Rectangle(Vector2D.Half, Size.One * 2);
			var borders = Rectangle.One;
			direction.ReflectIfHittingBorder(areaInsideBorders, borders);
			Assert.AreEqual(Vector2D.One, direction);
		}

		[Test]
		public void Lerp()
		{
			Assert.AreEqual(Vector2D.Half, Vector2D.Zero.Lerp(Vector2D.One, 0.5f));
			Assert.AreEqual(Vector2D.One, Vector2D.Zero.Lerp(Vector2D.One, 1.0f));
			Assert.AreEqual(new Vector2D(1.5f, 1.0f), new Vector2D(1, 2).Lerp(new Vector2D(5, -6), 0.125f));
		}

		[Test]
		public void Rotate()
		{
			Assert.IsTrue(Vector2D.UnitX.Rotate(90.0f).IsNearlyEqual(Vector2D.UnitY));
			Assert.IsTrue(Vector2D.UnitX.Rotate(-90.0f).IsNearlyEqual(-Vector2D.UnitY));
			Assert.IsTrue(Vector2D.UnitY.Rotate(-90.0f).IsNearlyEqual(Vector2D.UnitX));
		}

		[Test]
		public void RotateAround()
		{
			Assert.IsTrue(Vector2D.UnitX.RotateAround(Vector2D.Zero, 90.0f).IsNearlyEqual(Vector2D.UnitY));
			Assert.IsTrue(
				Vector2D.UnitY.RotateAround(new Vector2D(0.0f, 0.5f), 180.0f).IsNearlyEqual(Vector2D.Zero));
		}

		[Test]
		public void RotationTo()
		{
			var v = Vector2D.UnitX;
			var rotation = v.RotationTo(Vector2D.Zero);
			Assert.AreEqual(0, rotation);
			Assert.IsTrue(v.RotateAround(Vector2D.Zero, 90.0f).IsNearlyEqual(Vector2D.UnitY));
			rotation = Vector2D.UnitY.RotationTo(Vector2D.Zero);
			Assert.AreEqual(90, rotation);
		}

		[Test]
		public void Normalize()
		{
			var v = Vector2D.Normalize(new Vector2D(0.3f, -0.4f));
			Assert.AreEqual(new Vector2D(0.6f, -0.8f), v);
		}

		[Test]
		public void DotProduct()
		{
			var v3 = Vector2D.Normalize(new Vector2D(1, 1));
			var v4 = Vector2D.Normalize(new Vector2D(-1, 1));
			Assert.AreEqual(0.0f, v3.DotProduct(v4));
			Assert.AreEqual(0.7071f, v3.DotProduct(Vector2D.UnitY), 0.0001f);
		}

		[Test]
		public void DistanceToLine()
		{
			Assert.AreEqual(1, Vector2D.UnitX.DistanceToLine(Vector2D.Zero, Vector2D.Zero));
			Assert.AreEqual(0, Vector2D.Zero.DistanceToLine(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(0, Vector2D.UnitX.DistanceToLine(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(0, (-Vector2D.UnitX).DistanceToLine(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(1, Vector2D.UnitY.DistanceToLine(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(5, (Vector2D.One * 5).DistanceToLine(Vector2D.Zero, Vector2D.UnitX));
		}

		[Test]
		public void DistanceToLineSquared()
		{
			Assert.AreEqual(1, Vector2D.UnitX.DistanceToLineSquared(Vector2D.Zero, Vector2D.Zero));
			Assert.AreEqual(0, Vector2D.Zero.DistanceToLineSquared(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(0, Vector2D.UnitX.DistanceToLineSquared(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(0, (-Vector2D.UnitX).DistanceToLineSquared(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(1, Vector2D.UnitY.DistanceToLineSquared(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(25, (Vector2D.One * 5).DistanceToLineSquared(Vector2D.Zero, Vector2D.UnitX));
		}

		[Test]
		public void DistanceToLineSegment()
		{
			Assert.AreEqual(1, Vector2D.UnitX.DistanceToLineSegment(Vector2D.Zero, Vector2D.Zero));
			Assert.AreEqual(0, Vector2D.Zero.DistanceToLineSegment(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(1, Vector2D.UnitY.DistanceToLineSegment(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(5, new Vector2D(6, 0).DistanceToLineSegment(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(5, new Vector2D(-3, -4).DistanceToLineSegment(Vector2D.Zero, Vector2D.UnitX));
			Assert.AreEqual(0, Vector2D.Half.DistanceToLineSegment(Vector2D.UnitX, Vector2D.UnitY));
		}

		[Test]
		public void IsLeftOfLine()
		{
			Assert.IsTrue(Vector2D.Zero.IsLeftOfLineOrOnIt(Vector2D.Zero, Vector2D.UnitX));
			Assert.IsTrue(Vector2D.UnitY.IsLeftOfLineOrOnIt(Vector2D.Zero, Vector2D.UnitX));
			Assert.IsFalse((-Vector2D.UnitY).IsLeftOfLineOrOnIt(Vector2D.Zero, Vector2D.UnitX));
			Assert.IsTrue(Vector2D.UnitY.IsLeftOfLineOrOnIt(Vector2D.Zero, Vector2D.One));
			Assert.IsFalse(Vector2D.UnitX.IsLeftOfLineOrOnIt(Vector2D.Zero, Vector2D.One));
		}

		[Test]
		public void AngleBetweenVector()
		{
			Assert.AreEqual(90, Vector2D.UnitY.AngleBetweenVector(Vector2D.UnitX));
		}
	}
}