using System;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class MatrixTests
	{
		[SetUp]
		public void SetUp()
		{
			matrix = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
		}

		private Matrix matrix;

		[Test]
		public void MatrixZero()
		{
			matrix = new Matrix();
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(0, matrix[i]);
		}

		[Test]
		public void CreateWith16Floats()
		{
			AssertValues0To15();
		}

		private void AssertValues0To15()
		{
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(i, matrix[i]);
		}

		[Test]
		public void CreateFromString()
		{
			var textMatrix = new Matrix("0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15");
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(textMatrix[i], matrix[i]);
		}

		[Test]
		public void CheckRightVector()
		{
			Assert.AreEqual(new Vector3D(0, 1, 2), matrix.Right);
		}

		[Test]
		public void CheckUpVector()
		{
			Assert.AreEqual(new Vector3D(4, 5, 6), matrix.Up);
		}

		[Test]
		public void CheckForwardVector()
		{
			Assert.AreEqual(new Vector3D(8, 9, 10), matrix.Forward);
		}

		[Test]
		public void SetRightVector()
		{
			matrix.Right = new Vector3D(1, 10, 20);
			Assert.AreEqual(new Vector3D(1, 10, 20), matrix.Right);
		}

		[Test]
		public void SetUpVector()
		{
			matrix.Up = new Vector3D(3, 20, 30);
			Assert.AreEqual(new Vector3D(3, 20, 30), matrix.Up);
		}

		[Test]
		public void setForwardVector()
		{
			matrix.Forward = new Vector3D(9, 26, 44);
			Assert.AreEqual(new Vector3D(9, 26, 44), matrix.Forward);
		}

		[Test]
		public void CheckTranslationVector()
		{
			Assert.AreEqual(new Vector3D(12, 13, 14), matrix.Translation);
		}

		[Test]
		public void ChangeTranslationVector()
		{
			var vector = new Vector3D(-1, -2, -3);
			matrix.Translation = vector;
			Assert.AreEqual(vector, matrix.Translation);
		}

		[Test]
		public void CreateWithFloatArray()
		{
			matrix = new Matrix(new float[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
			AssertValues0To15();
		}

		[Test]
		public void CreateScale()
		{
			matrix = Matrix.CreateScale(Vector3D.One*2);
			var expected = new Matrix(2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2, 0, 0, 0, 0, 1);
			Assert.AreEqual(expected, matrix);
		}

		[Test]
		public void CreateScaleFromThreeScalar()
		{
			matrix = Matrix.CreateScale(3, 4, 7);
			var expected = new Matrix(3, 0, 0, 0, 0, 4, 0, 0, 0, 0, 7, 0, 0, 0, 0, 1);
			Assert.AreEqual(expected, matrix);
		}

		[Test]
		public void SizeOfMatrix()
		{
			Assert.AreEqual(64, Matrix.SizeInBytes);
		}

		[Test]
		public void RotateMatrix()
		{
			var matrix1 = new Matrix(1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1);
			var matrix2 = new Matrix(0, 0, -1, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1);
			var matrix3 = new Matrix(0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
			var matrix4 = matrix1 * (matrix2 * matrix3);
			Assert.IsTrue(matrix1.IsNearlyEqual(Matrix.CreateRotationZYX(90, 0, 0)));
			Assert.IsTrue(matrix2.IsNearlyEqual(Matrix.CreateRotationZYX(0, 90, 0)));
			Assert.IsTrue(matrix3.IsNearlyEqual(Matrix.CreateRotationZYX(0, 0, 90)));
			Assert.IsTrue(matrix4.IsNearlyEqual(Matrix.CreateRotationZYX(90, 90, 90)));
		}

		[Test]
		public void FromQuaternion()
		{
			var quaternion = Quaternion.FromAxisAngle(Vector3D.UnitY, 60.0f);
			matrix = Matrix.CreateRotationY(60.0f);
			Assert.AreEqual(matrix, Matrix.FromQuaternion(quaternion));
		}

		[Test]
		public void TranslateMatrix()
		{
			var matrix1 = Matrix.CreateTranslation(1, 2, 3);
			var matrix2 = Matrix.CreateTranslation(1, 0, 0);
			Assert.AreEqual(new Vector3D(1, 2, 3), matrix1.Translation);
			Assert.AreEqual(Vector3D.UnitX, matrix2.Translation);
		}

		[Test]
		public void Transpose()
		{
			var expected = new Matrix(0, 4, 8, 12, 1, 5, 9, 13, 2, 6, 10, 14, 3, 7, 11, 15);
			Assert.AreEqual(expected, Matrix.Transpose(matrix));
		}

		[Test]
		public void TransposingTwiceReturnsTheOriginal()
		{
			Assert.AreEqual(matrix, Matrix.Transpose(Matrix.Transpose(matrix)));
		}

		[Test]
		public void Invert()
		{
			var expected1 = new Matrix(1, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1);
			var expected2 = new Matrix(0, 0, 1, 0, 0, 1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1);
			var expected3 = new Matrix(0, -1, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
			Assert.IsTrue(Matrix.Invert(Matrix.CreateRotationX(90.0f)).IsNearlyEqual(expected1));
			Assert.IsTrue(Matrix.Invert(Matrix.CreateRotationY(90.0f)).IsNearlyEqual(expected2));
			Assert.IsTrue(Matrix.Invert(Matrix.CreateRotationZ(90.0f)).IsNearlyEqual(expected3));
		}

		[Test]
		public void InvertTranspose()
		{
			var source = new Matrix(-4, 0, 0, 0, -1, 2, 0, 0, -4, 4, 4, 0, -1, -9, -1, 1);
			var expected1 = new Matrix(-0.25f, -0.125f, -0.125f, -1.5f, 0, 0.5f, -0.5f, 4, 0, 0, 0.25f,
				0.25f, 0, 0, 0, 1);
			Assert.IsTrue(Matrix.InverseTranspose(source).IsNearlyEqual(expected1));
		}

		[Test]
		public void InvertingTwiceReturnsTheOriginal()
		{
			matrix = Matrix.CreateRotationY(60.0f);
			Assert.IsTrue(Matrix.Invert(Matrix.Invert(matrix)).IsNearlyEqual(matrix));
		}

		[Test]
		public void Unproject()
		{
			matrix = Matrix.CreatePerspective(60.0f, 1.0f, 0.5f, 10.0f);
			var position = Matrix.TransformHomogeneousCoordinate(Vector3D.One, matrix);
			var invMatrix = Matrix.Invert(matrix);
			Assert.IsTrue(
				Matrix.TransformHomogeneousCoordinate(position, invMatrix).IsNearlyEqual(Vector3D.One));
		}

		[Test]
		public void CreateOrtographicProjectionMatrix()
		{
			var size = new Size(10, 5);
			matrix = Matrix.CreateOrthoProjection(size);
			var expected = new Matrix(0.2f, 0.0f, 0.0f, 0.0f, 0.0f, -0.4f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
				0.0f, -1.0f, 1.0f, 0.0f, 1.0f);
			Assert.AreEqual(expected, matrix);
		}

		[Test]
		public void CreateOrtographicProjectionMatrixwithNearAndFarPlanes()
		{
			var size = new Size(10, 5);
			matrix = Matrix.CreateOrthoProjection(size, 0.1f, 10.0f);
			var expected = new Matrix(0.2f, 0.0f, 0.0f, 0.0f, 0.0f, 0.4f, 0.0f, 0.0f, 0.0f, 0.0f, -0.202f,
				0.0f, 0.0f, 0.0f, -1.0202f, 1.0f);
			Assert.IsTrue(expected.IsNearlyEqual(matrix));
		}

		[Test]
		public void CreateLookAt()
		{
			var createdLookAt = Matrix.CreateLookAt(Vector3D.UnitZ, Vector3D.UnitX, Vector3D.UnitY);
			var expected = new Matrix(0.7071f, 0.0f, -0.7071f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
				0.7071f, -0.0f, 0.7071f, 0.0f, -0.7071f, 0.0f, -0.7071f, 1.0f);
			Assert.IsTrue(createdLookAt.IsNearlyEqual(expected));
		}

		[Test]
		public void CreateLookAtFiveMetersDistance()
		{
			var createdLookAt = Matrix.CreateLookAt(new Vector3D(0.0f, 0.0f, 5.0f), Vector3D.Zero,
				Vector3D.UnitY);
			var expected = new Matrix(
				1.0f, 0.0f, 0.0f, 0.0f,
				0.0f, 1.0f, 0.0f, 0.0f,
				0.0f, 0.0f, 1.0f, 0.0f, 
				0.0f, 0.0f, -5.0f, 1.0f);
			Assert.IsTrue(createdLookAt.IsNearlyEqual(expected));
		}

		[Test]
		public void AccessViolation()
		{
			float num = matrix[15];
			Assert.AreEqual(15, num);
			Assert.Throws<IndexOutOfRangeException>(delegate { num = matrix[17]; });
		}

		[Test]
		public void IdentityHasDeterminantOne()
		{
			Assert.AreEqual(1, Matrix.Identity.GetDeterminant());
		}

		[Test]
		public void GetDeterminant()
		{
			matrix = new Matrix(1, 0, 0, 0, 0, 1, 2, 1, 0, 2, 1, 3, 0, 2, 1, 1);
			Assert.AreEqual(6, matrix.GetDeterminant());
		}

		[Test]
		public void AreEqual()
		{
			var matrix2 = new Matrix(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15);
			Assert.IsTrue(matrix == matrix2);
			Assert.IsTrue(matrix.Equals(matrix2));
			matrix2[5] = 20;
			Assert.IsTrue(matrix != matrix2);
			object pointAsObject = Vector2D.One;
			Assert.IsFalse(matrix.Equals(pointAsObject));
		}

		[Test]
		public void MultiplyVector()
		{
			var vector = new Vector3D(-1, -2, -3);
			Assert.AreEqual(new Vector3D(-20, -25, -30), matrix * vector);
		}

		[Test]
		public void MatrixTimesIdentityIsUnchanged()
		{
			Assert.AreEqual(matrix, matrix * Matrix.Identity);
			Assert.AreEqual(matrix, Matrix.Identity * matrix);
		}

		[Test]
		public void MultiplyMatrix()
		{
			var matrix2 = new Matrix(-1, -2, -3, -4, -5, -6, -7, -8, -9, -10, -11, -12, -13, -14, -15,
				-16);
			var result = new Matrix(-62, -68, -74, -80, -174, -196, -218, -240, -286, -324, -362, -400,
				-398, -452, -506, -560);
			Assert.AreEqual(result, matrix * matrix2);
		}

		[Test]
		public void IsNotNearlyEqual()
		{
			matrix = new Matrix(1, 0, 3, 0, 0, 2, 0, 4, 0, 3, 2, 0, 1, 0, 0, 2);
			Assert.IsFalse(matrix.IsNearlyEqual(Matrix.Identity));
		}

		[Test]
		public void WriteMatrix()
		{
			const string MatrixString = "0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15";
			Assert.AreEqual(MatrixString, matrix.ToString());
		}

		[Test]
		public void LoadFromString()
		{
			const string MatrixString = "0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15";
			var loadedMatrix = new Matrix(MatrixString);
			for (int i = 0; i < 16; i++)
				Assert.AreEqual(i, loadedMatrix.GetValues[i]);
		}

		[Test]
		public void CalculateHashCode()
		{
			Assert.AreEqual(1069547520, matrix.GetHashCode());
			Assert.AreEqual(0, new Matrix().GetHashCode());
		}

		[Test]
		public void TransformPosition()
		{
			var position = new Vector3D(3, 5, 2);
			var translation = Matrix.CreateTranslation(2, 0, 5);
			var rotation = Matrix.CreateRotationZYX(0, 90, 0);
			var scale = Matrix.CreateScale(3, 3, 3);
			var transformation = scale * rotation * translation;
			var result = translation * (rotation * (scale * position));
			Assert.IsTrue((transformation * position).IsNearlyEqual(result));
		}

		[Test]
		public void CreateFromAxisAngle()
		{
			var invTwoSqr = MathExtensions.InvSqrt(2);
			var matrix2 = new Matrix(invTwoSqr, 0, -invTwoSqr, 0, 0, 1, 0, 0, invTwoSqr, 0, invTwoSqr, 
				0, 0, 0, 0, 1);
			var axis = new Vector3D(0, 1, 0);
			const float Angle = 45.0f;
			Assert.AreEqual(Matrix.Identity, Matrix.FromAxisAngle(axis, 0.0f));
			Assert.AreEqual(matrix2, Matrix.FromAxisAngle(axis, Angle));
		}

		[Test]
		public void CreateFrustrum()
		{
			var matrixSimetric = new Matrix(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, -1, 1);
			Assert.AreEqual(matrixSimetric, Matrix.Frustum(-1, 1, -1, 1, 0, 100.0f));			
		}
	}
}