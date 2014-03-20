using System;
using System.Collections.Generic;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Shapes.Tests
{
	public class Triangle2DTests
	{
		[Test]
		public void DefaultConstructor()
		{
			var triangle = new Triangle2D();
			Assert.AreEqual(Vector2D.Zero, triangle.Corner1);
			Assert.AreEqual(Vector2D.Zero, triangle.Corner2);
			Assert.AreEqual(Vector2D.Zero, triangle.Corner3);
			Assert.AreEqual(triangle, Triangle2D.Zero);
		}

		[Test]
		public void SizeOfTriangle2D()
		{
			Assert.AreEqual(24, Triangle2D.SizeInBytes);
		}

		[Test]
		public void Constructor()
		{
			var triangle = new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6));
			Assert.AreEqual(new Vector2D(1, 2), triangle.Corner1);
			Assert.AreEqual(new Vector2D(3, 4), triangle.Corner2);
			Assert.AreEqual(new Vector2D(5, 6), triangle.Corner3);
		}

		[Test]
		public void Corner1()
		{
			var triangle = new Triangle2D { Corner1 = Vector2D.Half };
			Assert.AreEqual(Vector2D.Half, triangle.Corner1);
		}

		[Test]
		public void Corner2()
		{
			var triangle = new Triangle2D { Corner2 = Vector2D.Half };
			Assert.AreEqual(Vector2D.Half, triangle.Corner2);
		}

		[Test]
		public void Corner3()
		{
			var triangle = new Triangle2D { Corner3 = Vector2D.Half };
			Assert.AreEqual(Vector2D.Half, triangle.Corner3);
		}

		[Test]
		public void Equals()
		{
			var triangle1 = new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6));
			var triangle2 = new Triangle2D(new Vector2D(11, 12), new Vector2D(13, 14),
				new Vector2D(15, 16));
			Assert.AreNotEqual(triangle1, triangle2);
			Assert.AreEqual(triangle1,
				new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6)));
			Assert.IsTrue(triangle1 ==
				new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6)));
			Assert.IsTrue(triangle1 != triangle2);
			Assert.IsFalse(triangle1.Equals(triangle2));
			Assert.IsTrue(triangle1.Equals(triangle1));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var triangle1 = new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6));
			var triangle1Similar = new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4),
				new Vector2D(15, 16));
			var triangle2 = new Triangle2D(new Vector2D(11, 12), new Vector2D(13, 14),
				new Vector2D(15, 16));
			var triangles = new Dictionary<Triangle2D, int> { { triangle1, 1 }, { triangle2, 2 } };
			Assert.IsTrue(triangles.ContainsKey(triangle1));
			Assert.IsTrue(triangles.ContainsKey(triangle2));
			Assert.IsFalse(
				triangles.ContainsKey(new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4),
					new Vector2D(5, 7))));
			Assert.AreNotEqual(triangle1.GetHashCode(), triangle1Similar.GetHashCode());
		}

		[Test]
		public void Triangle2DToString()
		{
			var triangle = new Triangle2D(new Vector2D(1, 2), new Vector2D(3, 4), new Vector2D(5, 6));
			Assert.AreEqual("1, 2 3, 4 5, 6", triangle.ToString());
		}

		[Test]
		public void Triangle2DToStringAndFromString()
		{
			var triangle = new Triangle2D(new Vector2D(1.2f, 2.3f), new Vector2D(-3.4f, 4.5f),
				new Vector2D(0.0f, -5.67f));
			string triangleString = triangle.ToString();
			Assert.AreEqual(triangle, new Triangle2D(triangleString));
			Assert.Throws<Triangle2D.InvalidNumberOfComponents>(() => new Triangle2D("1, 2, 3"));
			Assert.Throws<FormatException>(() => new Triangle2D("abc"));
		}

		[Test]
		public void SaveAndLoad()
		{
			var data = BinaryDataExtensions.SaveToMemoryStream(Triangle2D.Zero);
			byte[] savedBytes = data.ToArray();
			Assert.AreEqual(1 + 4 + "Triangle2D".Length + Triangle2D.SizeInBytes, savedBytes.Length);
			Assert.AreEqual("Triangle2D".Length, savedBytes[0]);
			var reconstructed = data.CreateFromMemoryStream();
			Assert.AreEqual(Triangle2D.Zero, reconstructed);
		}
	}
}