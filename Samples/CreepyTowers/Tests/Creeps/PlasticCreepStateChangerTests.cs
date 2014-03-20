using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class PlasticCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreatePlasticCreep()
		{
			creep = new Creep(CreepType.Plastic, Vector3D.Zero);
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckImpactEffect()
		{
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Impact, creep);
			Assert.IsTrue(creep.State.Slow);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckAcidEffect()
		{
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Acid, creep);
			Assert.IsTrue(creep.State.Melt);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckIceThenWaterEffect()
		{
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Ice, creep);
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Water, creep);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckFireThenWaterEffect()
		{
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Burn);
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Water, creep);
			Assert.IsFalse(creep.State.Burn);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckFireThenIceEffect()
		{
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Burn);
			PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(TowerType.Ice, creep);
			Assert.IsFalse(creep.State.Burn);
		}
	}
}
