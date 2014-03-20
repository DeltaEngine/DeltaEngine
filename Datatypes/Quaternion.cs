using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using DeltaEngine.Extensions;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Useful for processing 3D rotations. Riemer's XNA tutorials have a nice introductory 
	/// example of use: http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Quaternions.php
	/// http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series2/Flight_kinematics.php
	/// </summary>
	[DebuggerDisplay("Quaternion(X={X}, Y={Y}, Z={Z}, W={W})")]
	[StructLayout(LayoutKind.Sequential)]
	public struct Quaternion : IEquatable<Quaternion>, Lerp<Quaternion>
	{
		public Quaternion(float x, float y, float z, float w)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Quaternion(string quaternionAsString)
			: this()
		{
			var partitions = quaternionAsString.SplitAndTrim(new[] { ',', ' ' });
			if (partitions.Length != 4)
				throw new InvalidNumberOfComponents();
			try
			{
				X = float.Parse(partitions[0]);
				Y = float.Parse(partitions[1]);
				Z = float.Parse(partitions[2]);
				W = float.Parse(partitions[3]);
			}
			catch
			{
				throw new InvalidStringFormat();
			}
		}

		public class InvalidNumberOfComponents : Exception {}

		public class InvalidStringFormat : Exception {}

		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
		public float W { get; set; }

		public static Quaternion FromAxisAngle(Vector3D axis, float angle)
		{
			var vectorPart = MathExtensions.Sin(angle * 0.5f) * axis;
			return new Quaternion(vectorPart.X, vectorPart.Y, vectorPart.Z,
				MathExtensions.Cos(angle * 0.5f));
		}

		public static Quaternion Normalize(Quaternion q)
		{
			return q * (1.0f / q.Length);
		}

		[Pure]
		public float Length
		{
			get { return MathExtensions.Sqrt(X * X + Y * Y + Z * Z + W * W); }
		}

		[Pure]
		public static Quaternion operator *(Quaternion q, float f)
		{
			return new Quaternion(q.X * f, q.Y * f, q.Z * f, q.W * f);
		}

		[Pure]
		public static Quaternion operator +(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q1.X + q2.X, q1.Y + q2.Y, q1.Z + q2.Z, q1.W + q2.W);
		}

		// From 
		// http://molecularmusings.wordpress.com/2013/05/24/a-faster-quaternion-vector-multiplication/
		public static Vector3D operator *(Quaternion q, Vector3D v)
		{
			var qv = q.Vector3D;
			Vector3D t = 2.0f * Vector3D.Cross(qv, v);
			return v + q.W * t + Vector3D.Cross(qv, t);
		}

		[Pure]
		public Vector3D Vector3D
		{
			get { return new Vector3D(X, Y, Z); }
		}

		// Adapted from 
		// http://www.euclideanspace.com/maths/algebra/realNormedAlgebra/quaternions/code/
		public static Quaternion operator *(Quaternion q1, Quaternion q2)
		{
			return new Quaternion(q2.X * q1.W + q2.Y * q1.Z - q2.Z * q1.Y + q2.W * q1.X,
				-q2.X * q1.Z + q2.Y * q1.W + q2.Z * q1.X + q2.W * q1.Y,
				q2.X * q1.Y - q2.Y * q1.X + q2.Z * q1.W + q2.W * q1.Z,
				-q2.X * q1.X - q2.Y * q1.Y - q2.Z * q1.Z + q2.W * q1.W);
		}

		public Quaternion Conjugate()
		{
			return new Quaternion(-X, -Y, -Z, W);
		}

		public static Quaternion CreateLookAt(Vector3D eyePosition, Vector3D targetPosition,
			Vector3D cameraUp)
		{
			Matrix matLookAt = Matrix.CreateLookAt(eyePosition, targetPosition, cameraUp);
			return FromRotationMatrix(matLookAt);
		}

		public static Quaternion FromRotationMatrix(Matrix m)
		{
			float t = 1.0f;
			float s = 0.5f;
			if (m[0] + m[5] + m[10] > 0.0f)
			{
				t += m[0] + m[5] + m[10];
				s *= t.InvSqrt();
				return new Quaternion(m[6] - m[9], m[8] - m[2], m[1] - m[4], t) * s;
			}
			if (m[0] > m[5] && m[0] > m[10])
			{
				t += m[0] - m[5] - m[10];
				s *= t.InvSqrt();
				return new Quaternion(t, m[1] + m[4], m[8] + m[2], m[6] - m[9]) * s;
			}
			if (m[5] > m[10])
			{
				t += - m[0] + m[5] - m[10];
				s *= t.InvSqrt();
				return new Quaternion(m[1] + m[4], t, m[6] + m[9], m[8] - m[2]) * s;
			}
			t += - m[0] - m[5] + m[10];
			s *= t.InvSqrt();
			return new Quaternion(m[8] + m[2], m[6] + m[9], t, m[1] - m[4]) * s;
		}

		[Pure]
		public Quaternion Lerp(Quaternion other, float interpolation)
		{
			return new Quaternion(X.Lerp(other.X, interpolation), Y.Lerp(other.Y, interpolation),
				Z.Lerp(other.Z, interpolation), W.Lerp(other.W, interpolation));
		}

		public Quaternion Slerp(Quaternion other, float interpolation)
		{
			var cos = Dot(other);
			var sin = MathExtensions.Sqrt((1.0f - cos * cos).Abs());
			var angle = MathExtensions.Atan2(sin, cos);
			var reciprocalSin = 1.0f / sin;
			var coefficient0 = MathExtensions.Sin((1 - interpolation) * angle) * reciprocalSin;
			var coefficient1 = MathExtensions.Sin(interpolation * angle) * reciprocalSin;
			return (this * coefficient0) + (other * coefficient1);
		}

		public float Dot(Quaternion other)
		{
			return (X * other.X) + (Y * other.Y) + (Z * other.Z) + (W * other.W);
		}

		public bool Equals(Quaternion other)
		{
			return X.IsNearlyEqual(other.X) && Y.IsNearlyEqual(other.Y) && Z.IsNearlyEqual(other.Z) &&
				W.IsNearlyEqual(other.W);
		}

		public static readonly Quaternion Identity = new Quaternion(0.0f, 0.0f, 0.0f, 1.0f);

		public override string ToString()
		{
			return X.ToInvariantString() + ", " + Y.ToInvariantString() + ", " + Z.ToInvariantString() +
				", " + W.ToInvariantString();
		}

		// Derived from: http://stackoverflow.com/questions/1031005/is-there-an-algorithm-for-converting-quaternion-rotations-to-euler-angle-rotatio/2070899#2070899
		// Returns Euler angles applied in ZYX order to match Matrix.CreateRotationZYX.
		[Pure]
		public EulerAngles ToEuler()
		{
			float ww = W * W;
			float xx = X * X;
			float yy = Y * Y;
			float zz = Z * Z;
			float lengthSqd = xx + yy + zz + ww;
			float singularityTest = Y * W - X * Z;
			float singularityValue = Singularity * lengthSqd;
			return singularityTest > singularityValue
				? new EulerAngles(-2 * MathExtensions.Atan2(Z, W), 90.0f, 0.0f)
				: singularityTest < -singularityValue
					? new EulerAngles(2 * MathExtensions.Atan2(Z, W), -90.0f, 0.0f)
					: new EulerAngles(MathExtensions.Atan2(2.0f * (Y * Z + X * W), 1.0f - 2.0f * (xx + yy)),
						MathExtensions.Asin(2.0f * singularityTest / lengthSqd),
						MathExtensions.Atan2(2.0f * (X * Y + Z * W), 1.0f - 2.0f * (yy + zz)));
		}

		private const float Singularity = 0.499f;

		public float CalculateAxisAngle()
		{
			var angle = (2.0f * (float)Math.Acos(W.Clamp(-1.0f, 1.0f))).RadiansToDegrees();
			return angle % 360;
		}

		public Vector3D CalculateRotationAxis()
		{
			return new Vector3D(X / (float)Math.Sqrt(1 - W * W), Y / (float)Math.Sqrt(1 - W * W),
				Z / (float)Math.Sqrt(1 - W * W));
		}
	}
}