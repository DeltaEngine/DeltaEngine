using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Entities;

namespace CreepyTowers.Enemy.Bosses
{
	public class BossState : EnemyState
	{
		public BossState()
		{
			VulnerabilityState = new Vulnerability[NumberOfTowerTypes];
			SetVulnerabilitiesToNormal();
		}

		public override void UpdateStateAndTimers(Agent agent)
		{
			var boss = (Boss)agent;
			if (Melt)
				UpdateMeltState();
			if (Wet)
				UpdateWetState();
			if (Slow)
				UpdateSlowState();
		}

		private void UpdateMeltState()
		{
			MeltTimer += Time.Delta;
			Melt = !(MeltTimer > MaxTimeShort);
			if (Melt)
				return;
			Slow = false; //ncrunch: no coverage start
		}

		//ncrunch: no coverage start

		private void UpdateWetState()
		{
			WetTimer += Time.Delta;
			Wet = !(WetTimer > MaxTimeShort);
			if (Wet)
				return;
			Slow = false; //ncrunch: no coverage start
		}

		//ncrunch: no coverage start

		private void UpdateSlowState()
		{
			SlowTimer += Time.Delta;
			Slow = !(SlowTimer > MaxTimeShort);
			if (Slow)
				return;
			SlowImmune = true; //ncrunch: no coverage start
			SlowImmuneTimer = 0;
		}

		//ncrunch: no coverage end
	}
}