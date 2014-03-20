using System.Collections.Generic;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;

namespace CreepyTowers.Towers
{
	public class TowerTargetFinder : Entity, Updateable
	{
		public void Update()
		{
			List<Creep> creeps = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			List<Tower> towers = EntitiesRunner.Current.GetEntitiesOfType<Tower>();
			if (creeps.Count < 1 || towers.Count < 1)
				return;
			foreach (Tower tower in towers)
			{
				List<Creep> creepsWithinRange = FindAllCreepsWithinRangeOfTower(tower, creeps);
				Creep target = FindClosestCreepToAttack(creepsWithinRange);
				if (target == null)
					continue;
				tower.AimToTarget(target.Position);
				if (tower.IsOnCooldown)
					continue;
				tower.FireAtCreep(target);
				DamageCreepAndSurroundingCreeps(tower, target, creepsWithinRange);
			}
		}

		private static List<Creep> FindAllCreepsWithinRangeOfTower(Tower tower, List<Creep> creeps)
		{
			float range = tower.GetStatValue("Range");
			float rangeSquared = range * range;
			var targets = new List<Creep>();
			foreach (Creep creep in creeps)
			{
				var distance = (creep.Position - tower.Position).LengthSquared;
				if (distance <= rangeSquared)
					targets.Add(creep);
			}
			return targets;
		}

		private static Creep FindClosestCreepToAttack(List<Creep> creepsWithinRange)
		{
			Creep closestCreep = null;
			float closestDistance = float.MaxValue;
			foreach (Creep creep in creepsWithinRange)
			{
				var distance = (creep.Position - creep.FinalTarget).LengthSquared;
				if (!(distance < closestDistance))
					continue;
				closestCreep = creep;
				closestDistance = distance;
			}
			return closestCreep;
		}

		private static void DamageCreepAndSurroundingCreeps(Tower tower, Creep target, 
			List<Creep> creepsWithinRange)
		{
			if (tower.AttackType == AttackType.Circle)
				DoRadiusAttack(tower, creepsWithinRange);
			else if (tower.AttackType == AttackType.Cone)
				DoConeAttack(tower, target, creepsWithinRange);
			//else if (tower.AttackType == AttackType.DirectShot)
			//	target.ReceiveAttack(tower.Type, tower.Power);
		}

		private static void DoRadiusAttack(Tower tower, List<Creep> creepsWithinRange)
		{
			var type = tower.Type;
			var power = tower.GetStatValue("Power");
			foreach (var creep in creepsWithinRange)
				creep.ReceiveAttack(type, power, RadiusAttackModifier);
		}

		private const float RadiusAttackModifier = 0.3f;

		private static void DoConeAttack(Tower tower, Creep target, List<Creep> creepsWithinRange)
		{
			Vector2D towerPosition = tower.Position.GetVector2D();
			Vector2D creepPosition = target.Position.GetVector2D();
			Vector2D aim = Vector2D.Normalize(creepPosition - towerPosition);
			float edgeDotProduct = GetEdgeDotProduct(creepPosition, towerPosition, aim);
			var power = tower.GetStatValue("Power");
			foreach (Creep creep in creepsWithinRange)
			{
				var position = creep.Position.GetVector2D();
				if (aim.DotProduct(Vector2D.Normalize(position - towerPosition)) >= edgeDotProduct)
					creep.ReceiveAttack(tower.Type, power, ConeAttackModifier);
			}
		}

		private const float ConeAttackModifier = 0.5f;

		private static float GetEdgeDotProduct(Vector2D creepPosition, Vector2D towerPosition, Vector2D aim)
		{
			Vector2D rotatedCreepPosition = creepPosition.RotateAround(towerPosition, HalfConeAngle);
			Vector2D edge = Vector2D.Normalize(rotatedCreepPosition - towerPosition);
			return aim.DotProduct(edge);
		}

		private const int HalfConeAngle = 15;
	}
}