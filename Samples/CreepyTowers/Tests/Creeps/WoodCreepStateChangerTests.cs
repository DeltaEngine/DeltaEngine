using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class WoodCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateSandCreep()
		{
			creep = new Creep(CreepType.Wood, Vector3D.Zero);
			new Player();
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTowerEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Fast);
			Assert.IsTrue(creep.State.Burst);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForWaterTowerEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Healing);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForIceTowerEffect()
		{
			StateChanger.CheckCreepState(TowerType.Ice, creep);
			Assert.IsFalse(creep.State.Burst);
			Assert.IsFalse(creep.State.Burn);
			Assert.IsFalse(creep.State.Fast);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckWaterThenFireEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Wet);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckWaterThenIceThenFireEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Wet);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Frozen);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckWaterThenIceThenImpactEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Wet);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Ice, creep);
			Assert.IsTrue(creep.State.Frozen);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Impact, creep);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckFireThenWaterEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Burst);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Water, creep);
			Assert.IsFalse(creep.State.Burst);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckFireThenIceEffect()
		{
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Burst);
			WoodCreepStateChanger.ChangeStatesIfWoodCreep(TowerType.Ice, creep);
			Assert.IsFalse(creep.State.Burst);
		}
	}
}
