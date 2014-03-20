using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Specifies a position in 3D space, used for 3D geometry, cameras and 3D physics.
	/// </summary>
	[DebuggerDisplay("Vector3D({X}, {Y}, {Z})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct Vector3D : IEquatable<Vector3D>, Lerp<Vector3D>
	{
		public Vector3D(float setX, float setY, float setZ)
			: this()
		{
			X = setX;
			Y = setY;
			Z = setZ;
		}

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public Vector3D(Vector2D setFromVector2D, float setZ = 0.0f)
			: this()
		{
			X = setFromVector2D.X;
			Y = setFromVector2D.Y;
			Z = setZ;
		}

		public Vector3D(string vectorAsString)
			: this()
		{
			var floats = vectorAsString.SplitIntoFloats();
			if (floats.Length != 3)
				throw new InvalidNumberOfComponents();
			X = floats[0];
			Y = floats[1];
			Z = floats[2];
		}

		public class InvalidNumberOfComponents : Exception {}

		public static readonly Vector3D Zero;
		public static readonly Vector3D One = new Vector3D(1, 1, 1);
		public static readonly Vector3D UnitX = new Vector3D(1, 0, 0);
		public static readonly Vector3D UnitY = new Vector3D(0, 1, 0);
		public static readonly Vector3D UnitZ = new Vector3D(0, 0, 1);
		public static readonly Vector3D MinusOne = new Vector3D(-1, -1, -1);
		public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3D));

		public static float Dot(Vector3D vector1, Vector3D vector2)
		{
			return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
		}

		public static Vector3D Cross(Vector3D vector1, Vector3D vector2)
		{
			return new Vector3D(vector1.Y * vector2.Z - vector1.Z * vector2.Y,
				vector1.Z * vector2.X - vector1.X * vector2.Z,
				vector1.X * vector2.Y - vector1.Y * vector2.X);
		}

		public static Vector3D Normalize(Vector3D vector)
		{
			float distanceSquared = vector.LengthSquared;
			if (distanceSquared == 0.0f)
				return vector;

			float distanceInverse = 1.0f / MathExtensions.Sqrt(distanceSquared);
			return new Vector3D(vector.X * distanceInverse, vector.Y * distanceInverse,
				vector.Z * distanceInverse);
		}

		public void Normalize()
		{
			if (LengthSquared == 0.0f || LengthSquared == 1.0f)
				return;
			float distanceInverse = 1.0f / MathExtensions.Sqrt(LengthSquared);
			X *= distanceInverse;
			Y *= distanceInverse;
			Z *= distanceInverse;
		}

		public Vector3D TransformNormal(Matrix matrix)
		{
			return matrix.TransformNormal(this);
		}

		[Pure]
		public Vector3D Lerp(Vector3D other, float interpolation)
		{
			return new Vector3D(X.Lerp(other.X, interpolation), Y.Lerp(other.Y, interpolation),
				Z.Lerp(other.Z, interpolation));
		}

		public static float AngleBetweenVectors(Vector3D vector1, Vector3D vector2)
		{
			float dotProduct = Dot(vector1, vector2);
			var cosine = dotProduct / (vector1.Length * vector2.Length);
			return (float)(Math.Acos(cosine) * 180.0 / Math.PI);
		}

		public float Length
		{
			get { return MathExtensions.Sqrt(X * X + Y * Y + Z * Z); }
		}

		public float LengthSquared
		{
			get { return X * X + Y * Y + Z * Z; }
		}

		public static Vector3D operator +(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
		}

		public static Vector3D operator -(Vector3D v1, Vector3D v2)
		{
			return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
		}

		public static Vector3D operator *(Vector3D v, float f)
		{
			return new Vector3D(v.X * f, v.Y * f, v.Z * f);
		}

		public static Vector3D operator *(float f, Vector3D v)
		{
			return new Vector3D(v.X * f, v.Y * f, v.Z * f);
		}

		public static Vector3D operator *(Vector3D left, Vector3D right)
		{
			return new Vector3D(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
		}

		public static Vector3D operator /(Vector3D v, float f)
		{
			return new Vector3D(v.X / f, v.Y / f, v.Z / f);
		}

		public static void Divide(ref Vector3D v, float f, ref Vector3D result)
		{
			float inverse = 1.0f / f;
			result.X = v.X * inverse;
			result.Y = v.Y * inverse;
			result.Z = v.Z * inverse;
		}

		public static Vector3D operator -(Vector3D value)
		{
			return new Vector3D(-value.X, -value.Y, -value.Z);
		}

		public static bool operator !=(Vector3D v1, Vector3D v2)
		{
			return v1.X != v2.X || v1.Y != v2.Y || v1.Z != v2.Z;
		}

		public static bool operator ==(Vector3D v1, Vector3D v2)
		{
			return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
		}

		[Pure]
		public bool Equals(Vector3D other)
		{
			return X == other.X && Y == other.Y && Z == other.Z;
		}

		[Pure]
		public override bool Equals(object other)
		{
			return other is Vector3D ? Equals((Vector3D)other) : base.Equals(other);
		}

		[Pure]
		public bool IsNearlyEqual(Vector3D other)
		{
			return X.IsNearlyEqual(other.X) && Y.IsNearlyEqual(other.Y) && Z.IsNearlyEqual(other.Z);
		}

		public static implicit operator Vector3D(Vector2D vector2D)
		{
			return new Vector3D(vector2D.X, vector2D.Y, 0);
		}

		public override int GetHashCode()
		{
			return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
		}

		public override string ToString()
		{
			return X.ToInvariantString() + ", " + Y.ToInvariantString() + ", " +
				Z.ToInvariantString();
		}

		public Vector2D GetVector2D()
		{
			return new Vector2D(X, Y);
		}

		public float Distance(Vector3D vector)
		{
			return (vector - this).Length;
		}

		public float DistanceSquared(Vector3D vector)
		{
			return (vector - this).LengthSquared;
		}

		public Vector3D TransformTranspose(Matrix matrix)
		{
			return new Vector3D( 
				X * matrix[0] + Y * matrix[1] + Z * matrix[2],
				X * matrix[4] + Y * matrix[5] + Z * matrix[6],
				X * matrix[8] + Y * matrix[9] + Z * matrix[10]);
		}

		public Vector3D RotateAround(Vector3D axis, float angle)
		{
			var quaternion = Quaternion.FromAxisAngle(axis, 0);
			quaternion.Conjugate();
			var worldSpaceVector = Transform(quaternion);
			quaternion = Quaternion.FromAxisAngle(axis, angle);
			Transform(ref worldSpaceVector, ref quaternion, out worldSpaceVector);
			return worldSpaceVector;
		}

		public Vector3D Transform(Quaternion quaternion)
		{
			float x2 = quaternion.X + quaternion.X;
			float y2 = quaternion.Y + quaternion.Y;
			float z2 = quaternion.Z + quaternion.Z;
			float xx2 = quaternion.X * x2;
			float xy2 = quaternion.X * y2;
			float xz2 = quaternion.X * z2;
			float yy2 = quaternion.Y * y2;
			float yz2 = quaternion.Y * z2;
			float zz2 = quaternion.Z * z2;
			float wx2 = quaternion.W * x2;
			float wy2 = quaternion.W * y2;
			float wz2 = quaternion.W * z2;
			return new Vector3D(
				X * (1.0f - yy2 - zz2) + Y * (xy2 - wz2) + Z * (xz2 + wy2),
				X * (xy2 + wz2) + Y * (1.0f - xx2 - zz2) + Z * (yz2 - wx2),
				X * (xz2 - wy2) + Y * (yz2 + wx2) + Z * (1.0f - xx2 - yy2));
		}

		public static void Transform(ref Vector3D vector, ref Quaternion quaternion,
			out Vector3D result)
		{
			float x2 = quaternion.X + quaternion.X;
			float y2 = quaternion.Y + quaternion.Y;
			float z2 = quaternion.Z + quaternion.Z;
			float xx2 = quaternion.X * x2;
			float xy2 = quaternion.X * y2;
			float xz2 = quaternion.X * z2;
			float yy2 = quaternion.Y * y2;
			float yz2 = quaternion.Y * z2;
			float zz2 = quaternion.Z * z2;
			float wx2 = quaternion.W * x2;
			float wy2 = quaternion.W * y2;
			float wz2 = quaternion.W * z2;
			result = new Vector3D(
				vector.X * ((1.0f - yy2) - zz2) + vector.Y * (xy2 - wz2) + vector.Z * (xz2 + wy2),
				vector.X * (xy2 + wz2) + vector.Y * ((1.0f - xx2) - zz2) + vector.Z * (yz2 - wx2),
				vector.X * (xz2 - wy2) + vector.Y * (yz2 + wx2) + vector.Z * ((1.0f - xx2) - yy2));
		}

		public Vector3D Reflect(Vector3D planeNormal)
		{
			return this - (planeNormal * Dot(this, planeNormal) * 2);
		}

		public Vector3D IntersectNormal(Vector3D normal)
		{
			return normal * Dot(this, normal);
		}

		public Vector3D IntersectRay(Vector3D rayOrigin, Vector3D rayDirection)
		{
			return rayDirection * Dot((this - rayOrigin), rayDirection) + rayOrigin;
		}

		public Vector3D IntersectPlane(Vector3D planeNormal)
		{
			return this - planeNormal * Dot(this, planeNormal);
		}

		public float Angle(Vector3D vector)
		{
			if (this == vector)
				return 0;
			var vec = Normalize(this);
			float val = Dot(vec, Normalize(vector));
			val = (val > 1) ? 1 : val;
			val = (val < -1) ? -1 : val;
			return (float)(Math.Acos(val) * 180.0f / MathExtensions.Pi);
		}

		public static Vector3D Hermite(Vector3D value1, Vector3D tangent1, Vector3D value2,
			Vector3D tangent2, float interpolationAmount)
		{
			float weightSquared = interpolationAmount * interpolationAmount;
			float weightCubed = interpolationAmount * weightSquared;
			float blend1 = 2 * weightCubed - 3 * weightSquared + 1;
			float tangent1Blend = weightCubed - 2 * weightSquared + interpolationAmount;
			float blend2 = -2 * weightCubed + 3 * weightSquared;
			float tangent2Blend = weightCubed - weightSquared;
			return new Vector3D(
				value1.X * blend1 + value2.X * blend2 + tangent1.X * tangent1Blend +
				tangent2.X * tangent2Blend,
				value1.Y * blend1 + value2.Y * blend2 + tangent1.Y * tangent1Blend +
				tangent2.Y * tangent2Blend,
				value1.Z * blend1 + value2.Z * blend2 + tangent1.Z * tangent1Blend +
				tangent2.Z * tangent2Blend);
		}
	}
}