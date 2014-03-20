using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Creeps
{
	public class ClothCreepStateChanger
	{
		public static void ChangeStatesIfClothCreep(TowerType damageType, Creep creep)
		{
			switch (damageType)
			{
			case TowerType.Ice:
				SetAffectedByIce(creep);
				break;
			case TowerType.Impact:
				SetAffectedByImpact(creep);
				break;
			case TowerType.Water:
				SetAffectedByWater(creep);
				break;
			case TowerType.Acid:
				SetAffectedByAcid(creep);
				break;
			case TowerType.Fire:
				SetAffectedByFire(creep);
				break;
			}
		}

		private static void SetAffectedByIce(Creep creep)
		{
			if (creep.State.Burst)
			{
				creep.State.Fast = false;
				creep.State.Burst = false;
			}
			else
			{
				StateChanger.MakeCreepSlow(creep);
				if (creep.State.Wet)
					StateChanger.MakeCreepFrozen(creep);
				creep.State.Burst = false;
				creep.State.Burn = false;
				creep.State.Fast = false;
			}
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			StateChanger.MakeCreepLimitedSlow(creep);
			if (creep.State.Frozen)
				StateChanger.CheckChanceForSudden(creep);
		}

		private static void SetAffectedByWater(Creep creep)
		{
			if (creep.State.Fast)
				creep.State.Fast = false;
			SetClothCreepWetState(creep);
		}

		private static void SetClothCreepWetState(Creep creep)
		{
			StateChanger.MakeCreepWet(creep);
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Impact);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Ice);
			StateChanger.MakeCreepNormalToType(creep, TowerType.Slice);
		}

		private static void SetAffectedByAcid(Creep creep)
		{
			StateChanger.MakeCreepEnfeeble(creep);
		}

		private static void SetAffectedByFire(Creep creep)
		{
			if (creep.State.Wet)
			{
				creep.State.Wet = false;
				ChangeStartStatesIfClothCreep(creep);
			}
			else if (creep.State.Frozen)
			{
				creep.State.Frozen = false;
				StateChanger.MakeCreepUnfreezable(creep);
				SetClothCreepWetState(creep);
			}
			else
			{
				if (creep.State.Slow)
					StateChanger.MakeCreepSlowImmune(creep);
				else
					StateChanger.MakeCreepFast(creep);
				StateChanger.MakeCreepBurst(creep);
			}
		}

		private static void ChangeStartStatesIfClothCreep(Creep creep)
		{
			creep.State.SetVulnerabilitiesToNormal();
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Ice);
			StateChanger.MakeCreepWeakToType(creep, TowerType.Slice);
			StateChanger.MakeCreepHardBoiledToType(creep, TowerType.Impact);
			StateChanger.MakeCreepVulnerableToType(creep, TowerType.Acid);
			StateChanger.MakeCreepVulnerableToType(creep, TowerType.Fire);
		}
	}
}