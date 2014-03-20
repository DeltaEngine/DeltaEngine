using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Represents a 2D vector, which is useful for screen positions (sprites, mouse, touch, etc.)
	/// </summary>
	[DebuggerDisplay("Vector2D({X}, {Y})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector2D : IEquatable<Vector2D>, Lerp<Vector2D>
	{
		public Vector2D(float x, float y)
			: this()
		{
			X = x;
			Y = y;
		}

		public Vector2D(string vectorAsString)
			: this()
		{
			float[] components = vectorAsString.SplitIntoFloats();
			if (components.Length != 2)
				throw new InvalidNumberOfComponents();
			X = components[0];
			Y = components[1];
		}

		public float X { get; set; }
		public float Y { get; set; }

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Vector2D Zero;
		public static readonly Vector2D One = new Vector2D(1, 1);
		public static readonly Vector2D Half = new Vector2D(0.5f, 0.5f);
		public static readonly Vector2D UnitX = new Vector2D(1, 0);
		public static readonly Vector2D UnitY = new Vector2D(0, 1);
		public static readonly Vector2D Unused = new Vector2D(-1, -1);
		public static readonly Vector2D ScreenLeft = new Vector2D(-1, 0);
		public static readonly Vector2D ScreenRight = new Vector2D(1, 0);
		public static readonly Vector2D ScreenUp = new Vector2D(0, -1);
		public static readonly Vector2D ScreenDown = new Vector2D(0, 1);
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2D));

		public static Vector2D operator +(Vector2D vector1, Vector2D vector2)
		{
			return new Vector2D(vector1.X + vector2.X, vector1.Y + vector2.Y);
		}

		public static Vector2D operator -(Vector2D vector1, Vector2D vector2)
		{
			return new Vector2D(vector1.X - vector2.X, vector1.Y - vector2.Y);
		}

		public static Vector2D operator *(float f, Vector2D vector)
		{
			return new Vector2D(vector.X * f, vector.Y * f);
		}

		public static Vector2D operator *(Vector2D vector, float f)
		{
			return new Vector2D(vector.X * f, vector.Y * f);
		}

		public static Vector2D operator *(Vector2D vector, Size scale)
		{
			return new Vector2D(vector.X * scale.Width, vector.Y * scale.Height);
		}

		public static Vector2D operator /(Vector2D vector, float f)
		{
			return new Vector2D(vector.X / f, vector.Y / f);
		}

		public static Vector2D operator /(Vector2D vector, Size scale)
		{
			return new Vector2D(vector.X / scale.Width, vector.Y / scale.Height);
		}

		public static Vector2D operator -(Vector2D vector)
		{
			return new Vector2D(-vector.X, -vector.Y);
		}

		public static bool operator !=(Vector2D vector1, Vector2D vector2)
		{
			return vector1.X != vector2.X || vector1.Y != vector2.Y;
		}

		public static bool operator ==(Vector2D vector1, Vector2D vector2)
		{
			return vector1.X == vector2.X && vector1.Y == vector2.Y;
		}

		[Pure]
		public bool Equals(Vector2D other)
		{
			return X == other.X && Y == other.Y;
		}

		[Pure]
		public override bool Equals(object other)
		{
			return other is Vector2D ? Equals((Vector2D)other) : base.Equals(other);
		}

		[Pure]
		public bool IsNearlyEqual(Vector2D other)
		{
			return X.IsNearlyEqual(other.X) && Y.IsNearlyEqual(other.Y);
		}

		public static implicit operator Vector2D(Size size)
		{
			return new Vector2D(size.Width, size.Height);
		}

		[Pure]
		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode();
		}

		[Pure]
		public override string ToString()
		{
			return X.ToInvariantString() + ", " + Y.ToInvariantString();
		}

		[Pure]
		public float Length
		{
			get { return (float)Math.Sqrt(X * X + Y * Y); }
		}

		[Pure]
		public float LengthSquared
		{
			get { return X * X + Y * Y; }
		}

		[Pure]
		public float GetRotation()
		{
			var normalized = Normalize(this);
			return MathExtensions.Atan2(normalized.Y, normalized.X);
		}

		[Pure]
		public float DistanceTo(Vector2D other)
		{
			float distanceX = X - other.X;
			float distanceY = Y - other.Y;
			return (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
		}

		[Pure]
		public float DistanceToSquared(Vector2D other)
		{
			float distanceX = X - other.X;
			float distanceY = Y - other.Y;
			return distanceX * distanceX + distanceY * distanceY;
		}

		[Pure]
		public Vector2D DirectionTo(Vector2D other)
		{
			return other - this;
		}

		public Vector2D ReflectIfHittingBorder(Rectangle box, Rectangle borders)
		{
			if (box.Width >= borders.Width || box.Height >= borders.Height)
				return this;
			if (box.Left <= borders.Left)
				X = X.Abs();
			if (box.Right >= borders.Right)
				X = -X.Abs();
			if (box.Top <= borders.Top)
				Y = Y.Abs();
			if (box.Bottom >= borders.Bottom)
				Y = -Y.Abs();
			return this;
		}

		[Pure]
		public Vector2D Lerp(Vector2D other, float interpolation)
		{
			return new Vector2D(X.Lerp(other.X, interpolation), Y.Lerp(other.Y, interpolation));
		}

		[Pure]
		public Vector2D Rotate(float angleInDegrees)
		{
			return RotateAround(Zero, angleInDegrees);
		}

		[Pure]
		public Vector2D RotateAround(Vector2D center, float angleInDegrees)
		{
			return RotateAround(center, MathExtensions.Sin(angleInDegrees),
				MathExtensions.Cos(angleInDegrees));
		}

		[Pure]
		public Vector2D RotateAround(Vector2D center, float rotationSin, float rotationCos)
		{
			var translatedPoint = this - center;
			return
				new Vector2D(center.X + translatedPoint.X * rotationCos - translatedPoint.Y * rotationSin,
					center.Y + translatedPoint.X * rotationSin + translatedPoint.Y * rotationCos);
		}

		[Pure]
		public float RotationTo(Vector2D target)
		{
			var normal = Normalize(this - target);
			return MathExtensions.Atan2(normal.Y, normal.X);
		}

		public static Vector2D Normalize(Vector2D vector)
		{
			var length = (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
			vector.X /= length;
			vector.Y /= length;
			return vector;
		}

		[Pure]
		public float DotProduct(Vector2D vector)
		{
			return X * vector.X + Y * vector.Y;
		}

		[Pure]
		public float DistanceFromProjectAxisPointX(Vector2D axis)
		{
			return (X * axis.X + Y * axis.Y) / (axis.X * axis.X + axis.Y * axis.Y) * axis.X;
		}

		[Pure]
		public float DistanceFromProjectAxisPointY(Vector2D axis)
		{
			return (X * axis.X + Y * axis.Y) / (axis.X * axis.X + axis.Y * axis.Y) * axis.Y;
		}

		/// <summary>
		/// http://en.wikipedia.org/wiki/Distance_from_a_point_to_a_line
		/// </summary>
		[Pure]
		public float DistanceToLine(Vector2D lineStart, Vector2D lineEnd)
		{
			var lineDirection = lineEnd - lineStart;
			var lineLengthSquared = lineDirection.LengthSquared;
			if (lineLengthSquared == 0.0)
				return DistanceTo(lineStart);
			var startDirection = this - lineStart;
			var linePosition = startDirection.DotProduct(lineDirection) / lineLengthSquared;
			var projection = lineStart + linePosition * lineDirection;
			return DistanceTo(projection);
		}

		[Pure]
		public float DistanceToLineSquared(Vector2D lineStart, Vector2D lineEnd)
		{
			var lineDirection = lineEnd - lineStart;
			var lineLengthSquared = lineDirection.LengthSquared;
			if (lineLengthSquared == 0.0)
				return DistanceToSquared(lineStart);
			var startDirection = this - lineStart;
			var linePosition = startDirection.DotProduct(lineDirection) / lineLengthSquared;
			var projection = lineStart + linePosition * lineDirection;
			return DistanceToSquared(projection);
		}

		/// <summary>
		/// http://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
		/// </summary>
		[Pure]
		public float DistanceToLineSegment(Vector2D lineStart, Vector2D lineEnd)
		{
			var lineDirection = lineEnd - lineStart;
			var lineLengthSquared = lineDirection.LengthSquared;
			if (lineLengthSquared == 0.0)
				return DistanceTo(lineStart);
			var startDirection = this - lineStart;
			var linePosition = startDirection.DotProduct(lineDirection) / lineLengthSquared;
			if (linePosition < 0.0)
				return DistanceTo(lineStart);
			if (linePosition > 1.0)
				return DistanceTo(lineEnd);
			var projection = lineStart + linePosition * lineDirection;
			return DistanceTo(projection);
		}

		/// <summary>
		/// http://stackoverflow.com/questions/3461453/determine-which-side-of-a-line-a-point-lies
		/// </summary>
		[Pure]
		public bool IsLeftOfLineOrOnIt(Vector2D lineStart, Vector2D lineEnd)
		{
			return ((lineEnd.X - lineStart.X) * (Y - lineStart.Y) -
				(lineEnd.Y - lineStart.Y) * (X - lineStart.X)) >= 0;
		}

		[Pure]
		public float AngleBetweenVector(Vector2D vector)
		{
			var dot = DotProduct(vector);
			var test = dot / (Length * vector.Length);
			return MathExtensions.Acos(test);
		}
	}
}