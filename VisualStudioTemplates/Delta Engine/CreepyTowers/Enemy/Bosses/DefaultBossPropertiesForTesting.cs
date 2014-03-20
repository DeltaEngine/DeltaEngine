using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Bosses
{
	public class DefaultBossPropertiesForTesting
	{
		public static BossData GetDefaultBossData(BossType type)
		{
			switch (type)
			{
			case BossType.Cloak:
				defaultBossData = new BossData(type, BossModels.CreepBossCloak.ToString(), 500, 0.5f, 15.0f,
					100, DamageModifierValuesForBoss());
				break;
			}

			return defaultBossData;
		}

		private static BossData defaultBossData;

		public static Dictionary<TowerType, float> DamageModifierValuesForBoss()
		{
			return new Dictionary<TowerType, float>
			{
				{ TowerType.Acid, 1.0f },
				{ TowerType.Fire, 1.0f },
				{ TowerType.Water, 1.0f },
				{ TowerType.Impact, 1.0f },
				{ TowerType.Slice, 1.0f },
				{ TowerType.Ice, 1.0f }
			};
		}
	}
}