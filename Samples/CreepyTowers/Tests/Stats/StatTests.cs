using CreepyTowers.Stats;
using NUnit.Framework;

namespace CreepyTowers.Tests.Stats
{
	public class StatTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void SetUp()
		{
			stat = new Stat(100.0f);
		}

		private Stat stat;
		
		[Test]
		public void Constructor()
		{
			Assert.AreEqual(100.0f, stat.BaseValue);
		}

		[Test]
		public void Adjust()
		{
			stat.Adjust(-5.0f);
			Assert.AreEqual(95.0f, stat.Value);
			Assert.AreEqual(100.0f, stat.MaxValue);
		}

		[Test]
		public void CantAdjustToAboveMaxValue()
		{
			stat.Adjust(-20.0f);
			stat.Adjust(50.0f);
			Assert.AreEqual(100.0f, stat.Value);
		}

		[Test]
		public void CantAdjustToBelowZero()
		{
			stat.Adjust(-110.0f);
			Assert.AreEqual(0.0f, stat.Value);
		}

		[Test]
		public void AddBuff()
		{
		  stat.ApplyBuff(new BuffEffect("TestHpBuff"));
			Assert.AreEqual(304.0f, stat.Value);
			Assert.AreEqual(304.0f, stat.MaxValue);
		}

		[Test]
		public void RemoveBuff()
		{
			stat.ApplyBuff(new BuffEffect("TestHpBuff"));
			stat.RemoveBuff(new BuffEffect("TestHpBuff"));
			Assert.AreEqual(100.0f, stat.Value);
			Assert.AreEqual(100.0f, stat.MaxValue);
		}

		[Test]
		public void Percentage()
		{
			stat.Adjust(-40.0f);
			Assert.AreEqual(0.6f, stat.Percentage);
			Assert.AreEqual(0.0f, new Stat(0.0f).Percentage);
		}

		[Test]
		public void CheckToString()
		{
			stat.Adjust(-40.0f);
			Assert.AreEqual("60/100", stat.ToString());
		}
	}
}