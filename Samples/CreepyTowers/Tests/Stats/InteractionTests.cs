using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Stats
{
	public class InteractionTests : CreepyTowersGameForTests
	{
		[SetUp]
		public void SetUp()
		{
			new Player();
			tower = new Tower(TowerType.Fire, Vector3D.Zero);
			creep = new Creep(CreepType.Cloth, Vector3D.Zero);
			adjustment = new StatAdjustment("TestAdjustment");
		}

		private Tower tower;
		private Creep creep;
		private StatAdjustment adjustment;

		[Test]
		public void Constructor()
		{
			var effect = new BuffEffect("TestHpBuff");
			var interaction = new Interaction(tower, creep, adjustment, effect);
			Assert.AreEqual(tower, interaction.Source);
			Assert.AreEqual(creep, interaction.Target);
		}

		[Test]
		public void PerformAdjustment()
		{
			var interaction = new Interaction(tower, creep, adjustment);
			interaction.Apply();
			Assert.AreEqual(0.0f, creep.GetStatValue("Hp"));
		}

		[Test]
		public void PerformBuff()
		{
			var effect = new BuffEffect("TestHpBuff");
			var interaction = new Interaction(tower, creep, adjustment, effect);
			interaction.Apply();
			Assert.AreEqual(204.0f, creep.GetStatValue("Hp"));
		}
	}
}