using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Extensions
{
	public class MathExtensionsTests
	{
		private const float Precision = 0.0001f;

		[Test]
		public void Abs()
		{
			Assert.AreEqual(5, MathExtensions.Abs(2) + MathExtensions.Abs(3));
			Assert.AreEqual(5, MathExtensions.Abs(-2) + MathExtensions.Abs(3));

			Assert.AreEqual(5, 2.0f.Abs() + 3.0f.Abs());
			Assert.AreEqual(5, (-2.0f).Abs() + 3.0f.Abs());
		}

		[Test]
		public void IsNearlyEqual()
		{
			Assert.IsTrue(2.0f.IsNearlyEqual(2.00001f));
			Assert.IsFalse(2.0f.IsNearlyEqual(2.0002f));
			Assert.IsTrue(MathExtensions.Pi.IsNearlyEqual(3.1415f));
		}

		[Test]
		public static void Round()
		{
			Assert.AreEqual(1, 1.25f.Round());
			Assert.AreEqual(10, 9.68f.Round());
			Assert.AreEqual(1.23f, 1.2345f.Round(2));
		}

		[Test]
		public void Sin()
		{
			Assert.AreEqual(0, MathExtensions.Sin(0));
			Assert.AreEqual(1, MathExtensions.Sin(90));
			Assert.AreEqual(0, MathExtensions.Sin(180), Precision);
			Assert.AreEqual(0, MathExtensions.Sin(360), Precision);
			Assert.AreNotEqual(0, MathExtensions.Sin(32));
		}

		[Test]
		public void Cos()
		{
			Assert.AreEqual(1, MathExtensions.Cos(0));
			Assert.AreEqual(0, MathExtensions.Cos(90), Precision);
			Assert.AreEqual(-1, MathExtensions.Cos(180), Precision);
			Assert.AreEqual(1, MathExtensions.Cos(360), Precision);
			Assert.AreNotEqual(0, MathExtensions.Cos(32));
		}

		[Test]
		public void Tan()
		{
			Assert.AreEqual(0.0f, MathExtensions.Tan(0.0f), Precision);
			Assert.AreEqual(0.0f, MathExtensions.Tan(180.0f), Precision);
			Assert.AreEqual(1.0f, MathExtensions.Tan(45.0f), Precision);
		}

		[Test]
		public void Asin()
		{
			Assert.AreEqual(0.0f, MathExtensions.Asin(0));
			Assert.AreEqual(90.0f, MathExtensions.Asin(1));
			Assert.AreEqual(-90.0f, MathExtensions.Asin(-1));
		}

		[Test]
		public void Atan2()
		{
			Assert.AreEqual(0.0f, MathExtensions.Atan2(0, 0));
			Assert.AreEqual(90.0f, MathExtensions.Atan2(1, 0));
			Assert.AreEqual(-135.0f, MathExtensions.Atan2(-1, -1));
		}

		[Test]
		public void Sqrt()
		{
			Assert.AreEqual(0.0f, MathExtensions.Sqrt(0.0f));
			Assert.AreEqual(2.0f, MathExtensions.Sqrt(4.0f));
			Assert.AreEqual(9.0f, MathExtensions.Sqrt(81.0f));
		}

		[Test]
		public void Clamp()
		{
			Assert.AreEqual(0, 0.Clamp(0, 1));
			Assert.AreEqual(90.0f, 100.0f.Clamp(0.0f, 90.0f));
		}

		[Test]
		public void Lerp()
		{
			Assert.AreEqual(1, MathExtensions.Lerp(1, 3, 0));
			Assert.AreEqual(0.2f, MathExtensions.Lerp(0, 1, 0.2f));
			Assert.AreEqual(0, MathExtensions.Lerp(-1, 1, 0.5f));
			Assert.AreEqual(0.5f, MathExtensions.Lerp(-4, -1, 1.5f));
		}

		[Test]
		public void RadiansToDegrees()
		{
			Assert.AreEqual(0.0f, MathExtensions.RadiansToDegrees(0));
			Assert.AreEqual(572.95776f, MathExtensions.RadiansToDegrees(10));
			Assert.AreEqual(0.0f, 0.0f.RadiansToDegrees());
			Assert.AreEqual(180.0f, MathExtensions.Pi.RadiansToDegrees());
			Assert.AreEqual(-360.0f, -2.0f * MathExtensions.Pi.RadiansToDegrees());
			Assert.AreEqual(1234.0f, 1234.0f.RadiansToDegrees().DegreesToRadians());
		}

		[Test]
		public void DegreesToRadians()
		{
			Assert.AreEqual(0.0f, MathExtensions.DegreesToRadians(0));
			Assert.AreEqual(MathExtensions.Pi, 180.0f.DegreesToRadians());
			Assert.AreEqual(-2.0f * MathExtensions.Pi, -360.0f.DegreesToRadians());
			Assert.AreEqual(1234.0f, 1234.0f.DegreesToRadians().RadiansToDegrees());
		}

		[Test]
		public void Max()
		{
			Assert.AreEqual(3.5f, MathExtensions.Max(3.5f, 2.5f));
			Assert.AreEqual(-1.5f, MathExtensions.Max(-3.5f, -1.5f));
			Assert.AreEqual(-2.5f, MathExtensions.Max(-2.5f, -2.5f));
			Assert.AreEqual(-2, MathExtensions.Max(-2, -3));
		}

		[Test]
		public void Min()
		{
			Assert.AreEqual(2.5f, MathExtensions.Min(3.5f, 2.5f));
			Assert.AreEqual(-3.5f, MathExtensions.Min(-3.5f, -1.5f));
			Assert.AreEqual(4, MathExtensions.Min(4, 5));
		}

		[Test]
		public void NearestMultiple()
		{
			Assert.AreEqual(640, 640.GetNearestMultiple(8));
			Assert.AreEqual(648, 650.GetNearestMultiple(8));
			Assert.AreEqual(1235, 1267.GetNearestMultiple(65));
		}

		[Test]
		public void InvSqrt()
		{
			Assert.AreEqual(0.5f, 4.0f.InvSqrt());
			Assert.AreEqual(0.1f, 100.0f.InvSqrt());
			Assert.AreEqual(10.0f, 0.01f.InvSqrt());
		}

		[Test]
		public void WrapRotationToMinus180ToPlus180()
		{
			Assert.AreEqual(0, MathExtensions.WrapRotationToMinus180ToPlus180(0));
			Assert.AreEqual(90, MathExtensions.WrapRotationToMinus180ToPlus180(90));
			Assert.AreEqual(180, MathExtensions.WrapRotationToMinus180ToPlus180(180));
			Assert.AreEqual(-90, MathExtensions.WrapRotationToMinus180ToPlus180(270));
			Assert.AreEqual(0, MathExtensions.WrapRotationToMinus180ToPlus180(720));
		}

		[Test]
		public void LineToLineIntersection()
		{
			Assert.IsTrue(MathExtensions.IsLineIntersectingWith(-Vector2D.UnitX, Vector2D.UnitX,
				-Vector2D.UnitY, Vector2D.UnitY));
			Assert.IsFalse(MathExtensions.IsLineIntersectingWith(-Vector2D.UnitX, Vector2D.UnitX,
				new Vector2D(-1, 1), new Vector2D(1, 1)));
		}
	}
}