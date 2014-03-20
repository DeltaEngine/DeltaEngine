using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using NUnit.Framework;

namespace CreepyTowers.Tests.Creeps
{
	public class CreepStateTests : CreepyTowersGameForTests
	{
		[Test]
		public void TestCreepState()
		{
			var state = new CreepState();
			state.SetVulnerabilityWithValue(TowerType.Ice, 1000f);
			Assert.AreEqual(Vulnerability.Sudden, state.GetVulnerability(TowerType.Ice));
		}

		[Test]
		public void TestFrozenState()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			var state = new CreepState();
			state.Frozen = true;
			state.UpdateStateAndTimers(creep);
			Assert.IsTrue(state.Frozen);
			state.Unfreezable = true;
			state.UpdateStateAndTimers(creep);
			Assert.IsTrue(state.Unfreezable);
		}

		[Test]
		public void TestMeltState()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			var state = new CreepState();
			state.Melt = true;
			state.UpdateStateAndTimers(creep);
			Assert.IsTrue(state.Melt);
		}

		[Test]
		public void TestSlowImmuneState()
		{
			var creep = new Creep(CreepType.Cloth, Vector2D.Zero);
			var state = new CreepState();
			state.Slow = true;
			state.UpdateStateAndTimers(creep);
			Assert.IsTrue(state.Slow);
			state.SlowImmune = true;
			state.UpdateStateAndTimers(creep);
			Assert.IsTrue(state.SlowImmune);
		}
	}
}