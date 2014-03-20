using DeltaEngine.Datatypes;

namespace DeltaEngine.Extensions
{
	class LinearMapExtensions
	{
		static public Vector3D TransformVector(Vector3D vector, Matrix matrix)
		{
			float[] mvalues = matrix.GetValues;
			return new Vector3D(
				vector.X * mvalues[0] + vector.Y * mvalues[4] + vector.Z * mvalues[8],
				vector.X * mvalues[1] + vector.Y * mvalues[5] + vector.Z * mvalues[9],
				vector.X * mvalues[2] + vector.Y * mvalues[6] + vector.Z * mvalues[10]);
		}

		public static Vector3D TransformVectorWithHomogeneousCoordinate(Vector3D vector, Matrix matrix)
		{
			float[] mvalues = matrix.GetValues;
			var retVector = new Vector3D(
				vector.X * mvalues[0] + vector.Y * mvalues[4] + vector.Z * mvalues[8] + mvalues[12],
				vector.X * mvalues[1] + vector.Y * mvalues[5] + vector.Z * mvalues[9] + mvalues[13],
				vector.X * mvalues[2] + vector.Y * mvalues[6] + vector.Z * mvalues[10] + mvalues[14]);
			float w = vector.X * mvalues[3] + vector.Y * mvalues[7] + vector.Z * mvalues[11] + mvalues[15];
			return retVector / w;
		}

		public static Matrix CreateMatrixRotatingAboutX(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				1f, 0f, 0f, 0f,
				0f, cosValue, sinValue, 0f,
				0f, -sinValue, cosValue, 0f,
				0f, 0f, 0f, 1f);
		}

		public static Matrix CreateMatrixRotatingAboutY(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				cosValue, 0f, -sinValue, 0f,
				0f, 1f, 0f, 0f,
				sinValue, 0f, cosValue, 0f,
				0f, 0f, 0f, 1f);
		}

		public static Matrix CreateMatrixRotatingAboutZ(float degrees)
		{
			float cosValue = MathExtensions.Cos(degrees);
			float sinValue = MathExtensions.Sin(degrees);
			return new Matrix(
				cosValue, sinValue, 0f, 0f, 
				-sinValue, cosValue, 0f, 0f,
				0f, 0f, 1f, 0f,
				0f, 0f, 0f, 1f);
		}

		public static Matrix CreatePerspectiveMatrix(float fieldOfView, float aspectRatio,
			float nearPlaneDistance, float farPlaneDistance)
		{
			float focalLength = 1.0f / MathExtensions.Tan(fieldOfView * 0.5f);
			float inverseDepth = 1.0f / (farPlaneDistance - nearPlaneDistance);
			return new Matrix(
				focalLength, 0.0f, 0.0f, 0.0f,
				0.0f, focalLength / aspectRatio, 0.0f, 0.0f,
				0.0f, 0.0f, -inverseDepth * (farPlaneDistance + nearPlaneDistance), -1.0f,
				0.0f, 0.0f, -inverseDepth * (2.0f * farPlaneDistance * nearPlaneDistance), 0.0f);
		}

		public static Matrix CreateOrthoProjectionMatrix(Size viewportSize)
		{
			return new Matrix(
				2.0f / viewportSize.Width, 0.0f, 0.0f, 0.0f,
				0.0f, 2.0f / -viewportSize.Height, 0.0f, 0.0f,
				0.0f, 0.0f, -1.0f, 0.0f,
				-1.0f, 1.0f, 0.0f, 1.0f);
		}

		public static Matrix CreateOrthoProjectionMatrix
			(Size viewportSize, float nearPlane, float farPlane)
		{
			var invDepth = 1.0f / (farPlane - nearPlane);
			return new Matrix(
				2.0f / viewportSize.Width, 0.0f, 0.0f, 0.0f,
				0.0f, 2.0f / viewportSize.Height, 0.0f, 0.0f,
				0.0f, 0.0f, -2.0f * invDepth, 0.0f,
				0.0f, 0.0f, -1.0f * (nearPlane + farPlane) * invDepth, 1.0f);
		}

		public static Matrix CreateLookAtMatrix(Vector3D cameraPosition, Vector3D cameraTarget,
			Vector3D cameraUp)
		{
			var forward = Vector3D.Normalize(cameraPosition - cameraTarget);
			var side = Vector3D.Normalize(Vector3D.Cross(Vector3D.Normalize(cameraUp), forward));
			var up = Vector3D.Normalize(Vector3D.Cross(forward, side));
			return new Matrix(
				side.X, up.X, forward.X, 0.0f,
				side.Y, up.Y, forward.Y, 0.0f,
				side.Z, up.Z, forward.Z, 0.0f,
				-Vector3D.Dot(side, cameraPosition),
				-Vector3D.Dot(up, cameraPosition),
				-Vector3D.Dot(forward, cameraPosition), 1.0f);
		}

		public static Matrix CreateRotationAboutZThenYThenX(float x, float y, float z)
		{
			float cx = MathExtensions.Cos(x), sx = MathExtensions.Sin(x);
			float cy = MathExtensions.Cos(y), sy = MathExtensions.Sin(y);
			float cz = MathExtensions.Cos(z), sz = MathExtensions.Sin(z);
			return new Matrix(
				cy * cz, cy * sz, -sy, 0.0f,
				sx * sy * cz + cx * -sz, sx * sy * sz + cx * cz, sx * cy, 0.0f,
				cx * sy * cz + sx * sz, cx * sy * sz + -sx * cz, cx * cy, 0.0f,
				0.0f, 0.0f, 0.0f, 1.0f);
		}

		public static Matrix InvertMatrix(Matrix matrix)
		{
			float[] mvalues = matrix.GetValues;
			var subFactors = new float[19];
			subFactors[0] = mvalues[10] * mvalues[15] - mvalues[14] * mvalues[11];
			subFactors[1] = mvalues[9] * mvalues[15] - mvalues[13] * mvalues[11];
			subFactors[2] = mvalues[9] * mvalues[14] - mvalues[13] * mvalues[10];
			subFactors[3] = mvalues[8] * mvalues[15] - mvalues[12] * mvalues[11];
			subFactors[4] = mvalues[8] * mvalues[14] - mvalues[12] * mvalues[10];
			subFactors[5] = mvalues[8] * mvalues[13] - mvalues[12] * mvalues[9];
			subFactors[6] = mvalues[6] * mvalues[15] - mvalues[14] * mvalues[7];
			subFactors[7] = mvalues[5] * mvalues[15] - mvalues[13] * mvalues[7];
			subFactors[8] = mvalues[5] * mvalues[14] - mvalues[13] * mvalues[6];
			subFactors[9] = mvalues[4] * mvalues[15] - mvalues[12] * mvalues[7];
			subFactors[10] = mvalues[4] * mvalues[14] - mvalues[12] * mvalues[6];
			subFactors[11] = mvalues[5] * mvalues[15] - mvalues[13] * mvalues[7];
			subFactors[12] = mvalues[4] * mvalues[13] - mvalues[12] * mvalues[5];
			subFactors[13] = mvalues[6] * mvalues[11] - mvalues[10] * mvalues[7];
			subFactors[14] = mvalues[5] * mvalues[11] - mvalues[9] * mvalues[7];
			subFactors[15] = mvalues[5] * mvalues[10] - mvalues[9] * mvalues[6];
			subFactors[16] = mvalues[4] * mvalues[11] - mvalues[8] * mvalues[7];
			subFactors[17] = mvalues[4] * mvalues[10] - mvalues[8] * mvalues[6];
			subFactors[18] = mvalues[4] * mvalues[9] - mvalues[8] * mvalues[5];
			var inverse = new Matrix(
				+(mvalues[5] * subFactors[0] - mvalues[6] * subFactors[1] + mvalues[7] * subFactors[2]),
				-(mvalues[1] * subFactors[0] - mvalues[2] * subFactors[1] + mvalues[3] * subFactors[2]),
				+(mvalues[1] * subFactors[6] - mvalues[2] * subFactors[7] + mvalues[3] * subFactors[8]),
				-(mvalues[1] * subFactors[13] - mvalues[2] * subFactors[14] + mvalues[3] * subFactors[15]),
				-(mvalues[4] * subFactors[0] - mvalues[6] * subFactors[3] + mvalues[7] * subFactors[4]),
				+(mvalues[0] * subFactors[0] - mvalues[2] * subFactors[3] + mvalues[3] * subFactors[4]),
				-(mvalues[0] * subFactors[6] - mvalues[2] * subFactors[9] + mvalues[3] * subFactors[10]),
				+(mvalues[0] * subFactors[13] - mvalues[2] * subFactors[16] + mvalues[3] * subFactors[17]),
				+(mvalues[4] * subFactors[1] - mvalues[5] * subFactors[3] + mvalues[7] * subFactors[5]),
				-(mvalues[0] * subFactors[1] - mvalues[1] * subFactors[3] + mvalues[3] * subFactors[5]),
				+(mvalues[0] * subFactors[11] - mvalues[1] * subFactors[9] + mvalues[3] * subFactors[12]),
				-(mvalues[0] * subFactors[14] - mvalues[1] * subFactors[16] + mvalues[3] * subFactors[18]),
				-(mvalues[4] * subFactors[2] - mvalues[5] * subFactors[4] + mvalues[6] * subFactors[5]),
				+(mvalues[0] * subFactors[2] - mvalues[1] * subFactors[4] + mvalues[2] * subFactors[5]),
				-(mvalues[0] * subFactors[8] - mvalues[1] * subFactors[10] + mvalues[2] * subFactors[12]),
				+(mvalues[0] * subFactors[15] - mvalues[1] * subFactors[17] + mvalues[2] * subFactors[18]));
			float[] ivalues = inverse.GetValues;
			float determinant = mvalues[0] * ivalues[0] + mvalues[1] * ivalues[4]+
				mvalues[2] * ivalues[8] + mvalues[3] * ivalues[12];
			inverse /= determinant;
			return inverse;
		}
	}
}
