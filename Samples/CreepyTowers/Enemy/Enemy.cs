using System;
using System.Collections.Generic;
using CreepyTowers.Effects;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;

namespace CreepyTowers.Enemy
{
	public abstract class Enemy : Agent
	{
		protected Enemy(Vector3D position, float rotationZ = 0.0f)
		{
			CurrentTarget = Position = position;
			RotationZ = rotationZ;
			ScaleFactor = 1.0f;
			Path = new List<Vector2D>();
			Start<Movement>();
		}

		public Vector3D CurrentTarget { get; set; }
		public Vector3D FinalTarget { get; set; }
		public List<Vector2D> Path { get; set; }

		protected override void InitializeModel()
		{
			ScaleChanged += RescaleModel;
			OrientationChanged += RotateModel;
		}

		private void RescaleModel()
		{
			Model.Scale = Scale;
		}

		private void RotateModel()
		{
			Model.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, RotationZ);
		}

		public Vector3D Direction
		{
			get
			{
				if (Path != null && Path.Count > 0)
				{
					CurrentTarget = Path[0];
					return Vector3D.Normalize(CurrentTarget - Position);
				}
				return Vector3D.Zero;
			}
		}

		public virtual void ReceiveAttack(TowerType damageType, float rawDamage,
			float typeInteraction = 1.0f)
		{
			if (!IsActive)
				return;
			UpdateDamageState(damageType);
			SpawnHitSparks(damageType);
			float resistance = GetStatValue("Resistance");
			var damage = CalculateDamage(rawDamage, resistance, typeInteraction);
			AdjustStat(new StatAdjustment("Hp", "", -damage));
			if (GetStatValue("Hp") <= 0.0f)
				Die();
		}

		public abstract void UpdateDamageState(TowerType damageType);

		public virtual void SpawnHitSparks(TowerType damageType)
		{
			var hitEffect = EffectLoader.GetHitEffect(damageType);
			hitEffect.FireBurstAtRelativePosition(Position);
		}

		public virtual float CalculateDamage(float rawDamage, float resistance,
			float interactionEffect)
		{
			return (rawDamage - resistance) * interactionEffect;
		}

		public virtual void Die()
		{
			DisplayDieEffect();
			if (IsDead != null)
				IsDead(); //ncrunch: no coverage
			RemoveEvents();
			Dispose();
		}

		public virtual void DisplayDieEffect()
		{
			var deathParticles = EffectLoader.GetCreepDeathEffect();
			deathParticles.FireBurstAtRelativePosition(Position);
		}

		public event Action IsDead;

		private void RemoveEvents()
		{
			IsDead = null;
			ReachedExit = null;
		}

		public virtual void HasReachedExit()
		{
			if (ReachedExit != null)
				ReachedExit();
			Dispose();
		}

		public event Action ReachedExit;
		protected abstract void RestartStatsAndState(float percentage, AgentData data);
	}
}