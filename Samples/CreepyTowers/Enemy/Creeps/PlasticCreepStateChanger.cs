using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Creeps
{
	public class PlasticCreepStateChanger
	{
		public static void ChangeStatesIfPlasticCreep(TowerType damageType, Creep creep)
		{
			if (damageType == TowerType.Fire)
				SetAffectedByFire(creep);
			else if (damageType == TowerType.Impact)
				SetAffectedByImpact(creep);
			else if (damageType == TowerType.Acid)
				SetAffectedByAcid(creep);
			else if (damageType == TowerType.Water)
				SetAffectedByWater(creep);
			else if (damageType == TowerType.Ice)
				SetAffectedByIce(creep);
		}

		private static void SetAffectedByFire(Creep creep)
		{
			StateChanger.MakeCreepBurn(creep);
			StateChanger.MakeCreepMelt(creep);
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			StateChanger.MakeCreepSlow(creep);
			StateChanger.MakeCreepEnfeeble(creep);
		}

		private static void SetAffectedByAcid(Creep creep)
		{
			StateChanger.MakeCreepMelt(creep);
		}

		private static void SetAffectedByWater(Creep creep)
		{
			if (!creep.State.Burn)
				return;
			ComeBackToNormal(creep);
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Water);
		}

		private static void ComeBackToNormal(Creep creep)
		{
			creep.State.Burn = false;
			creep.State.Melt = false;
			creep.State.Enfeeble = false;
			StateChanger.MakeCreepSlowImmune(creep);
		}

		private static void SetAffectedByIce(Creep creep)
		{
			if (!creep.State.Burn)
				return;
			ComeBackToNormal(creep);
			StateChanger.MakeCreepResistantToType(creep, TowerType.Ice);
		}
	}
}