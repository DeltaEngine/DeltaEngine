using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class BoundingBoxTests
	{
		[Test]
		public void CreateBoundingBoxFromMinAndMax()
		{
			var boundingBox = new BoundingBox(Vector3D.Zero, Vector3D.One);
			Assert.AreEqual(Vector3D.Zero, boundingBox.Min);
			Assert.AreEqual(Vector3D.One, boundingBox.Max);
		}

		[Test]
		public void CreateBoundingBoxFromCenter()
		{
			var box = BoundingBox.FromCenter(Vector3D.Zero, Vector3D.One);
			Assert.AreEqual(-Vector3D.One / 2, box.Min);
			Assert.AreEqual(Vector3D.One / 2, box.Max);
		}

		[Test]
		public void CreateBoundingBoxByPoints()
		{
			Assert.Throws<BoundingBox.NoPointsSpecified>(() => new BoundingBox(null));
			Assert.Throws<BoundingBox.NoPointsSpecified>(() => new BoundingBox(new Vector3D[0]));
			var points = new[] { new Vector3D(2, 5, 7), new Vector3D(6, 4, 2), new Vector3D(1, 7, 9), };
			var boundingBox = new BoundingBox(points);
			Assert.AreEqual(boundingBox.Min, new Vector3D(1, 4, 2));
			Assert.AreEqual(boundingBox.Max, new Vector3D(6, 7, 9));
		}

		[Test]
		public void CheckForExactEquality()
		{
			var box = new BoundingBox(Vector3D.Zero, Vector3D.One);
			var equalBox = new BoundingBox(Vector3D.Zero, Vector3D.One);
			Assert.IsTrue(box.Equals((object)equalBox));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var box = new BoundingBox(Vector3D.Zero, Vector3D.One);
			var equalBox = new BoundingBox(Vector3D.Zero, Vector3D.One);
			Assert.AreEqual(box.GetHashCode(), equalBox.GetHashCode());
			var otherBox = new BoundingBox(-Vector3D.One, Vector3D.One);
			Assert.AreNotEqual(box.GetHashCode(), otherBox.GetHashCode());
		}

		[Test]
		public void CheckForNearlyEqual()
		{
			var boxMaximum = Vector3D.One;
			var box = new BoundingBox(Vector3D.Zero, boxMaximum);
			const float AllowedValueEpsilon = MathExtensions.Epsilon * 0.999f;
			boxMaximum += new Vector3D(AllowedValueEpsilon, AllowedValueEpsilon, AllowedValueEpsilon);
			var otherBox = new BoundingBox(Vector3D.Zero, boxMaximum);
			Assert.IsTrue(box.IsNearlyEqual(otherBox));
		}

		[Test]
		public void IntersectBoundingBoxWithBoundingBox()
		{
			var box1 = new BoundingBox(Vector3D.Zero, Vector3D.One);
			var box2 = new BoundingBox(Vector3D.One / 2, Vector3D.One);
			Assert.IsTrue(box1.IsColliding(box2));
			var box3 = new BoundingBox(Vector3D.One * 2, Vector3D.One);
			Assert.IsFalse(box1.IsColliding(box3));
		}

		[Test]
		public void IntersectBoundingBoxWithBoundingSphere()
		{
			var boundingBox = new BoundingBox(Vector3D.Zero, Vector3D.One);
			var sphere = new BoundingSphere(Vector3D.Zero, 0.5f);
			Assert.IsTrue(boundingBox.IsColliding(sphere));
		}

		[Test]
		public void IntersectBoundingBoxWithRay()
		{
			var boundingBox = new BoundingBox(Vector3D.One * -0.5f, Vector3D.One * 0.5f);
			var ray = new Ray(Vector3D.UnitY * 2.0f, -Vector3D.UnitY);
			Assert.AreEqual(new Vector3D(0.0f, 0.5f, 0.0f), boundingBox.Intersect(ray));
		}

		[Test]
		public void MissBoundingBoxWithRay()
		{
			var boundingBox = new BoundingBox(Vector3D.One * -0.5f, Vector3D.One * 0.5f);
			var ray = new Ray(Vector3D.UnitZ * 2.0f, -Vector3D.UnitY);
			Assert.IsNull(boundingBox.Intersect(ray));
		}

		[Test]
		public void MissBoundingBoxWithRayFromBehind()
		{
			var boundingBox = new BoundingBox(Vector3D.One * -0.5f, Vector3D.One * 0.5f);
			var ray = new Ray(Vector3D.UnitY * 2.0f, Vector3D.UnitY);
			Assert.IsNull(boundingBox.Intersect(ray));
		}

		[Test]
		public void MergeTwoBoundingBoxes()
		{
			var boxA = new BoundingBox(Vector3D.Zero, Vector3D.Zero);
			boxA.Merge(new BoundingBox(-Vector3D.One, Vector3D.One));
			Assert.AreEqual(boxA, new BoundingBox(-Vector3D.One, Vector3D.One));
			boxA.Merge(new BoundingBox(Vector3D.Zero, Vector3D.Zero));
			Assert.AreEqual(boxA, new BoundingBox(-Vector3D.One, Vector3D.One));
		}
	}
}
