using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace Snake.Tests
{
	public class SnakeTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void Init()
		{
			gridSize = 25;
			blockSize = 1.0f / gridSize;
			startPosition = blockSize * (int)(gridSize / 2.0f);
			moveSpeed = 0.15f;
		}

		private int gridSize;
		private float blockSize;
		private float startPosition;
		private float moveSpeed;

		[Test, CloseAfterFirstFrame]
		public void CreateSnakeAtOrigin()
		{
			var snake = new Snake(gridSize, Color.Green);
			Assert.AreEqual(Vector2D.Half, snake.Get<Body>().HeadPosition);
			Assert.AreEqual(new Vector2D(startPosition, startPosition),
				snake.Get<Body>().BodyParts[0].TopLeft);
		}

		[Test, CloseAfterFirstFrame]
		public void SnakeHasTwoParts()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void AddToSnake()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			Assert.AreEqual(2, game.Snake.Get<Body>().BodyParts.Count);
		}

		[Test, CloseAfterFirstFrame]
		public void TouchTopBorder()
		{
			new Game(Resolve<Window>());
			AdvanceTimeAndUpdateEntities(moveSpeed * gridSize / 2);
		}

		[Test, CloseAfterFirstFrame]
		public void TouchLeftBorder()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.MoveLeft();
			AdvanceTimeAndUpdateEntities(moveSpeed * gridSize / 2);
		}

		[Test, CloseAfterFirstFrame]
		public void TouchRightBorder()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.MoveRight();
			AdvanceTimeAndUpdateEntities(moveSpeed * gridSize / 2);
		}

		[Test, CloseAfterFirstFrame]
		public void TouchBottomBorder()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.MoveLeft();
			AdvanceTimeAndUpdateEntities(moveSpeed);
			game.MoveDown();
			AdvanceTimeAndUpdateEntities(moveSpeed * gridSize / 2);
		}

		[Test, CloseAfterFirstFrame]
		public void CheckTrailingVector()
		{
			var snake = new Snake(gridSize, Color.Green);
			Assert.IsTrue(snake.Get<Body>().GetTrailingVector().IsNearlyEqual(new Vector2D(0, blockSize)));
		}

		[Test, CloseAfterFirstFrame]
		public void SnakeCollidingWithItselfWillRestart()
		{
			var game = new Game(Resolve<Window>());
			game.StartGame();
			game.Snake.Get<Body>().AddSnakeBody();
			game.Snake.Get<Body>().AddSnakeBody();
			game.Snake.Get<Body>().AddSnakeBody();
			game.MoveLeft();
			AdvanceTimeAndUpdateEntities(moveSpeed);
			game.MoveDown();
			AdvanceTimeAndUpdateEntities(moveSpeed);
			game.MoveRight();
			AdvanceTimeAndUpdateEntities(moveSpeed);
		}

		[Test, CloseAfterFirstFrame]
		public void DisposeSnake()
		{
			var snake = new Snake(gridSize, Color.Green) { IsActive = false };
			Assert.AreEqual(2, snake.Get<Body>().BodyParts.Count);
			snake.Dispose();
			Assert.Throws<Entity.ComponentNotFound>(() => snake.Get<Body>());
		}

		[Test, CloseAfterFirstFrame]
		public void RemovingAllBodyPartsStillGivesAPosition()
		{
			var snake = new Snake(gridSize, Color.Green);
			snake.Get<Body>().BodyParts.Clear();
			AdvanceTimeAndUpdateEntities();
			Assert.AreEqual(Vector2D.Half, snake.Get<Body>().HeadPosition);
		}
	}
}