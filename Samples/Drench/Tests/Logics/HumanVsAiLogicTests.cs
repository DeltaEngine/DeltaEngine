using System.Collections.Generic;
using DeltaEngine.Datatypes;
using Drench.Logics;
using NUnit.Framework;
using Randomizer = DeltaEngine.Core.Randomizer;

namespace Drench.Tests.Logics
{
	public class HumanVsAiLogicTests
	{
		public HumanVsAiLogicTests(Logic logic)
		{
			this.logic = logic;
			colorFinder = new AvailableColorFinder(logic.Board, new[] { PlayerHomeSquare, AiHomeSquare });
		}

		private readonly Logic logic;
		private readonly AvailableColorFinder colorFinder;

		public void AiMove()
		{
			bool isGameFinished = false;
			logic.GameFinished += () => isGameFinished = true;
			logic.MakeMove(Color.Red);
			Assert.AreEqual(Color.Red, logic.Board.GetColor(PlayerHomeSquare));
			Assert.AreEqual(new Color(0.5f, 0.5f, 1.0f), logic.Board.GetColor(AiHomeSquare));
			Assert.IsFalse(isGameFinished);
			Assert.AreEqual(0, logic.ActivePlayer);
			Assert.AreEqual(1, logic.GetPlayerScore(0));
			Assert.AreEqual(3, logic.GetPlayerScore(1));
		}

		private static readonly Vector2D PlayerHomeSquare = Vector2D.Zero;
		private static readonly Vector2D AiHomeSquare = new Vector2D(BoardTests.Width - 1,
			BoardTests.Height - 1);

		public void PlayUntilFinished()
		{
			while (!logic.IsGameOver)
				MakeRandomMove();
			Assert.AreEqual(3, logic.GetPlayerScore(0));
			Assert.AreEqual(1, logic.GetPlayerScore(1));
		}

		private void MakeRandomMove()
		{
			List<Color> availableColors = colorFinder.GetAvailableColors();
			logic.MakeMove(availableColors[Randomizer.Current.Get(0, availableColors.Count)]);
		}

		public void WhenPlayerPassesAiPlays()
		{
			logic.Pass();
			Assert.AreEqual(0, logic.ActivePlayer);
			Assert.AreEqual(1, logic.GetPlayerScore(0));
			Assert.AreEqual(3, logic.GetPlayerScore(1));
		}
	}
}