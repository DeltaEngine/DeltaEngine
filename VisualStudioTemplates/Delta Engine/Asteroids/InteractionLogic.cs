using System;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering3D.Particles;

namespace $safeprojectname$
{
	public class InteractionLogic : Entity, Updateable
	{
		public InteractionLogic()
		{
			explosionData = ContentLoader.Load<ParticleEmitterData>("ExplosionEffectEmitter0");
			//Instead of making use of content loading we could create those data dynamically like this:
			//explosionData = new ParticleEmitterData
			//{
			//	ParticleMaterial = ContentLoader.Load<Material>("ExplosionAnimated2D"),
			//	Size = new RangeGraph<Size>(new Size(0.2f, 0.2f), new Size(0.2f,0.2f)),
			//	LifeTime = 0,
			//	SpawnInterval = 0.001f,
			//	MaximumNumberOfParticles = 1
			//};
			IncreaseScore += i => { };
		}

		public void BeginGame()
		{
			IsActive = true;
			gameRunning = true;
			Player = new PlayerShip();
		}

		public PlayerShip Player { get; private set; }
		private readonly ParticleEmitterData explosionData;
		private bool gameRunning;

		public void CreateRandomAsteroids(int howMany, int sizeMod = 1)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount++)
			{
				new Asteroid(this, sizeMod);
			}
		}

		public void CreateAsteroidsAtPosition(Vector2D position, int sizeMod = 1, int howMany = 2)
		{
			for (int asteroidCount = 0; asteroidCount < howMany; asteroidCount++)
			{
				var asteroid = new Asteroid(this, sizeMod);
				asteroid.SetWithoutInterpolation(new Rectangle(position, asteroid.DrawArea.Size));
			}
		}

		public void IncrementScore(int increase)
		{
			IncreaseScore.Invoke(increase);
		}

		private void CheckAsteroidCollisions()
		{
			foreach (var asteroid in EntitiesRunner.Current.GetEntitiesOfType<Asteroid>())
			{
				//ncrunch: no coverage start
				foreach (var projectile in EntitiesRunner.Current.GetEntitiesOfType<Projectile>())
					if (ObjectsInHitRadius(projectile, asteroid, 0.1f / asteroid.sizeModifier))
					{
						CreateExplosionEmitter(projectile.Center, 0.7f);
						projectile.Dispose();
						asteroid.Fracture();
					} //ncrunch: no coverage end
				if (!Player.IsActive ||
					!ObjectsInHitRadius(Player, asteroid, 0.06f / asteroid.sizeModifier))
					continue;
				Player.Dispose();
				CreateExplosionEmitter(Player.Center, 0.5f);
				if (GameOver != null)
					GameOver();
			}
		}

		private void CreateExplosionEmitter(Vector2D position, float length)
		{
			var explosionEmitter = new ParticleEmitter(explosionData, position) { RenderLayer = 10 };
			explosionEmitter.DisposeAfterSeconds(length);
		}

		private static bool ObjectsInHitRadius(Entity2D entityAlpha, Entity2D entityBeta,
			float radius)
		{
			return entityAlpha.DrawArea.Center.DistanceTo(entityBeta.DrawArea.Center) < radius;
		}

		private void CreateNewAsteroidIfNecessary()
		{
			if (GlobalTime.Current.Milliseconds - 1000 > timeLastNewAsteroid &&
				EntitiesRunner.Current.GetEntitiesOfType<Asteroid>().Count <= MaximumAsteroids)
			{
				CreateRandomAsteroids(1);
				timeLastNewAsteroid = GlobalTime.Current.Milliseconds;
			}
		}

		private const int MaximumAsteroids = 10;
		private float timeLastNewAsteroid;
		public event Action GameOver;
		public event Action<int> IncreaseScore;

		public void Restart()
		{
			Dispose();
			BeginGame();
			gameRunning = true;
		}

		public override void Dispose()
		{
			foreach (Asteroid asteroid in EntitiesRunner.Current.GetEntitiesOfType<Asteroid>())
				asteroid.Dispose(); //ncrunch: no coverage 
			foreach (Projectile projectile in EntitiesRunner.Current.GetEntitiesOfType<Projectile>())
				projectile.Dispose(); //ncrunch: no coverage
			foreach (PlayerShip playerShip in EntitiesRunner.Current.GetEntitiesOfType<PlayerShip>())
				playerShip.Dispose();
			base.Dispose();
		}

		public void Update()
		{
			if(!gameRunning)
				return;
			CheckAsteroidCollisions();
			CreateNewAsteroidIfNecessary();
		}

		public void PauseUpdate()
		{
			gameRunning = false;
		}
	}
}