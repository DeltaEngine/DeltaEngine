using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Asteroids.Tests
{
	internal class GameLogicTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void InitGameLogic()
		{
			Resolve<Window>();
			interactionLogic = new InteractionLogic();
			interactionLogic.BeginGame();
		}

		private InteractionLogic interactionLogic;

		[Test, CloseAfterFirstFrame]
		public void AsteroidCreatedWhenTimeReached()
		{
			interactionLogic.BeginGame();
			AdvanceTimeAndUpdateEntities(1.1f);
			Assert.GreaterOrEqual(EntitiesRunner.Current.GetEntitiesOfType<Asteroid>().Count, 1);
		}

		[Test, CloseAfterFirstFrame]
		public void ProjectileAndAsteroidDisposedOnCollision()
		{
			var projectile = new Projectile(new Vector2D(0.5f, 0.55f), 0);
			interactionLogic.CreateAsteroidsAtPosition(Vector2D.Half, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.IsFalse(projectile.IsActive);
		}

		[Test, CloseAfterFirstFrame]
		public void PlayerShipAndAsteroidCollidingResultsInGameOver()
		{
			bool gameOver = false;
			interactionLogic.BeginGame();
			interactionLogic.GameOver += () => { gameOver = true; };
			interactionLogic.Player.Set(new Rectangle(Vector2D.Half, new Size(.05f)));
			interactionLogic.CreateAsteroidsAtPosition(Vector2D.Half, 1, 1);
			AdvanceTimeAndUpdateEntities();
			Assert.IsTrue(gameOver);
		}

		[Test, CloseAfterFirstFrame]
		public void InteractionLogicsIsPauseable()
		{
			Assert.IsTrue(interactionLogic.IsPauseable);
		}
	}
}