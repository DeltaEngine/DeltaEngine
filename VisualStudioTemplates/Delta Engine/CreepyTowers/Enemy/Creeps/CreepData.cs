using System.Collections.Generic;
using CreepyTowers.Enemy.Bosses;
using CreepyTowers.Towers;

namespace $safeprojectname$.Enemy.Creeps
{
	/// <summary>
	/// Initial data describing a creep, has no current in game values.
	/// </summary>
	public class CreepData : AgentData
	{
		public CreepData(CreepType type, string name, float maxHp, float speed, float resistance,
			int goldReward, Dictionary<TowerType, float> typeDamageModifier)
			: this(name, maxHp, speed, resistance, goldReward, typeDamageModifier)
		{
			Type = type;
		}

		public CreepData(string name, float maxHp, float speed, float resistance, int goldReward,
			Dictionary<TowerType, float> typeDamageModifier)
			: base(name)
		{
			MaxHp = maxHp;
			Speed = speed;
			Resistance = resistance;
			GoldReward = goldReward;
			TypeDamageModifier = typeDamageModifier;
		}

		public float MaxHp { get; private set; }
		public float Speed { get; private set; }
		public float Resistance { get; private set; }
		public int GoldReward { get; private set; }
		public Dictionary<TowerType, float> TypeDamageModifier;

		protected CreepData(BossType type, string name, float maxHp, float speed, float resistance,
			int goldReward, Dictionary<TowerType, float> typeDamageModifier)
			: this(name, maxHp, speed, resistance, goldReward, typeDamageModifier)
		{
			Type = type;
		}
	}
}