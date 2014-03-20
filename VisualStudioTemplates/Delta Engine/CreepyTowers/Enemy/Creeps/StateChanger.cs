using CreepyTowers.Content;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Core;

namespace $safeprojectname$.Enemy.Creeps
{
	public class StateChanger
	{
		public static void MakeCreepDelayed(Creep creep)
		{
			if(creep.State.Delayed)
				return;
			creep.State.Delayed = true;
			creep.State.DelayedTimer = 0;
			creep.TimeIcon.Material = ContentLoader.Load<Material>(CreepStates.SlowMat.ToString());
		}

		public static void MakeCreepBurn(Creep creep)
		{
			creep.State.Burn = true;
			creep.State.BurnTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.BurningMat.ToString());
		}

		public static void MakeCreepBurst(Creep creep)
		{
			creep.State.Burst = true;
			creep.State.BurstTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.BurningMat.ToString());
		}

		public static void MakeCreepUnfreezable(Creep creep)
		{
			creep.State.Unfreezable = true;
			creep.State.UnfreezableTimer = 0;
			creep.State.Frozen = false;
			creep.State.Paralysed = false;
			creep.StateIcon.Material = creep.EmptyMaterial;
		}

		public static void MakeCreepSlowImmune(Creep creep)
		{
			creep.State.SlowImmune = true;
			creep.State.SlowImmuneTimer = 0;
			creep.State.Slow = false;
			creep.StateIcon.Material = creep.EmptyMaterial;
		}

		public static void MakeCreepFrozen(Creep creep)
		{
			if (creep.State.Unfreezable)
				return;
			creep.State.Frozen = true;
			creep.State.FrozenTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.FrozenMat.ToString());
			MakeCreepParalysed(creep);
			MakeCreepResistantToType(creep, TowerType.Slice);
			creep.State.Wet = false;
			MakeCreepResistantToType(creep, TowerType.Water);
			MakeCreepVulnerableToType(creep, TowerType.Impact);
			creep.State.Burst = false;
			creep.State.Burn = false;
			MakeCreepImmuneToType(creep, TowerType.Fire);
		}

		public static void MakeCreepParalysed(Creep creep)
		{
			creep.State.Paralysed = true;
			creep.State.ParalysedTimer = 0;
		}

		public static void MakeCreepResistantToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.Resistant);
		}

		public static void MakeCreepVulnerableToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.Vulnerable);
		}

		public static void MakeCreepImmuneToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.Immune);
		}

		public static void MakeCreepWeakToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.Weak);
		}

		public static void MakeCreepFast(Creep creep)
		{
			creep.State.Fast = true;
			creep.State.FastTimer = 0;
			creep.TimeIcon.Material = creep.EmptyMaterial;
		}

		public static void MakeCreepMelt(Creep creep)
		{
			MakeCreepEnfeeble(creep);
			MakeCreepLimitedSlow(creep);
			creep.State.Melt = true;
			creep.State.MeltTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.MeltMat.ToString());
		}

		public static void MakeCreepEnfeeble(Creep creep)
		{
			creep.State.Enfeeble = true;
			creep.State.EnfeebleTimer = 0;
		}

		public static void MakeCreepSlow(Creep creep)
		{
			if (creep.State.SlowImmune || creep.State.Slow)
				return;
			creep.State.Slow = true;
			creep.State.SlowTimer = -1;
			creep.TimeIcon.Material = ContentLoader.Load<Material>(CreepStates.SlowMat.ToString());
		}

		public static void MakeCreepLimitedSlow(Creep creep)
		{
			if (creep.State.SlowImmune || creep.State.Slow)
				return;
			creep.State.Slow = true;
			creep.State.SlowTimer = 0;
			creep.TimeIcon.Material = ContentLoader.Load<Material>(CreepStates.SlowMat.ToString());
		}

		public static void MakeCreepRust(Creep creep)
		{
			creep.State.Rust = true;
			creep.State.RustTimer = 0;
			MakeCreepEnfeeble(creep);
			MakeCreepSlow(creep);
		}

		public static void MakeCreepHealing(Creep creep)
		{
			creep.State.Healing = true;
		}

		public static void MakeCreepWet(Creep creep)
		{
			MakeCreepLimitedSlow(creep);
			if (creep.State.Frozen)
				return;
			creep.State.Wet = true;
			creep.State.WetTimer = 0;
			creep.StateIcon.Material = ContentLoader.Load<Material>(CreepStates.WetMat.ToString());
			MakeCreepResistantToFire(creep);
		}

		private static void MakeCreepResistantToFire(Creep creep)
		{
			creep.State.Burst = false;
			creep.State.Burn = false;
			MakeCreepResistantToType(creep, TowerType.Fire);
		}

		public static void CheckChanceForSudden(Creep creep)
		{
			CheckChance(creep);
		}

		private static bool CheckChance(Creep creep)
		{
			var chanceForShatter = Randomizer.Current.Get(0, 100);
			if (chanceForShatter >= 15)
				return false;
			creep.AdjustStat(new StatAdjustment("Hp", "", -creep.GetStatValue("Hp")));
			return true;
		}

		public static void CheckChanceForShatter(Creep creep)
		{
			if (CheckChance(creep))
				creep.Shatter(); //ncrunch: no coverage;
		}

		public static void MakeCreepHardBoiledToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.HardBoiled);
		}

		public static void MakeCreepNormalToType(Creep creep, TowerType type)
		{
			creep.State.SetVulnerability(type, Vulnerability.Normal);
		}

		public static void CheckCreepState(TowerType type, Creep creep)
		{
			if (creep.Type == CreepType.Cloth)
				ClothCreepStateChanger.ChangeStatesIfClothCreep(type, creep);
			else if (creep.Type == CreepType.Sand)
				SandCreepStateChanger.ChangeStatesIfSandCreep(type, creep);
			else if (creep.Type == CreepType.Glass)
				GlassCreepStateChanger.ChangeStatesIfGlassCreep(type, creep);
			else if (creep.Type == CreepType.Wood)
				WoodCreepStateChanger.ChangeStatesIfWoodCreep(type, creep);
			else if (creep.Type == CreepType.Plastic)
				PlasticCreepStateChanger.ChangeStatesIfPlasticCreep(type, creep);
			else if (creep.Type == CreepType.Iron)
				IronCreepStateChanger.ChangeStatesIfIronCreep(type, creep);
			else if (creep.Type == CreepType.Paper)
				PaperCreepStateChanger.ChangeStatesIfPaperCreep(type, creep);
		}
	}
}