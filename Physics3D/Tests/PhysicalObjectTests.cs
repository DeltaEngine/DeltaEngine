using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Physics3D.Tests
{
	public class PhysicalObjectTests : TestWithMocksOrVisually
	{
		[Test]
		public void VerifyAssignedValues()
		{
			var physicalObject = new PhysicalEntity3D(Vector3D.Zero, Quaternion.Identity, 10.0f, 0.5f);
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(10.0f, physicalObject.Mass);
			Assert.AreEqual(0.5f, physicalObject.LifeTime);
			Assert.IsFalse(physicalObject.IsActive);
		}
	}
}
