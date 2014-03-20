using System.Collections.Generic;
using CreepyTowers.Effects;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Towers;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Rendering3D;

namespace $safeprojectname$.Avatars
{
	/// <summary>
	/// Dragon is a heavy hitter. Special abilities are: 
	/// Long Range: Increases the attack radius for towers by 25%
	/// Aura Cannon: Beam crosses the field bottom-right to top-left through a selected square
	/// Breath of Fire: Fireball affects selected and surrounding squares
	/// </summary>
	public class Dragon : Avatar
	{
		public Dragon()
		{
			IsLocked = false;
			AddTag(Content.Avatars.Dragon.ToString());
		}

		// position should be the exact, unrounded grid position
		public override void PerformAttack(AvatarAttack attack, Vector2D gridPosition)
		{
			if (!Level.Current.IsInsideLevelGrid(gridPosition))
				return;
			if (attack == AvatarAttack.DragonAuraCannon)
				PerformAuraCannonAttack(gridPosition);
			else if (attack == AvatarAttack.DragonBreathOfFire)
				PerformBreathOfFireAttack(gridPosition);
		}

		private static void PerformAuraCannonAttack(Vector2D position)
		{
			new RayAttack(position);
			Vector2D positionUp = position - Vector2D.UnitY;
			List<Creep> creeps = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			foreach (var creep in creeps)
				if (DistanceToLineSquared(creep, position, positionUp) <= AuraCannonRangeSquared)
				{
					creep.ReceiveAttack(TowerType.Fire, AuraCannonDamage);
					EffectLoader.GetAvatarSkillHitEffect(AvatarAttack.DragonAuraCannon).
					             FireBurstAtRelativePosition(creep.Position);
				}
		}

		private const float AuraCannonRange = 1.0f;
		private const float AuraCannonRangeSquared = AuraCannonRange * AuraCannonRange;
		private const float AuraCannonDamage = 10.0f;
		private const float AuraCannonSpeed = 20.0f;

		private static float DistanceToLineSquared(Creep creep, Vector2D lineStart, Vector2D lineEnd)
		{
			var creepPos = creep.Position.GetVector2D();
			var lineDirection = lineEnd - lineStart;
			var lineLengthSquared = lineDirection.LengthSquared;
			if (lineLengthSquared == 0.0)
				return creepPos.DistanceToSquared(lineStart); //ncrunch: no coverage, low float precision
			var startDirection = creepPos - lineStart;
			var linePosition = startDirection.DotProduct(lineDirection) / lineLengthSquared;
			var projection = lineStart + linePosition * lineDirection;
			return creepPos.DistanceToSquared(projection);
		}

		private class RayAttack : HierarchyEntity3D, Updateable
		{
			public RayAttack(Vector2D position) : base(position)
			{
				gridStartPosition = new Vector2D(position.X, Level.Current.Size.Height);
				gridExitPosition = new Vector2D(position.X, 0.0f);
				timeToPassGrid = gridExitPosition.DistanceTo(gridStartPosition) / AuraCannonSpeed;
				AddChild(EffectLoader.GetAvatarSkillEffect(AvatarAttack.DragonAuraCannon));
				Orientation = Quaternion.CreateLookAt(gridExitPosition, gridStartPosition, -Vector3D.UnitZ);
			}

			private readonly float timeToPassGrid;
			private float elapsedTime;
			private readonly Vector2D gridStartPosition;
			private readonly Vector2D gridExitPosition;

			public void Update()
			{
				elapsedTime += Time.Delta;
				var elapsedToImpactTime = elapsedTime / timeToPassGrid;
				Position = gridStartPosition.Lerp(gridExitPosition, elapsedToImpactTime);
				if (elapsedToImpactTime >= 1)
					Dispose();
			}
		}

		private static void PerformBreathOfFireAttack(Vector2D position)
		{
			new Fireball(position);
		}

		private class Fireball : HierarchyEntity3D, Updateable
		{
			public Fireball(Vector2D gridPosition) : base (Vector3D.Zero)
			{
				targetPosition = new Vector3D(gridPosition);
				startPosition = new Vector3D(Randomizer.Current.Get(-1.0f, 1.0f),
					Randomizer.Current.Get(-1.0f, 1.0f), 5.0f);
				Position = startPosition;
				AddChild(EffectLoader.GetAvatarSkillEffect(AvatarAttack.DragonBreathOfFire));
			}

			public void Update()
			{
				elapsedTime += Time.Delta;
				var elapsedToImpactTime = elapsedTime / BreathOfFireTimeTillImpact;
				Position = startPosition.Lerp(targetPosition, elapsedToImpactTime);
				if (elapsedToImpactTime >= 1.0f)
					ExplodeOnImpact();
			}

			private void ExplodeOnImpact()
			{
				EffectLoader.GetAvatarSkillHitEffect(AvatarAttack.DragonBreathOfFire).
				             FireBurstAtRelativePosition(targetPosition);
				var creepsWithinRange = GetCreepsWithinRange(Position.GetVector2D(), BreathOfFireRange);
				foreach (var creep in creepsWithinRange)
					creep.ReceiveAttack(TowerType.Fire, BreathOfFireDamage);
				Dispose();
			}

			private float elapsedTime;
			private readonly Vector3D targetPosition;
			private readonly Vector3D startPosition;
		}

		private const float BreathOfFireRange = 2.0f;
		private const float BreathOfFireDamage = 10.0f;
		public const float BreathOfFireTimeTillImpact = 1.2f;
	}
}