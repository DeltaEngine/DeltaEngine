using DeltaEngine.Datatypes;
using Jitter.LinearMath;

namespace DeltaEngine.Physics3D.Jitter
{
	/// <summary>
	/// Implements mapping between Jitter data types and Delta engine data types.
	/// </summary>
	internal static class JitterDatatypesMapping
	{
		public static JMatrix Convert(ref Matrix matrix)
		{
			JMatrix result;
			result.M11 = matrix[0];
			result.M12 = matrix[1];
			result.M13 = matrix[2];
			result.M21 = matrix[4];
			result.M22 = matrix[5];
			result.M23 = matrix[6];
			result.M31 = matrix[8];
			result.M32 = matrix[9];
			result.M33 = matrix[10];
			return result;
		}

		public static void Convert(JMatrix matrix, ref Matrix result)
		{
			result[0] = matrix.M11;
			result[1] = matrix.M12;
			result[2] = matrix.M13;
			result[4] = matrix.M21;
			result[5] = matrix.M22;
			result[6] = matrix.M23;
			result[8] = matrix.M31;
			result[9] = matrix.M32;
			result[10] = matrix.M33;
		}

		public static JVector Convert(ref Vector3D vector)
		{
			JVector result;
			result.X = vector.X;
			result.Y = vector.Y;
			result.Z = vector.Z;
			return result;
		}

		public static void Convert(ref Vector3D vector, out JVector result)
		{
			result.X = vector.X;
			result.Y = vector.Y;
			result.Z = vector.Z;
		}

		public static JVector ConvertSlow(Vector3D vector)
		{
			JVector result;
			result.X = vector.X;
			result.Y = vector.Y;
			result.Z = vector.Z;
			return result;
		}

		public static Vector3D Convert(JVector vector)
		{
			return new Vector3D(vector.X, vector.Y, vector.Z);
		}

		public static void Convert(ref JVector vector, ref Vector3D result)
		{
			result.X = vector.X;
			result.Y = vector.Y;
			result.Z = vector.Z;
		}
	}
}