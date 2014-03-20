using CreepyTowers.Stats;
using NUnit.Framework;

namespace CreepyTowers.Tests.Stats
{
	public class StatAdjustmentTests : CreepyTowersGameForTests
	{
		[Test]
		public void Constructor()
		{
			var effect = new StatAdjustment("Hp", "Armor", -50);
			Assert.AreEqual("Hp", effect.Attribute);
			Assert.AreEqual("Armor", effect.Resist);
			Assert.AreEqual(-50.0f, effect.Adjustment);
		}

		[Test]
		public void ConstructedFromXml()
		{
			var effect = new StatAdjustment("TestAdjustment");
			Assert.AreEqual("Hp", effect.Attribute);
			Assert.AreEqual("", effect.Resist);
			Assert.AreEqual(-100.0f, effect.Adjustment);
		}
	}
}