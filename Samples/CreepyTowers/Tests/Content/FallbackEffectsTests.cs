using CreepyTowers.Effects;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering3D.Cameras;
using NUnit.Framework;

namespace CreepyTowers.Tests.Content
{
	public class FallbackEffectsTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void SetUp()
		{
			camera = Camera.Use<LookAtCamera>();
			camera.Position = -Vector3D.UnitY;
		}

		private Camera camera;
		
		[Test]
		public void DrawAttackEffect()
		{
			var effect = FallbackEffects.AttackEffect();
			effect.FireBurstOfAllEmitters();
		}

		[Test]
		public void DrawProjectileEffect()
		{
			var effect = FallbackEffects.ProjectileEffect();
			effect.FireBurstOfAllEmitters();
		}

		[Test]
		public void DrawImpactEffect()
		{
			var effect = FallbackEffects.ImpactEffect();
			effect.FireBurstOfAllEmitters();
		}

		[Test]
		public void DrawCreepPerishEffect()
		{
			var effect = FallbackEffects.CreepDeathEffect();
			effect.FireBurstOfAllEmitters();
		}

		[Test]
		public void DrawCreepTransformationEffect()
		{
			var effect = FallbackEffects.CreepTransformationEffect();
			effect.FireBurstOfAllEmitters();
		}
	}
}