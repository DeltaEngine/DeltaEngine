using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Creeps
{
	public class SandCreepStateChanger
	{
		public static void ChangeStatesIfSandCreep(TowerType damageType, Creep creep) 
		{
			if (damageType == TowerType.Impact)
				SetAffectedByImpact(creep);
			else if (damageType == TowerType.Water)
				SetAffectedByWater(creep);
			else if (damageType == TowerType.Ice)
				SetAffectedByIce(creep);
			else if (damageType == TowerType.Fire)
				SetAffectedByFire(creep);
		}

		private static void SetAffectedByImpact(Creep creep)
		{
			StateChanger.MakeCreepLimitedSlow(creep);
			if (!creep.State.Frozen)
				return;
			StateChanger.CheckChanceForSudden(creep);
		}

		private static void SetAffectedByWater(Creep creep)
		{
			StateChanger.MakeCreepWet(creep);
		}

		private static void SetAffectedByIce(Creep creep)
		{
			if (creep.State.Wet)
				StateChanger.MakeCreepFrozen(creep);
		}

		private static void SetAffectedByFire(Creep creep)
		{
			if (creep.State.Wet)
				creep.State.Wet = false;
			else if (creep.State.Frozen)
			{
				creep.State.Frozen = false;
				StateChanger.MakeCreepUnfreezable(creep);
				StateChanger.MakeCreepWet(creep);
			}
			else
				TransformInGlassCreep(creep);
		}

		private static void TransformInGlassCreep(Creep creep)
		{
			var percentage = creep.GetStatPercentage("Hp");
			creep.ChangeCreepType(CreepType.Glass, percentage);
		}
	}
}