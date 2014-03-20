using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class BoundingSphereTests
	{
		[Test]
		public void CreateBoundingSphere()
		{
			var boundingSphere = new BoundingSphere(Vector3D.One, 2.0f);
			Assert.AreEqual(Vector3D.One, boundingSphere.Center);
			Assert.AreEqual(2.0f, boundingSphere.Radius);
		}

		[Test]
		public void IntersectBoundingSphereWithBoundingSphere()
		{
			var sphere1 = new BoundingSphere(Vector3D.Zero, 1.0f);
			var sphere2 = new BoundingSphere(Vector3D.One, 1.0f);
			Assert.IsTrue(sphere1.IsColliding(sphere2));
		}
	}
}