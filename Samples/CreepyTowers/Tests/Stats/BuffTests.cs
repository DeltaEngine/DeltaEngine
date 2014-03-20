using CreepyTowers.Stats;
using NUnit.Framework;

namespace CreepyTowers.Tests.Stats
{
	public class BuffTests : CreepyTowersGameForTests
	{
		[Test]
		public void Constructor()
		{
			var stat = new Stat(100.0f);
			var effect = new BuffEffect("TestGoldBuff");
			var buff = new Buff(stat, effect);
			Assert.AreEqual(stat, buff.Stat);
			Assert.AreEqual(effect, buff.Effect);
			Assert.AreEqual(0, buff.Elapsed);
		}

		[Test]
		public void Properties()
		{
			var stat = new Stat(100.0f);
			const float Elapsed = 4.0f;
			var effect = new BuffEffect("TestGoldBuff");
			var buff = new Buff(new Stat(0.0f), new BuffEffect("TestHpBuff"))
			{
				Stat = stat,
				Effect = effect,
				Elapsed = Elapsed
			};
			Assert.AreEqual(stat, buff.Stat);
			Assert.AreEqual(effect, buff.Effect);
			Assert.AreEqual(Elapsed, buff.Elapsed);
		}

		[Test]
		public void IsNotExpiredWhenDurationIsZero()
		{
			var buff = new Buff(new Stat(0.0f), new BuffEffect("DragonRangeMultiplier")) { Elapsed = 10 };
			Assert.IsFalse(buff.IsExpired);
		}

		[Test]
		public void IsNotExpiredWhenElapsedIsBelowDuration()
		{
			var buff = new Buff(new Stat(0.0f), new BuffEffect("TestHpBuff")) { Elapsed = 4};
			Assert.IsFalse(buff.IsExpired);
		}

		[Test]
		public void IsExpiredWhenElapsedIsAboveDuration()
		{
			var buff = new Buff(new Stat(0.0f), new BuffEffect("TestHpBuff")) { Elapsed = 10 };
			Assert.IsTrue(buff.IsExpired);
		}
	}
}