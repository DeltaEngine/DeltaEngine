using System;
using System.Runtime.InteropServices;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering2D.Shapes
{
	/// <summary>
	/// Holds the three corners that define a triangle in 2D space.
	/// </summary>
	public struct Triangle2D
	{
		public Triangle2D(Vector2D corner1, Vector2D corner2, Vector2D corner3)
			: this()
		{
			Corner1 = corner1;
			Corner2 = corner2;
			Corner3 = corner3;
		}

		public Vector2D Corner1;
		public Vector2D Corner2;
		public Vector2D Corner3;

		public Triangle2D(string triangle2DAsString)
			: this()
		{
			float[] components = triangle2DAsString.SplitIntoFloats(new[] { ',', '(', ')', ' ' });
			if (components.Length != 6)
				throw new InvalidNumberOfComponents();

			Corner1 = new Vector2D(components[0], components[1]);
			Corner2 = new Vector2D(components[2], components[3]);
			Corner3 = new Vector2D(components[4], components[5]);
		}

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Triangle2D Zero = new Triangle2D();
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Triangle2D));

		public static bool operator ==(Triangle2D triangle1, Triangle2D triangle2)
		{
			return triangle1.Equals(triangle2);
		}

		public static bool operator !=(Triangle2D triangle1, Triangle2D triangle2)
		{
			return !triangle1.Equals(triangle2);
		}

		public override bool Equals(object obj)
		{
			return obj is Triangle2D ? Equals((Triangle2D)obj) : base.Equals(obj);
		}

		public bool Equals(Triangle2D other)
		{
			return Corner1 == other.Corner1 && Corner2 == other.Corner2 && Corner3 == other.Corner3;
		}

		public override int GetHashCode()
		{
			//// ReSharper disable NonReadonlyFieldInGetHashCode
			return Corner1.GetHashCode() ^ Corner2.GetHashCode() ^ Corner3.GetHashCode();
		}

		public override string ToString()
		{
			return Corner1 + " " + Corner2 + " " + Corner3;
		}
	}
}