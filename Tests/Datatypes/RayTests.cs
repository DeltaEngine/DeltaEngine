using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class RayTests
	{
		[Test]
		public void EqualityOfRay()
		{
			Assert.AreEqual(new Ray(Vector3D.UnitZ, Vector3D.One), new Ray(Vector3D.UnitZ, Vector3D.One));
			Assert.AreNotEqual(new Ray(Vector3D.UnitX, Vector3D.One),
				new Ray(Vector3D.UnitZ, Vector3D.One));
			Assert.AreNotEqual(new Ray(Vector3D.UnitZ, Vector3D.One),
				new Ray(Vector3D.UnitZ, Vector3D.One * 2));
		}

		[Test]
		public void CreateRay()
		{
			var ray = new Ray(Vector3D.Zero, Vector3D.UnitZ);
			Assert.AreEqual(ray.Origin, Vector3D.Zero);
			Assert.AreEqual(ray.Direction, Vector3D.UnitZ);
		}

		[Test]
		public void CanConvertToStringAndBack()
		{
			var ray = new Ray(Vector3D.UnitX, Vector3D.UnitY);
			var stringRay = ray.ToString();
			Assert.AreEqual("Ray({1, 0, 0},{0, 1, 0})", stringRay);
			var retrievedRay = new Ray(stringRay);
			Assert.AreEqual(ray.Origin, retrievedRay.Origin);
			Assert.AreEqual(ray.Direction, retrievedRay.Direction);
		}

		[Test]
		public void ConvertingInvalidStringThrows()
		{
			Assert.Throws<Ray.InvalidNumberOfStringComponents>(() => { new Ray("Ray({1, 0, 0})"); });
			Assert.Throws<Ray.InvalidStringFormat>(() => { new Ray("Fray({1, 0, 0},{0, 1, 0})"); });
		}
	}
}