using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class GlassCreepStateChangerTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateGlassCreep()
		{
			creep = new Creep(CreepType.Glass, Vector3D.Zero);
		}

		private Creep creep;

		[Test, CloseAfterFirstFrame]
		public void CheckForFireTower()
		{
			StateChanger.CheckCreepState(TowerType.Fire, creep);
			Assert.IsTrue(creep.State.Melt);
			Assert.IsTrue(creep.State.Slow);
			Assert.IsTrue(creep.State.Enfeeble);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckForImpactTower()
		{
			GlassCreepStateChanger.ChangeStatesIfGlassCreep(TowerType.Impact, creep);
			Assert.IsFalse(creep.State.Sudden);
		}
	}
}