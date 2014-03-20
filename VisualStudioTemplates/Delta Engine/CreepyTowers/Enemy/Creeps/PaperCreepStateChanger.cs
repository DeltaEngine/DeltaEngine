using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Creeps
{
	public class PaperCreepStateChanger
	{
		public static void ChangeStatesIfPaperCreep(TowerType damageType, Creep creep)
		{
			if (damageType == TowerType.Fire)
				SetAffectedByFire(creep);
			else if (damageType == TowerType.Impact)
				SetAffectedByImpact(creep);
			else if (damageType == TowerType.Water)
				SetAffectedByWater(creep);
			else if (damageType == TowerType.Ice)
				SetAffectedByIce(creep);
		}

		private static void SetAffectedByFire(Creep creep)
		{
			if (creep.State.Wet)
			{
				creep.State.Wet = false;
				ChangeStartStatesIfPaperCreep(creep);
			}
			else if (creep.State.Frozen)
			{
				creep.State.Frozen = false;
				StateChanger.MakeCreepUnfreezable(creep);
				SetPaperCreepWetState(creep);
			}
			else
			{
				StateChanger.MakeCreepFast(creep);
				StateChanger.MakeCreepBurst(creep);
			}
		}

		private static void ChangeStartStatesIfPaperCreep(Creep creep)
		{
			creep.State.SetVulnerabilitiesToNormal();
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Impact);
			StateChanger.MakeCreepImmuneToType(creep, TowerType.Ice);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Water);
			StateChanger.MakeCreepVulnerableToType(creep, TowerType.Slice);
			StateChanger.MakeCreepVulnerableToType(creep, TowerType.Fire);
			StateChanger.MakeCreepVulnerableToType(creep, TowerType.Acid);
		}

		private static void SetPaperCreepWetState(Creep creep)
		{
			StateChanger.MakeCreepWet(creep);
			StateChanger.MakeCreepImmuneToType(creep, TowerType.Slice);
			StateChanger.MakeCreepImmuneToType(creep, TowerType.Impact);
			StateChanger.MakeCreepNormalToType(creep, TowerType.Ice);
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			StateChanger.MakeCreepSlow(creep);
			if (creep.State.Frozen)
				StateChanger.CheckChanceForSudden(creep);
		}

		private static void SetAffectedByWater(Creep creep)
		{
			SetPaperCreepWetState(creep);
		}

		private static void SetAffectedByIce(Creep creep)
		{
			if (creep.State.Wet)
				StateChanger.MakeCreepFrozen(creep);
			creep.State.Burst = false;
			creep.State.Burn = false;
			creep.State.Fast = false;
		}
	}
}