using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class PaperCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreatePaperCreep()
		{
			creep = new Creep(CreepType.Paper, Vector3D.Zero);
			new Player();
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckWaterEffect()
		{
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Water, creep);
			Assert.IsTrue(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyWaterThenFireToPaperCreep()
		{
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Water, creep);
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyWaterThenIceThenFireToPaperCreep()
		{
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Water, creep);
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Ice, creep);
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Fire, creep);
			Assert.IsFalse(creep.State.Frozen);
			Assert.IsTrue(creep.State.Unfreezable);
			Assert.IsTrue(creep.State.Wet);
		}

		[Test, CloseAfterFirstFrame]
		public void ApplyWaterThenIceThenImpactToPaperCreep()
		{
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Water, creep);
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Ice, creep);
			PaperCreepStateChanger.ChangeStatesIfPaperCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Frozen);
		}
	}
}
