using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class AsteroidTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void FractureAsteroid()
		{
			var asteroid = new Asteroid(new InteractionLogic());
			asteroid.Fracture();
			Assert.IsFalse(asteroid.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void ShowAsteroidsOfSeveralSizemodsAndFracture()
		{
			var gameLogic = new InteractionLogic();
			var largeAsteroid = new Asteroid(gameLogic);
			new Asteroid(gameLogic, 2);
			new Asteroid(gameLogic, 3);
			largeAsteroid.Fracture();
			Assert.IsFalse(largeAsteroid.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void CreateAsteroidAtDefinedPosition()
		{
			var asteroid = new Asteroid(Vector2D.Zero, new InteractionLogic());
			Assert.AreEqual(Vector2D.Zero, asteroid.Center);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckBorderCollisionLeft()
		{
			var asteroid = new Asteroid(new InteractionLogic());
			asteroid.Get<SimplePhysics.Data>().Velocity = new Vector2D(-0.1f, 0.0f);
			asteroid.Center = new Vector2D(-0.5f, 0.5f);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(asteroid.Center.X > 1.0f);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckBorderCollisionRight()
		{
			var asteroid = new Asteroid(new InteractionLogic());
			asteroid.Get<SimplePhysics.Data>().Velocity = new Vector2D(0.1f,0.0f);
			asteroid.Center = new Vector2D(1.5f,0.5f);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(asteroid.Center.X < 0.0f);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckBorderCollisionTop()
		{
			var asteroid = new Asteroid(new InteractionLogic());
			asteroid.Get<SimplePhysics.Data>().Velocity = new Vector2D(0.0f, -0.1f);
			asteroid.Center = new Vector2D(0.5f, -0.5f);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(asteroid.Center.Y > ScreenSpace.Current.Viewport.Bottom);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckBorderCollisionBottom()
		{
			var asteroid = new Asteroid(new InteractionLogic());
			asteroid.Get<SimplePhysics.Data>().Velocity = new Vector2D(0.0f, 0.1f);
			asteroid.Center = new Vector2D(0.5f, 1.5f);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(asteroid.Center.Y < ScreenSpace.Current.Viewport.Top);
		}
	}
}