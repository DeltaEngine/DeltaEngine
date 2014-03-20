using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Bosses
{
	public class StateChanger
	{
		public static void MakeBossSlowLimited(Boss boss)
		{
			if (boss.State.Slow)
				return;
			boss.State.Slow = true;
			boss.State.SlowTimer = 0;
		}

		public static void MakeBossMelt(Boss boss)
		{
			MakeBossEnfeeble(boss);
			MakeBossSlowLimited(boss);
			boss.State.Melt = true;
			boss.State.MeltTimer = 0;
		}

		private static void MakeBossEnfeeble(Boss boss)
		{
			boss.State.Enfeeble = true;
			boss.State.EnfeebleTimer = 0;
		}

		public static void MakeBossNormalToType(Boss boss, TowerType type)
		{
			boss.State.SetVulnerability(type, Vulnerability.Normal);
		}

		public static void MakeCreepWet(Boss boss)
		{
			MakeBossSlowLimited(boss);
			boss.State.Wet = true;
			boss.State.WetTimer = 0;
			MakeBossResistantToFire(boss);
		}

		private static void MakeBossResistantToFire(Boss boss)
		{
			MakeBossResistantToType(boss, TowerType.Fire);
		}

		public static void MakeBossResistantToType(Boss boss, TowerType type)
		{
			boss.State.SetVulnerability(type, Vulnerability.Resistant);
		}

		public static void MakeBossDelayedLimited(Boss boss)
		{
			if (boss.State.Delayed)
				return;
			boss.State.Delayed = true;
			boss.State.DelayedTimer = 0;
		}

		public static void CheckBossState(TowerType type, Boss boss)
		{
			if (boss.Type == BossType.Cloak)
				BossCloakStateChanger.ChangeStatesIfCloak(type, boss);
		}
	}
}