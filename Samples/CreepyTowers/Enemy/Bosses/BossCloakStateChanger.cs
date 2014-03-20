using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Bosses
{
	public class BossCloakStateChanger
	{
		public static void ChangeStatesIfCloak(TowerType damageType, Boss boss)
		{
			switch (damageType)
			{
			case TowerType.Acid:
				SetAffectedByAcid(boss);
				break;
			case TowerType.Fire:
				SetAffectedByFire(boss);
				break;
			case TowerType.Ice:
				SetAffectedByIce(boss);
				break;
			case TowerType.Impact:
				SetAffectedByImpact(boss);
				break;
			case TowerType.Slice:
				SetAffectedBySlice(boss);
				break;
			case TowerType.Water:
				SetAffectedByWater(boss);
				break;
			}
		}

		private static void SetAffectedByAcid(Boss boss)
		{
			StateChanger.MakeBossMelt(boss);
		}

		private static void SetAffectedByFire(Boss boss)
		{
			if (boss.State.Delayed)
				boss.State.Delayed = false;
			if (boss.State.Slow)
				boss.State.Slow = false;
			boss.State.SetVulnerabilitiesToNormal();
			StateChanger.MakeBossMelt(boss);
		}

		private static void SetAffectedByIce(Boss boss)
		{
			if (boss.State.Wet)
				StateChanger.MakeBossDelayedLimited(boss);
			else
				StateChanger.MakeBossSlowLimited(boss);
		}

		private static void SetAffectedByImpact(Boss boss)
		{
			StateChanger.MakeBossNormalToType(boss, TowerType.Impact);
		}

		private static void SetAffectedBySlice(Boss boss)
		{
			StateChanger.MakeBossNormalToType(boss, TowerType.Slice);
		}

		private static void SetAffectedByWater(Boss boss)
		{
			StateChanger.MakeBossMelt(boss);
		}
	}
}