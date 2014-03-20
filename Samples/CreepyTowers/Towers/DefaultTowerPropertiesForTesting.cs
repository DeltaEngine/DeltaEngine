using CreepyTowers.Content;

namespace CreepyTowers.Towers
{
	public class DefaultTowerPropertiesForTesting
	{
		public static TowerData DefaultTowerValuesForTesting(TowerType type)
		{
			switch (type)
			{
			case TowerType.Acid:
				defaultTowerData = new TowerData(type, TowerModels.TowerAcidConeJanitorHigh.ToString(),
					AttackType.DirectShot, 4.0f, 1.0f, 35, 230);
				break;

			case TowerType.Fire:
				defaultTowerData = new TowerData(type, TowerModels.TowerFireCandlehulaHigh.ToString(),
					AttackType.Circle, 3.0f, 0.5f, 35, 200);
				break;

			case TowerType.Ice:
				defaultTowerData = new TowerData(type, TowerModels.TowerIceConeIceladyHigh.ToString(),
					AttackType.Cone, 2.0f, 0.5f, 35, 150);
				break;

			case TowerType.Impact:
				defaultTowerData = new TowerData(type,
					TowerModels.TowerImpactRangedKnightscalesHigh.ToString(), AttackType.DirectShot, 1.5f,
					0.6f, 35, 130);
				break;

			case TowerType.Slice:
				defaultTowerData = new TowerData(type, TowerModels.TowerSliceConeKnifeblockHigh.ToString(),
					AttackType.Cone, 1.5f, 0.6f, 35, 120);
				break;

			case TowerType.Water:
				defaultTowerData = new TowerData(type,
					TowerModels.TowerWaterRangedWatersprayHigh.ToString(), AttackType.DirectShot, 3.0f, 1.0f,
					35, 100);
				break;
			}
			return defaultTowerData;
		}

		private static TowerData defaultTowerData;
	}
}