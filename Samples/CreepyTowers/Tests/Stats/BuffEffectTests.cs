using CreepyTowers.Stats;
using NUnit.Framework;

namespace CreepyTowers.Tests.Stats
{
	public class BuffEffectTests : CreepyTowersGameForTests
	{
		[Test]
		public void SettingsConstructor()
		{
			var goldBuff = new BuffEffect("TestGoldBuff");
			Assert.AreEqual("Gold", goldBuff.Attribute);
			Assert.AreEqual(2.0f, goldBuff.Multiplier);
			Assert.AreEqual(-3.0f, goldBuff.Addition);
			Assert.AreEqual(5.0f, goldBuff.Duration);
		}
	}
}