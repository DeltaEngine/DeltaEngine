using System;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Extensions
{
	/// <summary>
	/// Extends the System.Math class, but uses floats and provides some extra constants.
	/// </summary>
	public static class MathExtensions
	{
		public static float Abs(this float value)
		{
			return Math.Abs(value);
		}

		public static bool IsNearlyEqual(this float value1, float value2, float difference = Epsilon)
		{
			return (value1 - value2).Abs() < difference;
		}

		public const float Epsilon = 0.0001f;

		public static int Round(this float value)
		{
			return (int)Math.Round(value);
		}

		public static float Round(this float value, int decimals)
		{
			return (float)Math.Round(value, decimals);
		}

		public static float Sin(float degrees)
		{
			return (float)Math.Sin(degrees * Pi / 180.0f);
		}

		public const float Pi = 3.14159265359f;

		public static float Cos(float degrees)
		{
			return (float)Math.Cos(degrees * Pi / 180.0f);
		}

		public static float Tan(float degrees)
		{
			return (float)Math.Tan(degrees * Pi / 180.0f);
		}

		public static float Asin(float value)
		{
			return (float)Math.Asin(value) * 180 / Pi;
		}

		public static float Acos(float value)
		{
			return (float)Math.Acos(value) * 180 / Pi;
		}

		public static float Atan2(float y, float x)
		{
			return (float)Math.Atan2(y, x) * 180 / Pi;
		}

		public static float Sqrt(float value)
		{
			return (float)Math.Sqrt(value);
		}

		public static int Clamp(this int value, int min, int max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		public static float Clamp(this float value, float min, float max)
		{
			return value > max ? max : (value < min ? min : value);
		}

		public static float Lerp(this float value1, float value2, float percentage)
		{
			return value1 * (1 - percentage) + value2 * percentage;
		}

		public static float RadiansToDegrees(this float radians)
		{
			return (radians * 180.0f) / Pi;
		}

		public static float DegreesToRadians(this float degrees)
		{
			return (degrees * Pi) / 180.0f;
		}

		public static float Max(float value1, float value2)
		{
			return value1 > value2 ? value1 : value2;
		}

		public static int Max(int value1, int value2)
		{
			return value1 > value2 ? value1 : value2;
		}

		public static float Min(float value1, float value2)
		{
			return value1 < value2 ? value1 : value2;
		}

		public static int Min(int value1, int value2)
		{
			return value1 < value2 ? value1 : value2;
		}

		public static int GetNearestMultiple(this int value, int multipleValue)
		{
			int min = ((int)(value / (float)multipleValue)) * multipleValue;
			int max = ((int)(value / (float)multipleValue) + 1) * multipleValue;

			return max - value < value - min ? max : min;
		}

		public static float InvSqrt(this float value)
		{
			return 1.0f / Sqrt(value);
		}

		public static float WrapRotationToMinus180ToPlus180(float degrees)
		{
			degrees = (float)Math.IEEERemainder(degrees, 360);
			return degrees <= -180 ? degrees + 360 : (degrees > 180 ? degrees - 360 : degrees);
		}

		/// <summary>
		/// http://en.wikipedia.org/wiki/Line%E2%80%93line_intersection
		/// </summary>
		public static bool IsLineIntersectingWith(Vector2D line1Start, Vector2D line1End,
			Vector2D line2Start, Vector2D line2End)
		{
			float denominator = (line2End.Y - line2Start.Y) * (line1End.X - line1Start.X) -
													((line2End.X - line2Start.X) * (line1End.Y - line1Start.Y));
			float ua = ((line2End.X - line2Start.X) * (line1Start.Y - line2Start.Y) -
									(line2End.Y - line2Start.Y) * (line1Start.X - line2Start.X)) / denominator;
			float ub = ((line1End.X - line1Start.X) * (line1Start.Y - line2Start.Y) -
									(line1End.Y - line1Start.Y) * (line1Start.X - line2Start.X)) / denominator;
			return ua >= 0f && ua <= 1f && ub >= 0f && ub <= 1f;
		}
	}
}