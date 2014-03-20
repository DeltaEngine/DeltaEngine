using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace CreepyTowers.Tests.Creeps
{
	public class IronCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateClothCreep()
		{
			creep = new Creep(CreepType.Iron, Vector3D.Zero);
			new Player();
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerEffect()
		{
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Melt);
			Assert.AreEqual(Vulnerability.Normal,
				creep.State.VulnerabilityState[(int)TowerType.Slice]);
			Assert.AreEqual(Vulnerability.Weak,
				creep.State.VulnerabilityState[(int)TowerType.Impact]);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForAcidTowerEffect()
		{
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Acid, creep);
			Assert.IsTrue(creep.State.Melt);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForWaterTowerEffect()
		{
			StateChanger.CheckCreepState(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Rust);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyAcidThenWater()
		{
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Acid, creep);
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Water, creep);
			Assert.AreEqual(Vulnerability.Vulnerable,
				creep.State.VulnerabilityState[(int)TowerType.Water]);
			Assert.AreEqual(Vulnerability.HardBoiled,
				creep.State.VulnerabilityState[(int)TowerType.Slice]);
			Assert.AreEqual(Vulnerability.Resistant,
				creep.State.VulnerabilityState[(int)TowerType.Impact]);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyingIceToMeltedIronCreepMightShatter()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.0f }));
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Ice, creep);
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Acid, creep);
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Ice, creep);
			Assert.AreEqual(0, creep.GetStatValue("Hp"));
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyingIceToMeltedIronCreepMightNotShatter()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.9f }));
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Ice, creep);
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Acid, creep);
			IronCreepStateChanger.ChangeStatesIfIronCreep(TowerType.Ice, creep);
			Assert.AreEqual(1, creep.GetStatPercentage("Hp"));
		}
	}
}