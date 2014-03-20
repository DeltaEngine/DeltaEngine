using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class SizeTests
	{
		[Test]
		public void StaticSizes()
		{
			Assert.AreEqual(new Size(), Size.Zero);
			Assert.AreEqual(new Size(0.0f, 0.0f), Size.Zero);
			Assert.AreEqual(new Size(0.5f, 0.5f), Size.Half);
			Assert.AreEqual(new Size(1.0f, 1.0f), Size.One);
			Assert.AreEqual(new Size(-1.0f, -1.0f), Size.Unused);
			Assert.AreEqual(8, Size.SizeInBytes);
		}

		[Test]
		public void CreateWithEqualWidthAndHeight()
		{
			var size = new Size(-1.2f);
			Assert.AreEqual(-1.2f, size.Width);
			Assert.AreEqual(-1.2f, size.Height);
		}

		[Test]
		public void CreateFromString()
		{
			var size = new Size("1.2, 2.4");
			Assert.AreEqual(1.2f, size.Width);
			Assert.AreEqual(2.4f, size.Height);
		}

		[Test]
		public void CreateFromInvalidStringCrashes()
		{
			Assert.Throws<Size.InvalidNumberOfComponents>(() => new Size("1"));
			Assert.Throws<FormatException>(() => new Size("a, b"));
		}

		[Test]
		public void ChangeSize()
		{
			var size = new Size(1.0f, 1.0f) { Height = 2.1f, Width = 2.1f };
			Assert.AreEqual(2.1f, size.Height);
			Assert.AreEqual(2.1f, size.Width);
		}

		[Test]
		public void SizeHalf()
		{
			Assert.AreEqual(new Size(0.5f, 0.5f), Size.Half);
		}

		[Test]
		public void CreateFromFloat()
		{
			const float Width = 3.51f;
			const float Height = 0.23f;
			var s = new Size(Width, Height);
			Assert.AreEqual(s.Width, Width);
			Assert.AreEqual(s.Height, Height);
		}

		[Test]
		public void Equals()
		{
			var s1 = new Size(1, 2);
			var s2 = new Size(3, 4);
			Assert.AreNotEqual(s1, s2);
			Assert.AreEqual(s1, new Size(1, 2));
			Assert.AreNotEqual(s1, new Size(1, 2.00001f));
			Assert.AreNotEqual(s1, new Vector2D(1, 2));
			Assert.IsTrue(s1 == new Size(1, 2));
			Assert.IsTrue(s1 != s2);
		}

		[Test]
		public void NearlyEquals()
		{
			var s1 = new Size(1, 2);
			Assert.IsTrue(s1.IsNearlyEqual(new Size(0.99999f, 2.00001f)));
			Assert.IsTrue(s1.IsNearlyEqual(new Size(1, 2.00001f)));
			Assert.IsFalse(s1.IsNearlyEqual(new Size(0.9f, 2.00001f)));
			Assert.IsFalse(s1.IsNearlyEqual(new Size(1, 2.1f)));
		}

		[Test]
		public void PlusOperator()
		{
			var s1 = new Size(2, 2);
			var s2 = new Size(3, 3);
			Size plus = s1 + s2;
			Assert.AreEqual(plus, new Size(5f, 5f));
		}

		[Test]
		public void MinusOperator()
		{
			var s1 = new Size(2, 2);
			var s2 = new Size(1, 1);
			Size minus = s1 - s2;
			Assert.AreEqual(minus, new Size(1f, 1f));
		}

		[Test]
		public void MultiplyOperator()
		{
			var s1 = new Size(2, 3);
			var s2 = new Size(4, 5);
			Size multiply = s1 * s2;
			Assert.AreEqual(multiply, new Size(8, 15));
		}

		[Test]
		public void MultiplyByFloat()
		{
			Assert.AreEqual(new Size(5, 10), (new Size(2, 4) * 2.5f));
			Assert.AreEqual(new Size(5, 10), 2.5f * (new Size(2, 4)));
		}

		[Test]
		public void DivideSizeByFloat()
		{
			Size divide = new Size(4, 5) / 2.0f;
			Assert.AreEqual(new Size(2.0f, 2.5f), divide);
		}

		[Test]
		public void DivideFloatBySize()
		{
			Size divide = 2.0f / new Size(4, 5);
			Assert.AreEqual(new Size(0.5f, 0.4f), divide);
		}

		[Test]
		public void DivideSizeBySize()
		{
			Size divide = new Size(4, 5) / new Size(2, 2);
			Assert.AreEqual(new Size(2.0f, 2.5f), divide);
		}
		
		[Test]
		public void ExplicitCastFromVector2D()
		{
			var p = new Vector2D(1, 2);
			var s = new Size(1, 2);
			Size addition = (Size)p + s;
			Assert.AreEqual(new Size(2, 4), addition);
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var first = new Size(1, 2);
			var second = new Size(3, 4);
			var sizeValues = new Dictionary<Size, int> { { first, 1 }, { second, 2 } };
			Assert.IsTrue(sizeValues.ContainsKey(first));
			Assert.IsTrue(sizeValues.ContainsKey(second));
			Assert.IsFalse(sizeValues.ContainsKey(new Size(5, 6)));
		}

		[Test]
		public void SizeToString()
		{
			var testSize = new Size(2.23f, 3.45f);
			Assert.AreEqual("2.23, 3.45", testSize.ToString());
			Assert.AreEqual(testSize, new Size(testSize.ToString()));
		}

		[Test]
		public void SizeToStringAndFromString()
		{
			var s = new Size(2.23f, 3.45f);
			string sizeString = s.ToString();
			Assert.AreEqual(s, new Size(sizeString));
			Assert.Throws<Size.InvalidNumberOfComponents>(() => new Size("10"));
			Assert.Throws<FormatException>(() => new Size("abc"));
		}

		[Test]
		public void Length()
		{
			Assert.AreEqual(3, new Size(0, 3).Length);
			Assert.AreEqual(3, new Size(3, 0).Length);
			Assert.AreEqual(5, new Size(3, 4).Length);
		}

		[Test]
		public void Lerp()
		{
			var size1 = new Size(10, 20);
			var size2 = new Size(20, 30);
			var lerp20 = new Size(12, 22);
			Assert.AreEqual(lerp20, size1.Lerp(size2, 0.2f));
			Assert.AreEqual(size1, size1.Lerp(size2, 0.0f));
			Assert.AreEqual(size2, size1.Lerp(size2, 1.0f));
		}

		[Test]
		public void AspectRatio()
		{
			var portrait = new Size(0.5f, 1.0f);
			Size square = Size.One;
			var landscape = new Size(1.0f, 0.5f);
			Assert.AreEqual(0.5f, portrait.AspectRatio);
			Assert.AreEqual(1.0f, square.AspectRatio);
			Assert.AreEqual(2.0f, landscape.AspectRatio);
		}
	}
}