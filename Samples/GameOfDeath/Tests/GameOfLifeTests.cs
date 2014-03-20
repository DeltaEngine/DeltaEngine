using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace GameOfDeath.Tests
{
	internal class GameOfLifeTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateSimplestGameOfLifeEverWith1X1()
		{
			game = new GameOfLife(1, 1);
			Assert.AreEqual(0, game.GenerationCount);
			game.Update();
			Assert.AreEqual(1, game.GenerationCount);
		}

		private GameOfLife game;

		[Test]
		public void CreateInvalidGameOfLife()
		{
			Assert.Throws<GameOfLife.SizeMustBeGreaterThanZero>(() => new GameOfLife(0, 0));
		}

		[Test]
		public void OneCellShouldDisappearAfterOneIteration()
		{
			game = new GameOfLife(3, 3);
			game[1, 1] = true;
			game.Update();
			Assert.IsFalse(game[1, 1]);
		}

		[Test]
		public void SingleCellShouldNotSurviveAlone()
		{
			game = new GameOfLife(3, 3);
			game[1, 1] = true;
			Assert.IsFalse(game.ShouldSurvive(1, 1));
		}

		[Test]
		public void BlockShouldSurviveAlone()
		{
			game = new GameOfLife(2, 2);
			CreateBlock(0, 0);
			Assert.IsTrue(game.ShouldSurvive(0, 0));
		}

		[Test]
		public void IfCellIsAliveAndHasTwoNeighborsItShouldSurvive()
		{
			game = new GameOfLife(2, 2);
			game[0, 0] = true;
			game[0, 1] = true;
			game[1, 0] = true;
			Assert.IsTrue(game.ShouldSurvive(0, 0));
		}

		[Test]
		public void IfCellIsAliveAndHasThreeNeighborsItShouldSurvive()
		{
			game = new GameOfLife(2, 2);
			CreateBlock(0, 0);
			Assert.IsTrue(game.ShouldSurvive(0, 0));
		}

		[Test]
		public void IfCellIsNotAliveButHasThreeNeighborsItShouldResurrect()
		{
			game = new GameOfLife(2, 2);
			game[0, 1] = true;
			game[1, 0] = true;
			game[1, 1] = true;
			Assert.IsTrue(game.ShouldSurvive(0, 0));
		}

		[Test]
		public void IfCellIsNotAliveButHasTwoNeighborsItStayDead()
		{
			game = new GameOfLife(2, 2);
			game[0, 1] = true;
			game[1, 0] = true;
			Assert.IsFalse(game.ShouldSurvive(0, 0));
		}

		[Test]
		public void IfCellHasMoreThanThreeNeighboursItDies()
		{
			game = new GameOfLife(3, 3);
			CreateBlock(0, 0);
			CreateBlock(1, 1);
			Assert.IsFalse(game.ShouldSurvive(1, 1));
		}

		/// <summary>
		/// Shapes that survive: http://en.wikipedia.org/wiki/Conway's_Game_of_Life
		/// </summary>
		[Test]
		public void BlockShouldStayAlive()
		{
			game = new GameOfLife(4, 4);
			CreateBlock(1, 1);
			game.Update();
			Assert.IsTrue(game[1, 1]);
			Assert.IsTrue(game[2, 2]);
		}

		private void CreateBlock(int x, int y)
		{
			game[x, y] = true;
			game[x + 1, y] = true;
			game[x, y + 1] = true;
			game[x + 1, y + 1] = true;
		}

		[Test]
		public void LineShouldAlternate()
		{
			game = new GameOfLife(3, 3);
			CreateVerticalLine(1, 0);
			Assert.IsTrue(game[1, 0]);
			Assert.IsFalse(game[0, 1]);
			game.Update();
			Assert.IsFalse(game[1, 0]);
			Assert.IsTrue(game[0, 1]);
			Assert.IsTrue(game[1, 1]);
			Assert.IsTrue(game[2, 1]);
		}

		private void CreateVerticalLine(int x, int y)
		{
			game[x, y] = true;
			game[x, y + 1] = true;
			game[x, y + 2] = true;
		}

		public void ShowGameOfLife()
		{
			game = new GameOfLife(24, 24);
			game.Randomize();

			if (Time.CheckEvery(0.1f))
				game.Update();

			for (int x = 0; x < game.width; x++)
				for (int y = 0; y < game.height; y++)
				{
					float posX = 0.1f + 0.8f * x / game.width;
					float posY = 0.1f + 0.8f * y / game.height;
					Color color = game[x, y] ? Color.White : Color.DarkGray;
					new Ellipse(Rectangle.FromCenter(posX, posY, 0.025f, 0.025f), color);
				}
		}
	}
}