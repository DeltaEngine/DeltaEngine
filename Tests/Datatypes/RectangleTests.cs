using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class RectangleTests
	{
		[Test]
		public void Create()
		{
			var point = new Vector2D(2f, 2f);
			var size = new Size(1f, 1f);
			var rect = new Rectangle(point, size);
			Assert.AreEqual(point.X, rect.Left);
			Assert.AreEqual(point.Y, rect.Top);
			Assert.AreEqual(size.Width, rect.Width);
			Assert.AreEqual(size.Height, rect.Height);
			Assert.AreEqual(point, rect.TopLeft);
			Assert.AreEqual(size, rect.Size);
		}

		[Test]
		public void CreateFromFivePoints()
		{
			var points = new List<Vector2D> { Vector2D.Zero, Vector2D.One, Vector2D.One * 1.5f, 
				Vector2D.Half, -Vector2D.One };
			var rectangle = Rectangle.FromPoints(points);
			Assert.AreEqual(-Vector2D.One, rectangle.TopLeft);
			Assert.AreEqual(Vector2D.One * 1.5f, rectangle.BottomRight);
		}

		[Test]
		public void CreateFromTwoPoints()
		{
			var points = new List<Vector2D> { new Vector2D(4, 4), new Vector2D(3, 2) };
			var rectangle = Rectangle.FromPoints(points);
			Assert.AreEqual(new Vector2D(3, 2), rectangle.TopLeft);
			Assert.AreEqual(new Vector2D(4, 4), rectangle.BottomRight);
		}

		[Test]
		public void MergeRectangles()
		{
			var leftUpperRect = new Rectangle(-3.0f, -4.5f, 0.6f, 1.0f);
			var rightUpperRect = new Rectangle(5.0f, -3.0f, 1.0f, 1.0f);
			var leftLowerRect = new Rectangle(-4.0f, 6.2f, 1.0f, 0.8f);
			var rightLowerRect = new Rectangle(2.0f, 1.0f, 0.5f, 0.5f);
			Assert.AreEqual(new Rectangle(-3.0f, -4.5f, 9.0f, 2.5f), leftUpperRect.Merge(rightUpperRect));
			Assert.AreEqual(new Rectangle(2.0f, -3.0f, 4.0f, 4.5f), rightLowerRect.Merge(rightUpperRect));
			Assert.AreEqual(new Rectangle(-4.0f, -3.0f, 10.0f, 10.0f),
				leftLowerRect.Merge(rightUpperRect));
		}

		[Test]
		public void StaticRectangles()
		{
			Assert.AreEqual(new Rectangle(0, 0, 0, 0), Rectangle.Zero);
			Assert.AreEqual(new Rectangle(0, 0, 1, 1), Rectangle.One);
			Assert.AreEqual(new Rectangle(Vector2D.Unused, Size.Unused), Rectangle.Unused);
		}

		[Test]
		public void SizeOfRectangle()
		{
			Assert.AreEqual(16, Rectangle.SizeInBytes);
		}

		[Test]
		public void ChangeValues()
		{
			var rect = Rectangle.One;
			rect.Left = 2;
			rect.Top = 1;
			rect.Width = 2;
			rect.Height = 3;
			Assert.AreEqual(new Rectangle(2, 1, 2, 3), rect);
		}

		[Test]
		public void Equals()
		{
			var rect1 = new Rectangle(3, 4, 1, 2);
			var rect2 = new Rectangle(5, 6, 1, 2);
			Assert.AreNotEqual(rect1, rect2);
			Assert.AreEqual(rect1, new Rectangle(3, 4, 1, 2));
			Assert.AreNotEqual(rect1, new Rectangle(3.00001f, 4, 1, 2));
			Assert.IsTrue(rect1 == new Rectangle(3, 4, 1, 2));
			Assert.IsTrue(rect1 != rect2);
			Assert.IsFalse(rect1.Equals(rect2));
			Assert.IsTrue(rect1.Equals(rect1));
			Assert.False(rect1.Equals((object)Rectangle.One));
		}

		[Test]
		public void NearlyEquals()
		{
			var rect1 = new Rectangle(3, 4, 1, 2);
			Assert.IsTrue(rect1.IsNearlyEqual(new Rectangle(3.00001f, 4, 1, 2)));
			Assert.IsTrue(rect1.IsNearlyEqual(new Rectangle(3, 4, 0.99999f, 2)));
			Assert.IsFalse(rect1.IsNearlyEqual(new Rectangle(3.01f, 4, 1, 2)));
			Assert.IsFalse(rect1.IsNearlyEqual(new Rectangle(3, 4, 0.999f, 2)));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var rect1 = new Rectangle(3, 4, 1, 2);
			var rect2 = new Rectangle(5, 6, 1, 2);
			var rectValues = new Dictionary<Rectangle, int> { { rect1, 1 }, { rect2, 2 } };
			Assert.IsTrue(rectValues.ContainsKey(rect1));
			Assert.IsTrue(rectValues.ContainsKey(rect2));
			Assert.IsFalse(rectValues.ContainsKey(new Rectangle(3, 9, 1, 2)));
		}

		[Test]
		public void RectangleToString()
		{
			var v = new Vector2D(2f, 2f);
			var s = new Size(1f, 1f);
			var rect = new Rectangle(v, s);
			Assert.AreEqual("2, 2, 1, 1", rect.ToString());
		}

		[Test]
		public void RectangleToStringAndFromString()
		{
			var rect = new Rectangle(2.12f, 2.12f, 1.12f, 1.12f);
			string rectString = rect.ToString();
			Assert.AreEqual(rect, new Rectangle(rectString));
			Assert.AreEqual(Rectangle.One, new Rectangle("0, 0, 1, 1"));
			Assert.Throws<Rectangle.InvalidNumberOfComponents>(() => new Rectangle("abc"));
		}

		[Test]
		public void FromInvariantStringWithDatatypes()
		{
			Assert.AreEqual(Size.Zero, "0,0".Convert<Size>());
			Assert.AreEqual(Vector2D.UnitX, "1,0".Convert<Vector2D>());
			Assert.AreEqual(new Rectangle(1, 1, 2, 2), "1, 1, 2, 2".Convert<Rectangle>());
		}

		[Test]
		public  void TryingToConvertFromInvalidStringThrows()
		{
			Assert.Throws<Rectangle.InvalidNumberOfComponents>(() => { new Rectangle("1, 2, 2"); });
			Assert.Throws<Rectangle.TypeInStringNotEqualToInitializedType>(() => { new Rectangle("a, s, d, f"); });
		}

		[Test]
		public void Right()
		{
			var rect = new Rectangle(1, 2, 10, 20) { Right = 13 };
			Assert.AreEqual(3, rect.Left);
			Assert.AreEqual(13, rect.Right);
			Assert.AreEqual(10, rect.Width);
		}

		[Test]
		public void Bottom()
		{
			var rect = new Rectangle(1, 2, 10, 20) { Bottom = 23 };
			Assert.AreEqual(3, rect.Top);
			Assert.AreEqual(23, rect.Bottom);
			Assert.AreEqual(20, rect.Height);
		}

		[Test]
		public void TopRight()
		{
			var rect = new Rectangle(1, 2, 10, 20);
			Assert.AreEqual(new Vector2D(11, 2), rect.TopRight);
		}

		[Test]
		public void BottomLeft()
		{
			var rect = new Rectangle(1, 2, 10, 20);
			Assert.AreEqual(new Vector2D(1, 22), rect.BottomLeft);
		}

		[Test]
		public void BottomRight()
		{
			var rect = new Rectangle(1, 2, 10, 20);
			Assert.AreEqual(new Vector2D(11, 22), rect.BottomRight);
		}

		[Test]
		public void GetCenter()
		{
			var rect = new Rectangle(4, 4, 4, 4);
			Assert.AreEqual(new Vector2D(4, 4), rect.TopLeft);
			Assert.AreEqual(new Vector2D(8, 8), rect.BottomRight);
			Assert.AreEqual(new Vector2D(6, 6), rect.Center);
		}

		[Test]
		public void SetCenter()
		{
			var rect = new Rectangle(8, 10, 2, 2) { Center = Vector2D.One };
			Assert.AreEqual(new Vector2D(0, 0), rect.TopLeft);
			Assert.AreEqual(new Vector2D(2, 2), rect.BottomRight);
			Assert.AreEqual(new Vector2D(1, 1), rect.Center);
		}

		[Test]
		public void Contains()
		{
			var rect = new Rectangle(1, 2, 10, 20);
			Assert.IsTrue(rect.Contains(new Vector2D(1, 2)));
			Assert.IsTrue(rect.Contains(new Vector2D(5, 5)));
			Assert.IsFalse(rect.Contains(new Vector2D(11, 5)));
			Assert.IsFalse(rect.Contains(new Vector2D(5, 22)));
		}

		[Test]
		public void Lerp()
		{
			Assert.AreEqual(Rectangle.One, Rectangle.Zero.Lerp(Rectangle.One, 1.0f));
			Assert.AreEqual(new Rectangle(0.5f, 0.5f, 1, 1),
				Rectangle.Zero.Lerp(new Rectangle(1, 1, 2, 2), 0.5f));
		}

		[Test]
		public void FromCenter()
		{
			Rectangle rect = Rectangle.FromCenter(new Vector2D(11, 12), new Size(4, 6));
			Assert.AreEqual(new Rectangle(9, 9, 4, 6), rect);
			Rectangle anotherRect = Rectangle.FromCenter(0.5f, 0.5f, 1.0f, 1.0f);
			Assert.AreEqual(new Rectangle(0, 0, 1, 1), anotherRect);
		}

		[Test]
		public void FromCorners()
		{
			Rectangle rect = Rectangle.FromCorners(new Vector2D(1, 2), new Vector2D(3, 5));
			Assert.AreEqual(new Rectangle(new Vector2D(1, 2), new Size(2, 3)), rect);
		}

		[Test]
		public void Aspect()
		{
			Assert.AreEqual(0.5f, new Rectangle(0, 0, 1, 2).Aspect);
			Assert.AreEqual(2.0f, new Rectangle(0, 0, 4, 2).Aspect);
		}

		[Test]
		public void Increase()
		{
			var rect = new Rectangle(1, 1, 2, 2);
			Assert.AreEqual(new Rectangle(0.9f, 0.9f, 2.2f, 2.2f), rect.Increase(new Size(0.2f)));
		}

		[Test]
		public void Reduce()
		{
			var rect = new Rectangle(1, 1, 2, 2);
			Assert.AreEqual(new Rectangle(1.5f, 1.5f, 1, 1), rect.Reduce(Size.One));
		}

		[Test]
		public void GetInnerRectangle()
		{
			var rect = new Rectangle(1, 1, 2, 2);
			Assert.AreEqual(rect, rect.GetInnerRectangle(Rectangle.One));
			Assert.AreEqual(new Rectangle(1.0f, 1.0f, 1.0f, 1.0f),
				rect.GetInnerRectangle(new Rectangle(0.0f, 0.0f, 0.5f, 0.5f)));
			Assert.AreEqual(new Rectangle(2.0f, 2.0f, 1.0f, 1.0f),
				rect.GetInnerRectangle(new Rectangle(0.5f, 0.5f, 0.5f, 0.5f)));
		}

		[Test]
		public void GetRelativePoint()
		{
			var rect = new Rectangle(1, 2, 3, 4);
			Assert.AreEqual(Vector2D.Zero, rect.GetRelativePoint(new Vector2D(1, 2)));
			Assert.AreEqual(Vector2D.One, rect.GetRelativePoint(new Vector2D(4, 6)));
			Assert.AreEqual(new Vector2D(-1, -2), rect.GetRelativePoint(new Vector2D(-2, -6)));
		}

		[Test]
		public void Move()
		{
			var rect = new Rectangle(1, 1, 1, 1);
			Assert.AreEqual(rect, rect.Move(Vector2D.Zero));
			Assert.AreEqual(new Rectangle(2.0f, 2.0f, 1.0f, 1.0f), rect.Move(Vector2D.One));
			Assert.AreEqual(new Rectangle(-1.0f, -2.0f, 1.0f, 1.0f), rect.Move(-2, -3));
		}

		[Test]
		public void GetRotatedRectangleCornersWithoutRotation()
		{
			var points = new Rectangle(1, 1, 1, 1).GetRotatedRectangleCorners(Vector2D.Zero, 0);
			Assert.AreEqual(4, points.Length);
			Assert.AreEqual(Vector2D.One, points[0]);
			Assert.AreEqual(new Vector2D(1, 2), points[1]);
			Assert.AreEqual(new Vector2D(2, 2), points[2]);
		}

		[Test]
		public void GetRotatedRectangleCornersWith180DegreesRotation()
		{
			var points = new Rectangle(1, 1, 1, 1).GetRotatedRectangleCorners(Vector2D.Zero, 180);
			Assert.IsTrue(points[0].IsNearlyEqual(-Vector2D.One));
			Assert.IsTrue(points[1].IsNearlyEqual(-new Vector2D(1, 2)));
			Assert.IsTrue(points[2].IsNearlyEqual(-new Vector2D(2, 2)));
		}

		[Test]
		public void IsColliding()
		{
			var screenRect = Rectangle.One;
			var insideRect = new Rectangle(0.1f, 0.1f, 2.9f, 0.3f);
			var outsideRect = new Rectangle(2.4f, 0.35f, 0.1f, 0.1f);
			Assert.IsTrue(insideRect.IsColliding(0, screenRect, 0));
			Assert.IsFalse(outsideRect.IsColliding(0, screenRect, 0));
			Assert.IsTrue(outsideRect.IsColliding(0, insideRect, 0));
			Assert.IsFalse(outsideRect.IsColliding(0, insideRect, 70));
		}

		[Test]
		public void IsCollidingTopBottom()
		{
			var topRect = new Rectangle(0.44f, 0.4f, 0.05f, 0.03f);
			var bottomRect = new Rectangle(0.44f, 0.44f, 0.04f, 0.03f);
			Assert.IsFalse(topRect.IsColliding(0, bottomRect, 0));
			Assert.IsFalse(bottomRect.IsColliding(0, topRect, 0));
		}

		[Test]
		public void IsOneRectangleCollidingWhenInsideAnother()
		{
			var insideRect = new Rectangle(0.3f, 0.3f, 0.1f, 0.1f);
			var outsideRect = new Rectangle(0.2f, 0.2f, 0.3f, 0.3f);
			Assert.IsTrue(outsideRect.IsColliding(0, insideRect, 0));
			Assert.IsTrue(outsideRect.IsColliding(0, insideRect, 70));
		}

		[Test]
		public void CheckIntersectionWithCircle()
		{
			var rectangle = new Rectangle(-1.0f, -1.0f, 2.0f, 2.0f);
			Assert.IsFalse(rectangle.IntersectsCircle(new Vector2D(2.0f, 2.0f), 0.5f));
			Assert.IsFalse(rectangle.IntersectsCircle(new Vector2D(-1.5f, -1.0f), 0.4f));
			Assert.IsTrue(rectangle.IntersectsCircle(Vector2D.Zero, 3.0f));
			Assert.IsTrue(rectangle.IntersectsCircle(Vector2D.Half, 0.1f));
			Assert.IsTrue(rectangle.IntersectsCircle(new Vector2D(1.2f, 1.2f), 0.6f));
			Assert.IsTrue(rectangle.IntersectsCircle(new Vector2D(0.0f, -2.0f), 10.0f));
		}

		[Test]
		public void InitializeRectangleFromPoints()
		{
			var points =
				new List<Vector2D>(new[]
				{
					new Vector2D(1.0f, 3.0f), new Vector2D(4.0f, 2.0f), new Vector2D(1.0f, 6.0f),
					new Vector2D(1.7f, 4.3f)
				});
			var rectangle = Rectangle.FromPoints(points);
			Assert.AreEqual(new Rectangle(1.0f, 2.0f, 3.0f, 4.0f), rectangle);
		}

		[Test]
		public void RotateBoundingBox()
		{
			var boundingBox = new Rectangle(1, 1, 1, 1).GetBoundingBoxAfterRotation(90);
			Assert.AreEqual(new Rectangle(1, 1, 1, 1), boundingBox);
		}
	}
}