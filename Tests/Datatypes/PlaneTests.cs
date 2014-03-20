using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class PlaneTests
	{
		[Test]
		public void EqualityOfPlanes()
		{
			const float Distance = 4.0f;
			Assert.AreEqual(new Plane(Vector3D.UnitZ, Distance), new Plane(Vector3D.UnitZ, Distance));
			Assert.AreNotEqual(new Plane(Vector3D.UnitZ, Distance), new Plane(Vector3D.UnitZ, 1));
			Assert.AreNotEqual(new Plane(Vector3D.UnitZ, Distance), new Plane(Vector3D.UnitX, Distance));
		}

		[Test]
		public void CreatePlaneFromDistance()
		{
			var plane = new Plane(Vector3D.UnitY, 1.0f);
			Assert.AreEqual(Vector3D.UnitY, plane.Normal);
			Assert.AreEqual(1.0f, plane.Distance);
		}

		[Test]
		public void CreatePlaneFromPointOnPlane()
		{
			var plane = new Plane(Vector3D.UnitY, new Vector3D(0, 1, 0));
			Assert.AreEqual(Vector3D.UnitY, plane.Normal);
			Assert.AreEqual(-1.0f, plane.Distance);
		}

		[Test]
		public void RayPlaneIntersect()
		{
			VerifyIntersectPoint(new Ray(Vector3D.UnitZ, -Vector3D.UnitZ), new Plane(Vector3D.UnitZ, 3.0f),
				-Vector3D.UnitZ * 3.0f);
			VerifyIntersectPoint(new Ray(3 * Vector3D.One, -Vector3D.One),
				new Plane(Vector3D.UnitY, Vector3D.One), Vector3D.One);
		}

		private static void VerifyIntersectPoint(Ray ray, Plane plane, Vector3D expectedIntersect)
		{
			Assert.AreEqual(expectedIntersect, plane.Intersect(ray));
		}

		[Test]
		public void RayPointingAwayFromPlaneDoesntIntersect()
		{
			var ray = new Ray(3 * Vector3D.One, Vector3D.One);
			var plane = new Plane(Vector3D.UnitY, Vector3D.One);
			Assert.IsNull(plane.Intersect(ray));
		}

		[Test]
		public void RayParallelToPlaneDoesntIntersect()
		{
			var ray = new Ray(Vector3D.One, Vector3D.UnitZ);
			var plane = new Plane(Vector3D.UnitY, Vector3D.Zero);
			Assert.IsNull(plane.Intersect(ray));
		}
	}
}