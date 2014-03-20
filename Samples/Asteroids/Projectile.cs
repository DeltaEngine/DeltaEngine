using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.ScreenSpaces;

namespace Asteroids
{
	/// <summary>
	/// Game object representing the projectiles fired by the player
	/// </summary>
	public sealed class Projectile : Entity2D
	{
		//ncrunch: no coverage start
		public Projectile(Vector2D startPosition, float angle)
			: base(Rectangle.FromCenter(startPosition, new Size(.02f)))
		{
			Rotation = angle;
			RenderLayer = (int)AsteroidsRenderLayer.Rockets;
			missileAndTrails =
				new ParticleSystem(ContentLoader.Load<ParticleSystemData>("MissileEffect"));
			//Replacing usage of the ContentLoader we could do the following to dynamically create data:
			//missileAndTrails = new ParticleSystem();
			//var rocketData = new ParticleEmitterData
			//{
			//	ParticleMaterial = ContentLoader.Load<Material>("Missile2D"),
			//	Size = new RangeGraph<Size>(new Size(0.025f, 0.025f), new Size(0.025f, 0.025f)),
			//	LifeTime = 0,
			//	SpawnInterval = 0.001f,
			//	MaximumNumberOfParticles = 1
			//};
			//var trailData = new ParticleEmitterData
			//{
			//	ParticleMaterial = ContentLoader.Load<Material>("Projectile2D"),
			//	Size = new RangeGraph<Size>(new Size(0.02f, 0.03f), new Size(0.02f, 0.04f)),
			//	StartPosition =
			//		new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.02f, 0.0f), new Vector3D(0.0f, 0.02f, 0.0f)),
			//	LifeTime = 2.2f,
			//	SpawnInterval = 0.2f,
			//	MaximumNumberOfParticles = 8
			//};
			//missileAndTrails.AttachEmitter(new ParticleEmitter(trailData, Vector3D.Zero));
			//missileAndTrails.AttachEmitter(new ParticleEmitter(rocketData, Vector3D.Zero));
			//missileAndTrails.AttachedEmitters[0].EmitterData.DoParticlesTrackEmitter = true;
			//missileAndTrails.AttachedEmitters[1].EmitterData.DoParticlesTrackEmitter = true;
			//foreach (var emitter in missileAndTrails.AttachedEmitters)
			//	emitter.EmitterData.StartRotation =
			//		new RangeGraph<ValueRange>(new ValueRange(Rotation, Rotation),
			//			new ValueRange(Rotation, Rotation));
			missileAndTrails.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ, Rotation);
			var data = new SimplePhysics.Data
			{
				Gravity = Vector2D.Zero,
				Velocity =
					new Vector2D(MathExtensions.Sin(angle) * ProjectileVelocity,
						-MathExtensions.Cos(angle) * ProjectileVelocity)
			};
			Add(data);
			Start<MoveAndDisposeOnBorderCollision>();
		}

		private const float ProjectileVelocity = .5f;
		private readonly ParticleSystem missileAndTrails;

		public override void Dispose()
		{
			missileAndTrails.DisposeSystem();
			base.Dispose();
		}

		private class MoveAndDisposeOnBorderCollision : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (Projectile projectile in entities.OfType<Projectile>())
				{
					projectile.missileAndTrails.Position = new Vector3D(projectile.Center);
					projectile.missileAndTrails.Orientation = Quaternion.FromAxisAngle(Vector3D.UnitZ,
						projectile.Rotation);
					projectile.DrawArea = CalculateFutureDrawArea(projectile, Time.Delta);
					if (ObjectHasCrossedScreenBorder(projectile.DrawArea, ScreenSpace.Current.Viewport))
						projectile.Dispose();
				}
			}

			private static Rectangle CalculateFutureDrawArea(Projectile projectile, float deltaT)
			{
				return
					new Rectangle(
						projectile.DrawArea.TopLeft + projectile.Get<SimplePhysics.Data>().Velocity * deltaT,
						projectile.DrawArea.Size);
			}

			private static bool ObjectHasCrossedScreenBorder(Rectangle objectArea, Rectangle borders)
			{
				return (objectArea.Right <= borders.Left || objectArea.Left >= borders.Right ||
					objectArea.Bottom <= borders.Top || objectArea.Top >= borders.Bottom);
			}
		}
	}
}