using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Creeps
{
	public class GlassCreepStateChanger
	{
		public static void ChangeStatesIfGlassCreep(TowerType damageType, Creep creep)
		{
			if (damageType == TowerType.Fire)
				SetAffectedByFire(creep);
			else if (damageType == TowerType.Impact)
				SetAffectedByImpact(creep);
		}

		private static void SetAffectedByFire(Creep creep)
		{
			StateChanger.MakeCreepMelt(creep);
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			StateChanger.CheckChanceForShatter(creep);
		}
	}
}