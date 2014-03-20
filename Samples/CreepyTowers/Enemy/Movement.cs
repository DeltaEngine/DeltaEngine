using System.Collections.Generic;
using CreepyTowers.Collectables;
using CreepyTowers.Enemy.Bosses;
using CreepyTowers.Enemy.Creeps;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;

namespace CreepyTowers.Enemy
{
	public class Movement : UpdateBehavior
	{
		public override void Update(IEnumerable<Entity> entities)
		{
			var collectables = EntitiesRunner.Current.GetEntitiesOfType<Collectable>();
			foreach (Enemy enemy in entities)
			{
				if (enemy.Path == null)
					continue; //ncrunch: no coverage
				if (enemy.Path.Count > 0)
					enemy.CurrentTarget = enemy.Path[0];
				if (enemy.Position == enemy.FinalTarget)
				{
					enemy.HasReachedExit();
					enemy.Dispose();
					continue;
				}
				var previous = enemy.Direction;
				UpdatePositionAndRotation(enemy);
				CheckForProximityToDamagingCollectable(enemy, collectables);
				CheckIfWeCanRemoveFirstNode(enemy, previous.GetVector2D());
				if (!(enemy is Creep))
					continue;
				((Creep)enemy).RecalculateHitpointBar();
				((Creep)enemy).ModifyStatusIcons();
			}
		}

		private static void UpdatePositionAndRotation(CreepyTowers.Enemy.Enemy enemy)
		{
			var direction = enemy.Direction;
			enemy.Position += direction * (GetVelocity(enemy, enemy.GetStatValue("Speed")) * Time.Delta);
			if (enemy.Model == null)
				return;
			enemy.Model.Position = enemy.Position;
			enemy.RotationZ = MathExtensions.Atan2(direction.X, -direction.Y);
		}

		private static float GetVelocity(CreepyTowers.Enemy.Enemy enemy, float velocity)
		{
			if (enemy is Creep)
			{
				state = ((Creep)enemy).State;
				if (((CreepState)state).Paralysed || ((CreepState)state).Frozen)
					return 0;
			}
			else if (enemy is Boss)
				state = ((Boss)enemy).State;

			if (state.Delayed)
				return velocity / 3;
			if (state.Slow)
				return velocity / 2;
			if (state.Fast)
				return velocity * 2;
			return velocity;
		}

		private static EnemyState state;

		private static void CheckForProximityToDamagingCollectable(CreepyTowers.Enemy.Enemy enemy,
			List<Collectable> collectables)
		{
			foreach (Collectable collectable in collectables)
			{
				if (collectable.Damage == 0.0f ||
					(collectable.Position.DistanceSquared(enemy.Position) > collectable.DamageRangeSquared))
					continue;
				enemy.ReceiveAttack(collectable.DamageType, collectable.Damage);
				collectable.Dispose();
			}
		}

		private static void CheckIfWeCanRemoveFirstNode(CreepyTowers.Enemy.Enemy enemy, Vector2D previous)
		{
			if (enemy.Path.Count <= 0)
				return;
			var current = enemy.CurrentTarget - enemy.Position;
			if (!(previous.DotProduct(current.GetVector2D()) <= 0.0f))
				return;
			enemy.Position = enemy.Path[0];
			enemy.Path.Remove(enemy.Path[0]);
		}
	}
}