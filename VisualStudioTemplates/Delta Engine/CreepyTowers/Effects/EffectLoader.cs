using System;
using CreepyTowers.Avatars;
using CreepyTowers.Content;
using CreepyTowers.Towers;
using DeltaEngine.Content;
using DeltaEngine.Rendering3D.Particles;

namespace $safeprojectname$.Effects
{
	/// <summary>
	/// Loads particle effects for tower attacks
	/// </summary>
	public class EffectLoader
	{
		public static ParticleSystem GetAttackEffect(TowerType towerType, AttackType attackType)
		{
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>(towerType + attackType.ToString() + 
					EffectType.Attack);
				return new ParticleSystem(effectData);
			}
			catch //ncrunch: no coverage start, MockContentLoader will have the above lines always pass
			{
				return FallbackEffects.AttackEffect();
			} //ncrunch: no coverage end
		}

		public static ParticleSystem GetProjectileEffect(TowerType towerType, AttackType attackType)
		{
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>(towerType + attackType.ToString() +
						EffectType.Projectile);
				var effect = new ParticleSystem(effectData);
				foreach (var emitter in effect.AttachedEmitters)
					emitter.EmitterData.DoParticlesTrackEmitter = true;
				return effect;
			}
			catch //ncrunch: no coverage start
			{
				return FallbackEffects.ProjectileEffect();
			} //ncrunch: no coverage end
		}

		public static ParticleSystem GetHitEffect(TowerType attackType)
		{
			if (HitEffects[(int)attackType] != null)
				return HitEffects[(int)attackType];
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>(attackType.ToString() + 
					EffectType.Hit);
				return HitEffects[(int)attackType] = new ParticleSystem(effectData);
			}
			catch //ncrunch: no coverage start
			{
				return HitEffects[(int)attackType] = FallbackEffects.AttackEffect();
			} //ncrunch: no coverage end
		}

		private static readonly ParticleSystem[] HitEffects =
			new ParticleSystem[Enum.GetValues(typeof(TowerType)).Length];

		public static ParticleSystem GetCreepDeathEffect()
		{
			if (deathEffect != null)
				return deathEffect;
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>("Creep" + EffectType.Death);
				return deathEffect = new ParticleSystem(effectData);
			}
			catch //ncrunch: no coverage start
			{
				return deathEffect = FallbackEffects.CreepDeathEffect();
			} //ncrunch: no coverage end
		}

		private static ParticleSystem deathEffect;

		public static ParticleSystem GetAvatarSkillEffect(AvatarAttack avatarAttack)
		{
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>(avatarAttack + "Skill");
				return new ParticleSystem(effectData);
			}
			catch //ncrunch: no coverage start
			{
				return new ParticleSystem();
			} //ncrunch: no coverage end
		}

		public static ParticleSystem GetAvatarSkillHitEffect(AvatarAttack avatarAttack)
		{
			if (AvatarHitEffects[(int)avatarAttack] != null)
				return AvatarHitEffects[(int)avatarAttack];
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>(avatarAttack + "SkillHit");
				return AvatarHitEffects[(int)avatarAttack] = new ParticleSystem(effectData);
			}
			catch //ncrunch: no coverage start
			{
				return AvatarHitEffects[(int)avatarAttack] = new ParticleSystem();
			} //ncrunch: no coverage end
		}

		private static readonly ParticleSystem[] AvatarHitEffects =
			new ParticleSystem[Enum.GetValues(typeof(AvatarAttack)).Length];

		public static ParticleSystem GetTrailMarkerEffect()
		{
			if (trailMarker != null)
				return trailMarker;
			try
			{
				var effectData = ContentLoader.Load<ParticleSystemData>("DefaultTrail");
				return trailMarker = new ParticleSystem(effectData);
			}
			catch
			{
				return new ParticleSystem();
			}
		}

		private static ParticleSystem trailMarker;
	}
}