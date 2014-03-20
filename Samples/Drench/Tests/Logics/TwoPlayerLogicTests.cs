using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using Drench.Logics;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Logics
{
	public class TwoPlayerLogicTests
	{
		[SetUp]
		public void SetUp()
		{
			Randomizer.Use(new FixedRandom(new[] { 0.1f, 0.6f, 0.7f, 0.2f }));
			logic = new TwoHumanLogic(BoardTests.Width, BoardTests.Height);
		}

		private Logic logic;

		[Test]
		public void FirstMoveChangesTopLeftColorAndActivePlayer()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			logic.MakeMove(Color.Red);
			Assert.AreEqual(Color.Red, logic.Board.GetColor(0, 0));
			Assert.IsFalse(isGameFinished);
			Assert.AreEqual(1, logic.ActivePlayer);
			Assert.AreEqual(1, logic.GetPlayerScore(0));
		}

		[Test]
		public void SecondMoveChangesBottomRightColorAndActivePlayer()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			logic.MakeMove(Color.Red);
			logic.MakeMove(Color.Green);
			Assert.AreEqual(Color.Green,
				logic.Board.GetColor(BoardTests.Width - 1, BoardTests.Height - 1));
			Assert.IsFalse(isGameFinished);
			Assert.AreEqual(0, logic.ActivePlayer);
			Assert.AreEqual(1, logic.GetPlayerScore(1));
		}

		[Test]
		public void ThirdMoveChangesTopLeftColorAndActivePlayer()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			logic.MakeMove(Color.Red);
			logic.MakeMove(Color.Green);
			logic.MakeMove(Color.Purple);
			Assert.AreEqual(Color.Purple, logic.Board.GetColor(0, 0));
			Assert.IsFalse(isGameFinished);
			Assert.AreEqual(1, logic.ActivePlayer);
			Assert.AreEqual(1, logic.GetPlayerScore(0));
		}

		[Test]
		public void GameOverWhenOnlyPlayerColorsRemain()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			SetFirstThreeColumnsToBlue();
			SetNextThreeColumnsToGreen();
			SetRemainingColumnsToRed();
			logic.MakeMove(Color.Green);
			Assert.IsTrue(isGameFinished);
			Assert.AreEqual(6 * BoardTests.Height, logic.GetPlayerScore(0));
		}

		private void SetFirstThreeColumnsToBlue()
		{
			for (int x = 0; x < 3; x++)
				for (int y = 0; y < BoardTests.Height; y++)
					logic.Board.SetColor(x, y, Color.Blue);
		}

		private void SetNextThreeColumnsToGreen()
		{
			for (int x = 3; x < 6; x++)
				for (int y = 0; y < BoardTests.Height; y++)
					logic.Board.SetColor(x, y, Color.Green);
		}

		private void SetRemainingColumnsToRed()
		{
			for (int x = 6; x < BoardTests.Width; x++)
				for (int y = 0; y < BoardTests.Height; y++)
					logic.Board.SetColor(x, y, Color.Red);
		}

		[Test]
		public void PassingChangesActivePlayer()
		{
			logic.Pass();
			Assert.AreEqual(1, logic.ActivePlayer);
		}

		[Test]
		public void PassingTwiceChangesActivePlayerBack()
		{
			logic.Pass();
			logic.Pass();
			Assert.AreEqual(0, logic.ActivePlayer);
		}

		[Test]
		public void CreateFromBoardData()
		{
			logic = new TwoHumanLogic(BoardTests.CreateBoardData());
			logic.MakeMove(Color.Red);
			Assert.AreEqual(Color.Red, logic.Board.GetColor(0, 0));
			Assert.AreEqual(1, logic.ActivePlayer);
			Assert.AreEqual(2, logic.GetPlayerScore(0));
		}
	}
}