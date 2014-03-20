using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.ScreenSpaces;

namespace SideScroller
{
	public class EnemyPlane : Plane
	{
		public EnemyPlane(Vector2D initialPosition)
			: base(ContentLoader.Load<Material>("EnemyPlaneMaterial"), initialPosition)
		{
			Hitpoints = 5;
			verticalDecelerationFactor = 3.0f;
			verticalAccelerationFactor = 1.5f;
			RenderLayer = (int)DefRenderLayer.Player;
			elapsedSinceLastMissile = missileCadenceInverse - 0.2f;
			// ParticleSystemData can very well be loaded by a ContentLoader, unused for simplicity in M5
			//machineGunAndLauncher =
			//	new ParticleSystem(ContentLoader.Load<ParticleSystemData>("MachineGunAndLauncherEnemy"));
			machineGunAndLauncher = new ParticleSystem();

			var machineGunData = new ParticleEmitterData
			{
				ParticleMaterial = ContentLoader.Load<Material>("BulletMaterial"),
				Size = new RangeGraph<Size>(new Size(0.01f, 0.005f), new Size(0.01f, 0.005f)),
				LifeTime = 3.0f,
				StartPosition =
					new RangeGraph<Vector3D>(new Vector3D(0.0f, -0.01f, 0.0f),
						new Vector3D(0.0f, -0.01f, 0.0f)),
				StartRotation =
					new RangeGraph<ValueRange>(new ValueRange(180.0f, 180.0f), new ValueRange(180.0f, 180.0f)),
				StartVelocity = new RangeGraph<Vector3D>(new Vector3D(-0.8f, 0.0f, 0.0f), Vector3D.Zero),
				Acceleration =
					new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.1f, 0.0f), new Vector3D(0.0f, 0.1f, 0.0f)),
				SpawnInterval = 0,
				MaximumNumberOfParticles = 64
			};
			var launcherData = new ParticleEmitterData
			{
				ParticleMaterial = ContentLoader.Load<Material>("RocketMaterial"),
				Size = new RangeGraph<Size>(new Size(0.03f, 0.007f), new Size(0.03f, 0.007f)),
				LifeTime = 3.0f,
				StartPosition = new RangeGraph<Vector3D>(new Vector3D(0.0f, 0.01f, 0.0f), new Vector3D(0.0f, 0.01f, 0.0f)),
				StartRotation =
					new RangeGraph<ValueRange>(new ValueRange(180.0f, 180.0f), new ValueRange(180.0f, 180.0f)),
				StartVelocity = new RangeGraph<Vector3D>(new Vector3D(-0.5f, 0.0f, 0.0f), Vector3D.Zero),
				Acceleration = new RangeGraph<Vector3D>(new Vector3D[] { new Vector3D(0.0f, -0.1f, 0.0f), new Vector3D(0.0f, 0.1f, 0.0f), new Vector3D(0.0f, -0.1f, 0.0f) }),
				SpawnInterval = 0,
				MaximumNumberOfParticles = 8
			};

			machineGunAndLauncher.AttachEmitter(new ParticleEmitter(machineGunData, Vector3D.Zero));
			machineGunAndLauncher.AttachEmitter(new ParticleEmitter(launcherData, Vector3D.Zero));

			machineGunAndLauncher.Position = new Vector3D(initialPosition);
			Add(new Velocity2D(new Vector2D(-0.3f, 0), MaximumSpeed));
			Start<EnemyHandler>();
		}

		public void CheckIfHitAndReact(Vector2D playerShotStartPosition)
		{
			if (Math.Abs(playerShotStartPosition.Y - Center.Y) < 0.1f)
				Hitpoints--;
		}

		private class EnemyHandler : UpdateBehavior
		{
			private static Rectangle CalculateRectAfterMove(Entity entity)
			{
				var pointAfterVerticalMovement =
					new Vector2D(
						entity.Get<Rectangle>().TopLeft.X + entity.Get<Velocity2D>().velocity.X * Time.Delta,
						entity.Get<Rectangle>().TopLeft.Y + entity.Get<Velocity2D>().velocity.Y * Time.Delta);
				return new Rectangle(pointAfterVerticalMovement, entity.Get<Rectangle>().Size);
			}

			private static void MoveEntity(Entity entity, Rectangle rect)
			{
				entity.Set(rect);
			}

			private static float RotationAccordingToVerticalSpeed(Vector2D vel)
			{
				return - 50 * vel.Y / MaximumSpeed;
			}

			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var enemy = entity as EnemyPlane;
					var newRect = CalculateRectAfterMove(enemy);
					MoveEntity(enemy, newRect);
					var velocity2D = enemy.Get<Velocity2D>();
					velocity2D.velocity.Y -= velocity2D.velocity.Y * enemy.verticalDecelerationFactor *
						Time.Delta;
					enemy.Set(velocity2D);
					enemy.Rotation = RotationAccordingToVerticalSpeed(velocity2D.velocity);
					if (enemy.DrawArea.Right < ScreenSpace.Current.Viewport.Left)
						entity.Dispose();
					enemy.FireMissileIfAllowed();
					HitTestToPlayerPlane(enemy);
				}
			}

			private static void HitTestToPlayerPlane(EnemyPlane enemyPlane)
			{
				var playerPlanes = EntitiesRunner.Current.GetEntitiesOfType<PlayerPlane>();
				if (playerPlanes == null)
					return;
				var bullets = enemyPlane.machineGunAndLauncher.AttachedEmitters[0].particles;
				if (bullets != null)
					for (int i = 0; i < bullets.Length; i++)
					{
						if (!bullets[i].IsActive)
							continue;
						if (bullets[i].Position.X < ScreenSpace.Current.Viewport.Left)
							bullets[i].IsActive = false;
						foreach (var player in playerPlanes)
							if (player.DrawArea.Contains(bullets[i].Position.GetVector2D()))
							{
								player.ReceiveAttack();
								bullets[i].IsActive = false;
							}
					}
				var rockets = enemyPlane.machineGunAndLauncher.AttachedEmitters[1].particles;
				if (rockets != null)
					for (int i = 0; i < rockets.Length; i++)
					{
						if (!rockets[i].IsActive)
							continue;
						if (rockets[i].Position.X < ScreenSpace.Current.Viewport.Left)
							rockets[i].IsActive = false;
						foreach (var player in playerPlanes)
							if (player.DrawArea.Contains(rockets[i].Position.GetVector2D()))
							{
								player.ReceiveAttack(5, true);
								rockets[i].IsActive = false;
							}
					}
			}
		}
	}
}