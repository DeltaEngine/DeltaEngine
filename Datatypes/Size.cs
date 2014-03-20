using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Holds the width and height of an object (e.g. a rectangle).
	/// </summary>
	[DebuggerDisplay("Size({Width}, {Height})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct Size : IEquatable<Size>, Lerp<Size>
	{
		public Size(float widthAndHeight)
			: this(widthAndHeight, widthAndHeight) {}

		public Size(float width, float height)
			: this()
		{
			Width = width;
			Height = height;
		}

		public float Width { get; set; }
		public float Height { get; set; }

		public Size(string sizeAsString)
			: this()
		{
			float[] components = sizeAsString.SplitIntoFloats();
			if (components.Length != 2)
				throw new InvalidNumberOfComponents();
			Width = components[0];
			Height = components[1];
		}

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Size Zero;
		public static readonly Size One = new Size(1, 1);
		public static readonly Size Half = new Size(0.5f, 0.5f);
		public static readonly Size Unused = new Size(-1, -1);
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Size));

		[Pure]
		public Size Lerp(Size other, float interpolation)
		{
			return new Size(Width.Lerp(other.Width, interpolation),
				Height.Lerp(other.Height, interpolation));
		}

		public float AspectRatio
		{
			get { return Width / Height; }
		}

		public float Length
		{
			get { return (float)Math.Sqrt(Width * Width + Height * Height); }
		}

		public static bool operator ==(Size s1, Size s2)
		{
			return s1.Width == s2.Width && s1.Height == s2.Height;
		}

		public static bool operator !=(Size s1, Size s2)
		{
			return s1.Width != s2.Width || s1.Height != s2.Height;
		}

		public static Size operator *(Size s1, Size s2)
		{
			return new Size(s1.Width * s2.Width, s1.Height * s2.Height);
		}

		public static Size operator *(Size s, float f)
		{
			return new Size(s.Width * f, s.Height * f);
		}

		public static Size operator *(float f, Size s)
		{
			return new Size(f * s.Width, f * s.Height);
		}

		public static Size operator /(Size s, float f)
		{
			return new Size(s.Width / f, s.Height / f);
		}

		public static Size operator /(float f, Size s)
		{
			return new Size(f / s.Width, f / s.Height);
		}

		public static Size operator /(Size s1, Size s2)
		{
			return new Size(s1.Width / s2.Width, s1.Height / s2.Height);
		}

		public static Size operator +(Size s1, Size s2)
		{
			return new Size(s1.Width + s2.Width, s1.Height + s2.Height);
		}

		public static Size operator -(Size s1, Size s2)
		{
			return new Size(s1.Width - s2.Width, s1.Height - s2.Height);
		}

		[Pure]
		public bool Equals(Size other)
		{
			return Width == other.Width && Height == other.Height;
		}

		[Pure]
		public override bool Equals(object other)
		{
			return other is Size ? Equals((Size)other) : base.Equals(other);
		}

		[Pure]
		public bool IsNearlyEqual(Size other)
		{
			return Width.IsNearlyEqual(other.Width) && Height.IsNearlyEqual(other.Height);
		}

		public static explicit operator Size(Vector2D vector2D)
		{
			return new Size(vector2D.X, vector2D.Y);
		}

		[Pure]
		public override int GetHashCode()
		{
			return Width.GetHashCode() ^ Height.GetHashCode();
		}

		[Pure]
		public override string ToString()
		{
			return Width.ToInvariantString() + ", " + Height.ToInvariantString();
		}
	}
}