using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Creeps
{
	public class WoodCreepStateChanger
	{
		public static void ChangeStatesIfWoodCreep(TowerType damageType, Creep creep)
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
				ChangeStartStatesIfWoodCreep(creep);
			}
			else if (creep.State.Frozen)
			{
				StateChanger.MakeCreepUnfreezable(creep);
				SetWoodCreepWetState(creep);
			}
			else
			{
				StateChanger.MakeCreepFast(creep);
				StateChanger.MakeCreepBurst(creep);
			}
		}

		private static void ChangeStartStatesIfWoodCreep(Creep creep)
		{
			creep.State.SetVulnerabilitiesToNormal();
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Impact);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Slice);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Fire);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Acid);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Ice);
			StateChanger.MakeCreepImmuneToType(creep, TowerType.Water);
		}

		private static void SetWoodCreepWetState(Creep creep)
		{
			StateChanger.MakeCreepWet(creep);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Slice);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Impact);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Ice);
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			if (creep.State.Frozen)
				StateChanger.CheckChanceForSudden(creep);
		}

		private static void SetAffectedByWater(Creep creep)
		{
			if (creep.State.Burst)
			{
				creep.State.Burst = false;
				creep.State.Fast = false;
				ChangeStartStatesIfWoodCreep(creep);
			}
			else
			{
				SetWoodCreepWetState(creep);
				StateChanger.MakeCreepHealing(creep);
			}
		}

		private static void SetAffectedByIce(Creep creep)
		{
			if (creep.State.Wet)
				StateChanger.MakeCreepFrozen(creep);
			else if (creep.State.Burst)
			{
				creep.State.Burst = false;
				creep.State.Burn = false;
				creep.State.Fast = false;
				ChangeStartStatesIfWoodCreep(creep);
			}
		}
	}
}