using CreepyTowers.Effects;
using CreepyTowers.Towers;
using DeltaEngine.Rendering3D.Particles;
using NUnit.Framework;
using DeltaEngine.Platforms;

namespace CreepyTowers.Tests.Content
{
	class EffectTests : TestWithMocksOrVisually
	{
		[Test]
		public void LoadAttackEffect()
		{
			var attackEffect = EffectLoader.GetAttackEffect(TowerType.Slice, AttackType.Cone);
			AssertIsValidEffect(attackEffect);
		}
		
		private static void AssertIsValidEffect(ParticleSystem effect)
		{
			Assert.IsNotNull(effect);
			Assert.IsNotEmpty(effect.AttachedEmitters);
		}

		[Test]
		public void LoadHitEffect()
		{
			var hitEffect = EffectLoader.GetHitEffect(TowerType.Acid);
			AssertIsValidEffect(hitEffect);
		}

		[Test]
		public void LoadProjectileEffect()
		{
			var projectileEffect = EffectLoader.GetProjectileEffect(TowerType.Water, AttackType.DirectShot);
			AssertIsValidEffect(projectileEffect);
		}

		[Test]
		public void CreateAndShowFallbackAttackEffect()
		{
			var attackEffect = FallbackEffects.AttackEffect();
			AssertIsValidEffect(attackEffect);
		}
	}
}
