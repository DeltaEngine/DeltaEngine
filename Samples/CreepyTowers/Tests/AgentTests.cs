using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests
{
	public class AgentTests : CreepyTowersGameForTests
	{
		[SetUp]
		public override void Initialize()
		{
			base.Initialize();
			creep = new Creep(CreepType.Cloth, Vector3D.Zero) { Path = null };
		}

		private Creep creep;

		[Test]
		public void InitialValues()
		{
			Assert.AreEqual(100.0, creep.GetStatValue("Hp"));
			Assert.AreEqual(1.0, creep.GetStatPercentage("Hp"));
		}

		[Test]
		public void ApplyBuff()
		{
			creep.ApplyBuff(new BuffEffect("TestHpBuff"));
			Assert.AreEqual(304.0, creep.GetStatValue("Hp"));
			Assert.AreEqual(100.0, creep.GetStatBaseValue("Hp"));
			Assert.AreEqual(1.0, creep.GetStatPercentage("Hp"));
		}

		[Test]
		public void ReceiveBuffOnAttributeItDoesNotHave()
		{
			Assert.DoesNotThrow(() => creep.ApplyBuff(new BuffEffect("DragonRangeMultiplier")));
		}

		[Test, CloseAfterFirstFrame]
		public void AdjustStatDownwards()
		{
			creep.AdjustStat(new StatAdjustment("Hp", "", -50.0f));
			Assert.AreEqual(50.0f, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void AdjustStatUpwards()
		{
			creep.AdjustStat(new StatAdjustment("Hp", "", -50.0f));
			creep.AdjustStat(new StatAdjustment("Hp", "", 20.0f));
			Assert.AreEqual(70.0f, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void NotElapsedBuffDoesNotExpire()
		{
			creep.ApplyBuff(new BuffEffect("TestGoldBuff"));
			AdvanceTimeAndUpdateEntities(1.0f);
			Assert.AreEqual(23.0f, creep.GetStatValue("Gold"));
		}

		[Test, CloseAfterFirstFrame]
		public void ElapsedBuffExpires()
		{
			creep.ApplyBuff(new BuffEffect("TestGoldBuff"));
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			AdvanceTimeAndUpdateEntities(7.0f);
			Assert.AreEqual(1, EntitiesRunner.Current.GetEntitiesOfType<Creep>().Count);
			Assert.AreEqual(13.0f, creep.GetStatValue("Gold"));
		}
	}
}
