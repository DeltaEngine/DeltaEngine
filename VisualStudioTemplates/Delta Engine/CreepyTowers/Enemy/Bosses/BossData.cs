using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Bosses
{
	public class BossData : CreepData
	{
		public BossData(BossType type, string name, float maxHp, float speed, float resistance,
			int goldReward, Dictionary<TowerType, float> typeDamageModifier)
			: base(type, name, maxHp, speed, resistance, goldReward, typeDamageModifier) {}
	}
}