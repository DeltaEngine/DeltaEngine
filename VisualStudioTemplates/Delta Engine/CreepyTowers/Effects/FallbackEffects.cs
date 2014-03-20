using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering3D.Particles;

namespace $safeprojectname$.Effects
{
	public class FallbackEffects
	{
		public static ParticleSystem AttackEffect()
		{
			var attackEffect = new ParticleSystem();
			var attackEmitterData = new ParticleEmitterData();
			attackEmitterData.ParticleMaterial = GetFallbackMaterial();
			attackEmitterData.StartVelocity = new RangeGraph<Vector3D>(-Vector3D.UnitY);
			attackEmitterData.SpawnInterval = 0;
			attackEmitterData.MaximumNumberOfParticles = 16;
			attackEmitterData.LifeTime = 0.5f;
			attackEmitterData.Size = new RangeGraph<Size>(new Size(0.2f), new Size(0.4f));
			attackEmitterData.Color = new RangeGraph<Color>(Color.Red, Color.TransparentBlack);
			attackEffect.AttachEmitter(new ParticleEmitter(attackEmitterData, Vector3D.Zero));
			return attackEffect;
		}

		private static Material GetFallbackMaterial()
		{
			var image = ContentLoader.Create<Image>(new ImageCreationData(new Size(1.0f)));
			image.Fill(Color.White);
			return new Material(ContentLoader.Create<Shader>(
				new ShaderCreationData(ShaderFlags.ColoredTextured)), image);
		}

		public static ParticleSystem ProjectileEffect()
		{
			var projectileEffect = new ParticleSystem();
			var projectileEmitterData = new ParticleEmitterData();
			projectileEmitterData.ParticleMaterial = GetFallbackMaterial();
			projectileEmitterData.SpawnInterval = 0.01f;
			projectileEmitterData.LifeTime = 0;
			projectileEmitterData.MaximumNumberOfParticles = 1;
			projectileEmitterData.Color = new RangeGraph<Color>(Color.Red, Color.Red);
			projectileEmitterData.Size = new RangeGraph<Size>(new Size(0.2f), new Size(0.2f));
			projectileEffect.AttachEmitter(new ParticleEmitter(projectileEmitterData, Vector3D.Zero));
			return projectileEffect;
		}

		public static ParticleSystem ImpactEffect()
		{
			var impactEffect = new ParticleSystem();
			var impactEmitterData = new ParticleEmitterData();
			impactEmitterData.ParticleMaterial = GetFallbackMaterial();
			impactEffect.AttachEmitter(new ParticleEmitter(impactEmitterData, Vector3D.Zero));
			return impactEffect;
		}

		public static ParticleSystem CreepDeathEffect()
		{
			var deathEffect = new ParticleSystem();
			var emitterData = new ParticleEmitterData();
			emitterData.ParticleMaterial = GetFallbackMaterial();
			emitterData.Size = new RangeGraph<Size>(new Size(1.0f), new Size(1.0f));
			emitterData.LifeTime = 1.0f;
			emitterData.MaximumNumberOfParticles = 1;
			deathEffect.AttachEmitter(new ParticleEmitter(emitterData, Vector3D.Zero));
			return deathEffect;
		}

		public static ParticleSystem CreepTransformationEffect()
		{
			var transformationEffect = new ParticleSystem();
			var transformationEmitterData = new ParticleEmitterData();
			transformationEmitterData.ParticleMaterial = GetFallbackMaterial();
			transformationEffect.AttachEmitter(new ParticleEmitter(transformationEmitterData,
				Vector3D.Zero));
			return transformationEffect;
		}
	}
}
