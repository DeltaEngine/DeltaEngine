using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Drench.Logics;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Logics
{
	public class SinglePlayerLogicTests
	{
		[SetUp]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			logic = new SinglePlayerLogic(BoardTests.Width, BoardTests.Height);
		}

		private Logic logic;

		[Test]
		public void ActivePlayerIsZero()
		{
			Assert.AreEqual(0, logic.ActivePlayer);
		}

		[Test]
		public void GetColor()
		{
			Assert.AreEqual(new Color(0.5f, 1.0f, 1.0f), logic.Board.GetColor(0, 0));
		}

		[Test]
		public void ResetGame()
		{
			logic.Reset();
			Assert.AreEqual(new Color(1.0f, 1.0f, 0.5f), logic.Board.GetColor(0, 0));
		}

		[Test]
		public void MakeMove()
		{
			logic.MakeMove(Color.Red);
			Assert.AreEqual(Color.Red, logic.Board.GetColor(0, 0));
		}

		[Test]
		public void GameIsNotFinishedAfterFirstMove()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			logic.MakeMove(Color.Red);
			Assert.IsFalse(logic.IsGameOver);
			Assert.IsFalse(isGameFinished);
			Assert.AreEqual(1, logic.GetPlayerScore(0));
		}

		[Test]
		public void WhenBoardIsAllTheSameColorGameIsFinished()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			MakeWinningMove();
			Assert.IsTrue(logic.IsGameOver);
			Assert.IsTrue(isGameFinished);
			Assert.AreEqual(BoardTests.Width * BoardTests.Height, logic.GetPlayerScore(0));
		}

		private void MakeWinningMove()
		{
			for (int x = 0; x < BoardTests.Width; x++)
				for (int y = 0; y < BoardTests.Height; y++)
					logic.Board.SetColor(x, y, Color.Blue);
			logic.MakeMove(Color.Red);
		}

		[Test]
		public void MakingMoveAfterGameFinishedThrowsException()
		{
			MakeWinningMove();
			Assert.Throws<Logic.CannotMakeMoveWhenGameIsOver>(() => logic.MakeMove(Color.Red));
		}

		[Test]
		public void PassingDoesNothing()
		{
			logic.Pass();
		}
	}
}