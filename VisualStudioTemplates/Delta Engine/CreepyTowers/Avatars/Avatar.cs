using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace $safeprojectname$.Avatars
{
	public abstract class Avatar : Entity
	{
		protected Avatar()
		{
			AttackFrequencyMultiplier = 1.0f;
			RangeMultiplier = 1.0f;
			PowerMultiplier = 1.0f;
		}

		public float AttackFrequencyMultiplier { get; protected set; }
		public float RangeMultiplier { get; protected set; }
		public float PowerMultiplier { get; protected set; }
		public int Xp { get; set; }
		public int ProgressLevel { get; set; }
		public AvatarAttack ActivatedSpecialAttack { get; set; }
		public bool IsSelected { get; set; }
		public bool SpecialAttackAIsActivated { get; set; }
		public bool SpecialAttackBIsActivated { get; set; }
		public bool IsLocked { get; set; }

		public abstract void PerformAttack(AvatarAttack attack, Vector2D position);

		protected static List<Creep> GetCreepsWithinRange(Vector2D position, float range)
		{
			List<Creep> creeps = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			var creepsWithinRange = new List<Creep>();
			var position3D = new Vector3D(position);
			var rangeSquared = range * range;
			foreach (var creep in creeps)
				if (creep.Position.DistanceSquared(position3D) <= rangeSquared)
					creepsWithinRange.Add(creep);
			return creepsWithinRange;
		}

		public void Reset()
		{
			SpecialAttackAIsActivated = false;
			SpecialAttackBIsActivated = false;
			IsSelected = false;
		}
	}
}