using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class SandCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateSandCreep()
		{
			creep = new Creep(CreepType.Sand, Vector3D.Zero);
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckForImpactTowerEffect()
		{
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Slow);
			Assert.AreEqual(0, creep.State.SlowTimer);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForWaterTowerEffect()
		{
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Slow);
			Assert.AreEqual(0, creep.State.SlowTimer);
			Assert.IsTrue(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerOnWetCreep()
		{
			creep.State.Wet = true;
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Frozen);
			Assert.IsFalse(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForImpactTowerOnFrozenCreep()
		{
			creep.State.Frozen = true;
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Frozen);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerEffectOnWetCreep()
		{
			creep.State.Wet = true;
			StateChanger.CheckCreepState(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckWaterThenIceThenFireEffect()
		{
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Wet);
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Frozen);
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Frozen);
			Assert.IsTrue(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckFireEffect()
		{
			SandCreepStateChanger.ChangeStatesIfSandCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Frozen);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckUnfreezableCanNotBeFrozenEffect()
		{
			creep.State.Unfreezable = true;
			StateChanger.MakeCreepFrozen(creep);
			Assert.IsFalse(creep.State.Frozen);
		}
	}
}