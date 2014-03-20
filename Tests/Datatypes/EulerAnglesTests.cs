using System.Collections.Generic;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace DeltaEngine.Tests.Datatypes
{
	public class EulerAnglesTests
	{
		[Test]
		public void Create()
		{
			var euler = new EulerAngles(1, 2, 3);
			Assert.AreEqual(euler.Pitch, 1);
			Assert.AreEqual(euler.Yaw, 2);
			Assert.AreEqual(euler.Roll, 3);
		}

		[Test]
		public void Equals()
		{
			var euler1 = new EulerAngles(1, 2, 3);
			var euler2 = new EulerAngles(3, 4, 5);
			Assert.AreNotEqual(euler1, euler2);
			Assert.AreEqual(euler1, new EulerAngles(1, 2, 3));
			Assert.IsTrue(euler1 == new EulerAngles(1, 2, 3));
			Assert.IsTrue(euler1 != euler2);
			Assert.IsTrue(euler1.Equals((object)new EulerAngles(1, 2, 3)));
		}

		[Test]
		public void GetHashCodeViaDictionary()
		{
			var first = new EulerAngles(1, 2, 3);
			var second = new EulerAngles(3, 4, 5);
			var eulerAngles = new Dictionary<EulerAngles, int> { { first, 1 }, { second, 2 } };
			Assert.IsTrue(eulerAngles.ContainsKey(first));
			Assert.IsTrue(eulerAngles.ContainsKey(second));
			Assert.IsFalse(eulerAngles.ContainsKey(new EulerAngles(5, 6, 7)));
		}

		[Test]
		public void EulerAnglesToString()
		{
			Assert.AreEqual("Pitch: 3 Yaw: 4 Roll: 5", new EulerAngles(3, 4, 5).ToString());
		}
	}
}