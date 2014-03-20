using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	internal class QuaternionTests
	{
		[Test]
		public void SetQuaternion()
		{
			var quaternion = new Quaternion(5.2f, 2.6f, 4.4f, 1.1f);
			Assert.AreEqual(5.2f, quaternion.X);
			Assert.AreEqual(2.6f, quaternion.Y);
			Assert.AreEqual(4.4f, quaternion.Z);
			Assert.AreEqual(1.1f, quaternion.W);
		}

		[Test]
		public void CreateQuaternionFromAxisAngle()
		{
			var axis = Vector3D.UnitY;
			const float Angle = 90.0f;
			var quaternion = Quaternion.FromAxisAngle(axis, Angle);
			var sinHalfAngle = MathExtensions.Sin(Angle * 0.5f);
			Assert.AreEqual(quaternion.X, axis.X * sinHalfAngle);
			Assert.AreEqual(quaternion.Y, axis.Y * sinHalfAngle);
			Assert.AreEqual(quaternion.Z, axis.Z * sinHalfAngle);
			Assert.AreEqual(quaternion.W, MathExtensions.Cos(Angle * 0.5f));
		}

		[Test]
		public void Length()
		{
			Assert.AreEqual(5.477f, new Quaternion(1, -2, 3, -4).Length, 0.001f);
		}

		[Test]
		public void Normalize()
		{
			var quaternion = new Quaternion(1, 3, 5, 7);
			var expected = new Quaternion(0.1091f, 0.3273f, 0.5455f, 0.7638f);
			Assert.AreEqual(expected, Quaternion.Normalize(quaternion));
		}

		[Test]
		public void MultiplyByFloat()
		{
			var quaternion = new Quaternion(1, 3, 5, 7);
			Assert.AreEqual(new Quaternion(-2, -6, -10, -14), quaternion * -2);
		}

		[Test]
		public void Lerp()
		{
			var rightOrientedQuat = Quaternion.FromAxisAngle(Vector3D.UnitY, 90.0f);
			var leftOrientedQuat = Quaternion.FromAxisAngle(Vector3D.UnitY, -90.0f);
			var result = rightOrientedQuat.Lerp(leftOrientedQuat, 0.5f);
			Assert.AreEqual(result, new Quaternion(0.0f, 0.0f, 0.0f, 0.7071f));
		}

		[Test]
		public void Slerp()
		{
			var rightOrientedQuat = Quaternion.FromAxisAngle(Vector3D.UnitY, 90.0f);
			var leftOrientedQuat = Quaternion.FromAxisAngle(Vector3D.UnitY, -90.0f);
			var result = rightOrientedQuat.Slerp(leftOrientedQuat, 0.5f);
			Assert.AreEqual(result, Quaternion.FromAxisAngle(Vector3D.UnitY, 0.0f));
		}

		[Test]
		public void ConvertingQuaternionToMatrixAndBackLeavesItUnchanged()
		{
			var q1 = new Quaternion(1, 2, 3, 4);
			var q2 = new Quaternion(2, 0, 0, 1);
			var q3 = new Quaternion(0, 2, 0, 1);
			var q4 = new Quaternion(0, 0, 0, 1);
			Assert.AreEqual(q1, Quaternion.FromRotationMatrix(Matrix.FromQuaternion(q1)));
			Assert.AreEqual(q2, Quaternion.FromRotationMatrix(Matrix.FromQuaternion(q2)));
			Assert.AreEqual(q3, Quaternion.FromRotationMatrix(Matrix.FromQuaternion(q3)));
			Assert.AreEqual(q4, Quaternion.FromRotationMatrix(Matrix.FromQuaternion(q4)));
		}

		[Test]
		public void RotatingUnitXByQuaternionMatchesRotatingItByMatrix()
		{
			var quaternion = Quaternion.FromAxisAngle(Vector3D.UnitY, 90.0f);
			var rotatedViaMatrix = Matrix.FromQuaternion(quaternion) * Vector3D.UnitX;
			var rotatedViaQuaternion = quaternion * Vector3D.UnitX;
			Assert.AreEqual(rotatedViaMatrix, rotatedViaQuaternion);
		}

		[Test]
		public void RotatingVectorByQuaternionMatchesRotatingItByMatrix()
		{
			var axis = new Vector3D(4, 5, 6);
			axis.Normalize();
			var quaternion = Quaternion.FromAxisAngle(axis, 23.0f);
			var direction = new Vector3D(1, 2, 3);
			var rotatedViaMatrix = Matrix.FromQuaternion(quaternion) * direction;
			var rotatedViaQuaternion = quaternion * direction;
			Assert.IsTrue(rotatedViaQuaternion.IsNearlyEqual(rotatedViaMatrix));
		}

		[Test]
		public void MultiplyingTwoQuaternionsMatchesMultiplyingTwoMatrices()
		{
			var q1 = Quaternion.FromAxisAngle(Vector3D.Normalize(new Vector3D(1, 2, 3)), 40.0f);
			var q2 = Quaternion.FromAxisAngle(Vector3D.Normalize(new Vector3D(4, -5, 6)), -10.0f);
			var q3 = q1 * q2;
			var m1 = Matrix.FromQuaternion(q1);
			var m2 = Matrix.FromQuaternion(q2);
			var m3 = m1 * m2;
			Assert.AreEqual(q3, Quaternion.FromRotationMatrix(m3));
		}

		[Test]
		public void CheckCreateLookAt()
		{
			var matLookAt = Matrix.CreateLookAt(Vector3D.One, Vector3D.UnitY, Vector3D.UnitZ);
			var quaternion = Quaternion.CreateLookAt(Vector3D.One, Vector3D.UnitY, Vector3D.UnitZ);
			Assert.AreEqual(quaternion, Quaternion.FromRotationMatrix(matLookAt));
		}

		[Test]
		public void CheckVector()
		{
			Assert.AreEqual(new Vector3D(1, 2, 3), new Quaternion(1, 2, 3, 4).Vector3D);
		}

		[Test]
		public void CheckConjugate()
		{
			Assert.AreEqual(new Quaternion(-1, -2, -3, 4), new Quaternion(1, 2, 3, 4).Conjugate());
		}

		[Test]
		public void String()
		{
			Assert.AreEqual("1, 2, 3, 4", new Quaternion(1, 2, 3, 4).ToString());
		}

		[Test]
		public void ConvertBackFromString()
		{
			var quaternionFromString = new Quaternion("6, 3, 4, 6");
			Assert.AreEqual(new Quaternion(6, 3, 4, 6), quaternionFromString);
		}

		[Test]
		public void TryingToConvertFromInvalidStringThrows()
		{
			Assert.Throws<Quaternion.InvalidNumberOfComponents>(() => { new Quaternion("asdf"); });
			Assert.Throws<Quaternion.InvalidStringFormat>(() => { new Quaternion("a, s, d, f"); });
		}

		[Test]
		public void ToEuler()
		{
			VerifyEuler(new EulerAngles(-5, -40, 10));
			VerifyEuler(new EulerAngles(5, 10, 20));
			VerifyEuler(new EulerAngles(45, -30, 50));
		}

		private static void VerifyEuler(EulerAngles eulerAngles)
		{
			var rotationMatrix = Matrix.CreateRotationZYX(eulerAngles);
			var rotationQuaternion = Quaternion.FromRotationMatrix(rotationMatrix);
			Assert.AreEqual(eulerAngles, rotationQuaternion.ToEuler());
		}

		[Test]
		public void CalculatedAxesAndAnglesCorrect()
		{
			VerifyAxisAngle(Vector3D.UnitZ, 90);
			VerifyAxisAngle(new Vector3D(-1.0f, 2.3f, 4.5f), 270);
		}

		private static void VerifyAxisAngle(Vector3D axis, float angle)
		{
			axis.Normalize();
			var quaternion = Quaternion.FromAxisAngle(axis, angle);
			Assert.AreEqual(angle, quaternion.CalculateAxisAngle(), 0.0001f);
			var calculatedAxis = quaternion.CalculateRotationAxis();
			Assert.AreEqual(axis.X, calculatedAxis.X, 0.0001f);
			Assert.AreEqual(axis.Y, calculatedAxis.Y, 0.0001f);
			Assert.AreEqual(axis.Z, calculatedAxis.Z, 0.0001f);
		}
	}
}