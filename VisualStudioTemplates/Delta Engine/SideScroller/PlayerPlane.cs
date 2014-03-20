using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class PlayerPlane : Plane
	{
		/// <summary>
		/// Special behavior of the player's plane for use in the side-scrolling shoot'em'up.
		/// Can be controlled in its vertical position, fire shots and make the environment move.
		/// (It is so mighty that, when it flies faster, in truth the world turns faster!)
		/// </summary>
		public PlayerPlane(Vector2D initialPosition)
			: base(ContentLoader.Load<Material>("PlayerPlaneMaterial"), initialPosition)
		{
			Hitpoints = 3;
			verticalDecelerationFactor = 3.0f;
			verticalAccelerationFactor = 1.5f;
			mgCadenceInverse = PlayerMgCadenceInverse;
			missileCadenceInverse = PlayerMissileCadenceInverse;
			RenderLayer = (int)DefRenderLayer.Player;
			// ParticleSystemData can very well be loaded by a ContentLoader, unused for simplicity in M5
			//machineGunAndLauncher =
			//	new ParticleSystem(ContentLoader.Load<ParticleSystemData>("MachineGunAndLauncher"));
			machineGunAndLauncher = new ParticleSystem();
			var machineGunData = new ParticleEmitterData
			{
				ParticleMaterial = ContentLoader.Load<Material>("BulletMaterial"),
				Size = new RangeGraph<Size>(new Size(0.01f, 0.005f), new Size(0.01f, 0.005f)),
				LifeTime = 3.0f,
				StartPosition =
					new RangeGraph<Vector3D>(new Vector3D(0.0f, -0.01f, 0.0f),
						new Vector3D(0.0f, -0.01f, 0.0f)),
				StartVelocity = new RangeGraph<Vector3D>(new Vector3D(0.8f, 0.0f, 0.0f), Vector3D.Zero),
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
				StartVelocity = new RangeGraph<Vector3D>(new Vector3D(0.3f,0.0f,0.0f), Vector3D.Zero),
				Acceleration = new RangeGraph<Vector3D>(new Vector3D[]{new Vector3D(0.0f, -0.1f, 0.0f), new Vector3D(0.0f, 0.1f, 0.0f), new Vector3D(0.0f, -0.1f, 0.0f)}),
				SpawnInterval = 0,
				MaximumNumberOfParticles = 8
			};

			machineGunAndLauncher.AttachEmitter(new ParticleEmitter(machineGunData, Vector3D.Zero));
			machineGunAndLauncher.AttachEmitter(new ParticleEmitter(launcherData, Vector3D.Zero));

			machineGunAndLauncher.Position = new Vector3D(initialPosition);

			Add(new Velocity2D(Vector2D.Zero, MaximumSpeed));
			Start<PlayerMovement>();
			Start<MachineGunFire>();
		}

		protected class PlayerMovement : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var playerPlane = entity as PlayerPlane;
					CHeckIfHittingABorder(playerPlane);
					if (!isHittingABorder)
						CalculateRectAfterMove(playerPlane);
					var velocity2D = playerPlane.Get<Velocity2D>();
					velocity2D.velocity.Y -= velocity2D.velocity.Y * playerPlane.verticalDecelerationFactor *
						Time.Delta;
					playerPlane.Set(velocity2D);
					playerPlane.Rotation = RotationAccordingToVerticalSpeed();
				}
			}

			private static void CalculateRectAfterMove(PlayerPlane entity)
			{
				var pointAfterVerticalMovement = new Vector2D(entity.Get<Rectangle>().TopLeft.X,
					entity.Get<Rectangle>().TopLeft.Y + entity.Get<Velocity2D>().velocity.Y * Time.Delta);
				entity.Set(new Rectangle(pointAfterVerticalMovement, entity.Get<Rectangle>().Size));
			}

			private static void CHeckIfHittingABorder(PlayerPlane entity)
			{
				if (entity.defeated)
					return;
				isHittingABorder = false;
				drawArea = entity.Get<Rectangle>();
				movementSpeed = entity.Get<Velocity2D>();
				CheckStopTopBorder(ScreenSpace.Current.Viewport);
				CheckStopBottomBorder(ScreenSpace.Current.Viewport);
				entity.Set(drawArea);
				entity.Set(movementSpeed);
			}

			private static Rectangle drawArea;
			private static Velocity2D movementSpeed;

			private static void CheckStopTopBorder(Rectangle borders)
			{
				if (drawArea.Top <= borders.Top && movementSpeed.velocity.Y < 0)
				{
					isHittingABorder = true;
					movementSpeed.velocity.Y = 0.02f;
					drawArea.Top = borders.Top;
				}
			}

			private static bool isHittingABorder;

			private static void CheckStopBottomBorder(Rectangle borders)
			{
				if (drawArea.Bottom >= borders.Bottom && movementSpeed.velocity.Y > 0)
				{
					isHittingABorder = true;
					movementSpeed.velocity.Y = -0.02f;
					drawArea.Bottom = borders.Bottom;
				}
			}

			private static float RotationAccordingToVerticalSpeed()
			{
				return 50 * movementSpeed.velocity.Y / MaximumSpeed;
			}
		}

		private class MachineGunFire : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					var playerPlane = entity as PlayerPlane;
					if (playerPlane.IsFireing)
						playerPlane.FireMgShotIfAllowed();
					HitTestToEnemyPlanes(playerPlane);
				}
			}

			private static void HitTestToEnemyPlanes(PlayerPlane playerPlane)
			{
				var enemies = EntitiesRunner.Current.GetEntitiesOfType<EnemyPlane>();

				if (enemies == null)
					return;
				foreach (var enemy in enemies)
					if (playerPlane.DrawArea.IsColliding(playerPlane.Rotation, enemy.DrawArea, enemy.Rotation))
					{
						playerPlane.ReceiveAttack(5);
						enemy.ReceiveAttack(5);
					}

				var bullets = playerPlane.machineGunAndLauncher.AttachedEmitters[0].particles;
				if (bullets != null)
					for (int i = 0; i < bullets.Length; i++)
					{
						if (!bullets[i].IsActive)
							continue;
						if (bullets[i].Position.X > ScreenSpace.Current.Viewport.Right)
							bullets[i].IsActive = false;
						foreach (var enemy in enemies)
							if (enemy.DrawArea.Contains(bullets[i].Position.GetVector2D()))
							{
								enemy.ReceiveAttack();
								bullets[i].IsActive = false;
							}
					}

				var rockets = playerPlane.machineGunAndLauncher.AttachedEmitters[1].particles;
				if (rockets != null)
					for (int i = 0; i < rockets.Length; i++)
					{
						if (!rockets[i].IsActive)
							continue;
						if (rockets[i].Position.X > ScreenSpace.Current.Viewport.Right)
							rockets[i].IsActive = false;
						foreach (var enemy in enemies)
							if (enemy.DrawArea.Contains(rockets[i].Position.GetVector2D()))
							{
								enemy.ReceiveAttack(5, true);
								rockets[i].IsActive = false;
							}
					}
			}
		}

		public bool IsFireing;
		public const float PlayerMgCadenceInverse = 0.01f;
		public const float PlayerMissileCadenceInverse = 2.0f;

		public void AccelerateHorizontally() {}

		public void DecelerateHorizontally() {}
	}
}