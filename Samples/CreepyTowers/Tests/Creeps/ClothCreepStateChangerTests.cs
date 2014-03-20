using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace CreepyTowers.Tests.Creeps
{
	public class ClothCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateClothCreep()
		{
			creep = new Creep(CreepType.Cloth, Vector3D.Zero);
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void NonClothCreepsShouldBeIgnored()
		{
			var glassCreep = new Creep(CreepType.Glass, Vector3D.Zero);
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Acid, glassCreep);
			Assert.AreEqual(creep.State.Wet, glassCreep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerEffect()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Slow);
			Assert.AreEqual(-1, creep.State.SlowTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForImpactTowerEffect()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Slow);
			Assert.AreEqual(0, creep.State.SlowTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void ChecForkWaterTowerEffect()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Slow);
			Assert.AreEqual(0, creep.State.SlowTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForAcidTowerEffect()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Acid, creep);
			Assert.IsTrue(creep.State.Enfeeble);
			Assert.AreEqual(0, creep.State.EnfeebleTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerOnDryClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Burst);
			Assert.AreEqual(0, creep.State.BurstTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerWetClothCreep()
		{
			creep.State.Wet = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Wet);
			Assert.AreEqual(Vulnerability.HardBoiled,
				creep.State.GetVulnerability(TowerType.Impact));
			Assert.AreEqual(Vulnerability.HardBoiled,
				creep.State.GetVulnerability(TowerType.Ice));
			Assert.AreEqual(Vulnerability.Weak,
				creep.State.GetVulnerability(TowerType.Slice));
			Assert.AreEqual(Vulnerability.Vulnerable,
				creep.State.GetVulnerability(TowerType.Acid));
			Assert.AreEqual(Vulnerability.Vulnerable,
				creep.State.GetVulnerability(TowerType.Fire));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerOnFrozenClothCreep()
		{
			creep.State.Frozen = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Frozen);
			Assert.IsFalse(creep.State.Burst);
			Assert.IsFalse(creep.State.Burn);
			Assert.IsTrue(creep.State.Wet);
			Assert.AreEqual(0, creep.State.WetTimer);
			Assert.AreEqual(Vulnerability.Resistant,
				creep.State.GetVulnerability(TowerType.Fire));
			Assert.AreEqual(Vulnerability.HardBoiled,
				creep.State.GetVulnerability(TowerType.Impact));
			Assert.AreEqual(Vulnerability.Weak,
				creep.State.GetVulnerability(TowerType.Ice));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForWaterTowerOnFrozenClothCreep()
		{
			creep.State.Frozen = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Frozen);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForImpactTowerOnFrozenClothCreep()
		{
			Randomizer.Use(new FixedRandom(new[] { 0f }));
			creep.State.Frozen = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Frozen);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerOnFrozenClothCreep()
		{
			creep.State.Frozen = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForWaterTowerOnDryClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Water, creep);
			Assert.IsFalse(creep.State.Frozen);
			Assert.IsFalse(creep.State.Burst);
			Assert.IsFalse(creep.State.Burn);
			Assert.IsTrue(creep.State.Wet);
			Assert.AreEqual(0, creep.State.WetTimer);
			Assert.AreEqual(Vulnerability.Resistant,
				creep.State.GetVulnerability(TowerType.Fire));
			Assert.AreEqual(Vulnerability.HardBoiled,
				creep.State.GetVulnerability(TowerType.Impact));
			Assert.AreEqual(Vulnerability.Weak,
				creep.State.GetVulnerability(TowerType.Ice));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerOnWetClothCreep()
		{
			creep.State.Wet = true;
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Frozen);
			Assert.IsTrue(creep.State.Paralysed);
			Assert.AreEqual(0, creep.State.FrozenTimer);
			Assert.AreEqual(Vulnerability.Resistant,
				creep.State.GetVulnerability(TowerType.Slice));
			Assert.AreEqual(Vulnerability.Resistant,
				creep.State.GetVulnerability(TowerType.Water));
			Assert.AreEqual(Vulnerability.Vulnerable,
				creep.State.GetVulnerability(TowerType.Impact));
			Assert.AreEqual(Vulnerability.Immune,
				creep.State.GetVulnerability(TowerType.Fire));
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerOnDryClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
			Assert.IsFalse(creep.State.Wet);
			Assert.IsFalse(creep.State.Burst);
			Assert.IsFalse(creep.State.Burn);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyFireThenIceToClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
			Assert.IsFalse(creep.State.Fast);
			Assert.IsFalse(creep.State.Burst);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyIceThenFireToClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Ice, creep);
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Slow);
			Assert.IsFalse(creep.State.Fast);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyFireThenWaterToClothCreep()
		{
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Fire, creep);
			ClothCreepStateChanger.ChangeStatesIfClothCreep(TowerType.Water, creep);
			Assert.IsFalse(creep.State.Fast);
		}

		[Test, CloseAfterFirstFrame]
		public void WetCreepAfterDryingIsNotSlow()
		{
			new Player();
			creep.State.Wet = true;
			AdvanceTimeAndUpdateEntities(2.1f);
			Assert.IsFalse(creep.State.Slow);
		}

		[Test, CloseAfterFirstFrame]
		public void CreepAfterMeltStopIsNotSlowOrEnfeeble()
		{
			new Player();
			creep.State.Melt = true;
			AdvanceTimeAndUpdateEntities(2.1f);
			Assert.IsFalse(creep.State.Slow);
			Assert.IsFalse(creep.State.Enfeeble);
		}

		[Test, CloseAfterFirstFrame]
		public void CompareTwoCreepStates()
		{
			creep.State.Wet = true;
			var creepA = new Creep(CreepType.Plastic, Vector2D.Half);
			creepA.State.Melt = true;
			Assert.IsFalse(creep.State.Equals(creepA.State));
			creepA.State.Wet = true;
			creepA.State.Melt = false;
			Assert.IsTrue(creep.State.Equals(creepA.State));
		}
	}
}