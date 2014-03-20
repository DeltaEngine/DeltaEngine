using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Towers;

namespace CreepyTowers.Enemy.Creeps
{
	public class DefaultCreepPropertiesForTesting
	{
		public static CreepData GetDefaultCreepData(CreepType type)
		{
			switch (type)
			{
			case CreepType.Cloth:
				defaultCreepData = new CreepData(type, CreepModels.CreepCottonMummyHigh.ToString(), 100,
					1.0f, 10.0f, 13, DamageModifierValuesForClothCreep());
				break;

			case CreepType.Glass:
				defaultCreepData = new CreepData(type, CreepModels.CreepGlassHigh.ToString(), 70, 1.3f,
					15.0f, 18, DamageModifierValuesForGlassCreep());
				break;

			case CreepType.Iron:
				defaultCreepData = new CreepData(type, CreepModels.CreepMetalTAxeHigh.ToString(), 120,
					0.7f, 20.0f, 25, DamageModifierValuesForIronCreep());
				break;

			case CreepType.Paper:
				defaultCreepData = new CreepData(type, CreepModels.CreepPaperPaperplaneHigh.ToString(),
					90, 1.5f, 8.0f, 10, DamageModifierValuesForPaperCreep());
				break;

			case CreepType.Plastic:
				defaultCreepData = new CreepData(type, CreepModels.CreepPlasticBottledogHigh.ToString(),
					100, 1.0f, 15.0f, 14, DamageModifierValuesForPlasticCreep());
				break;
					
			case CreepType.Sand:
				defaultCreepData = new CreepData(type, CreepModels.CreepSandSandyHigh.ToString(), 130,
					0.8f, 7.0f, 16, DamageModifierValuesForSandCreep());
				break;

			case CreepType.Wood:
				defaultCreepData = new CreepData(type, CreepModels.CreepWoodScarecrowHigh.ToString(),
					60, 0.9f, 17.0f, 15, DamageModifierValuesForWoodCreep());
				break;
			}

			return defaultCreepData;
		}

		private static CreepData defaultCreepData;

		public static Dictionary<TowerType, float> DamageModifierValuesForClothCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 3.0f },
				{ TowerType.Fire, 3.0f },
				{ TowerType.Water, 1.0f },
				{ TowerType.Impact, 0.25f },
				{ TowerType.Slice, 2.0f },
				{ TowerType.Ice, 0.25f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForGlassCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 0.1f },
				{ TowerType.Fire, 0.5f },
				{ TowerType.Water, 0.1f },
				{ TowerType.Impact, 3.0f },
				{ TowerType.Slice, 0.5f },
				{ TowerType.Ice, 2.0f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForIronCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 1.0f },
				{ TowerType.Fire, 1.0f },
				{ TowerType.Water, 0.5f },
				{ TowerType.Impact, 0.5f },
				{ TowerType.Slice, 0.25f },
				{ TowerType.Ice, 0.1f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForPaperCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 3.0f },
				{ TowerType.Fire, 3.0f },
				{ TowerType.Water, 2.0f },
				{ TowerType.Impact, 0.25f },
				{ TowerType.Slice, 3.0f },
				{ TowerType.Ice, 0.1f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForPlasticCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 1.0f },
				{ TowerType.Fire, 2.0f },
				{ TowerType.Water, 0.1f },
				{ TowerType.Impact, 2.0f },
				{ TowerType.Slice, 0.5f },
				{ TowerType.Ice, 0.1f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForSandCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 0.1f },
				{ TowerType.Fire, 0.1f },
				{ TowerType.Water, 2.0f },
				{ TowerType.Impact, 0.5f },
				{ TowerType.Slice, 0.25f },
				{ TowerType.Ice, 0.1f }
			};
		}

		public static Dictionary<TowerType, float> DamageModifierValuesForWoodCreep()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 2.0f },
				{ TowerType.Fire, 2.0f },
				{ TowerType.Water, 0.1f },
				{ TowerType.Impact, 0.25f },
				{ TowerType.Slice, 2.0f },
				{ TowerType.Ice, 2.0f }
			};
		}
	}
}