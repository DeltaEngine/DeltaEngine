using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;

namespace $safeprojectname$.Logics
{
	public abstract class Logic
	{
		protected Logic(int width, int height, Vector2D[] homeSquares)
		{
			Board = new Board(width, height);
			this.homeSquares = homeSquares;
			availableColorFinder = new AvailableColorFinder(Board, homeSquares);
			turns = new int[homeSquares.Length];
		}

		internal Board Board { get; set; }
		internal readonly AvailableColorFinder availableColorFinder;
		protected readonly Vector2D[] homeSquares;
		protected readonly int[] turns;

		protected Logic(Board.Data boardData, Vector2D[] homeSquares)
		{
			Board = new Board(boardData);
			this.homeSquares = homeSquares;
			availableColorFinder = new AvailableColorFinder(Board, homeSquares);
			turns = new int[homeSquares.Length];
		}

		public void Reset()
		{
			Board.Randomize();
		}

		public abstract void Pass();
		public abstract void MakeMove(Color color);

		public class CannotMakeMoveWhenGameIsOver : Exception {}

		protected void CheckForGameOver()
		{
			for (int i = 0; i < homeSquares.Length; i++)
				if (HasPlayerAnyValidMoves(i))
					return;
			GameOver();
		}

		protected void GameOver()
		{
			IsGameOver = true;
			if (GameFinished != null)
				GameFinished();
		}

		public bool IsGameOver { get; private set; }
		public event Action GameFinished;

		public int GetPlayerScore(int player)
		{
			return Board.GetConnectedColorsCount(homeSquares[player]);
		}

		public int ActivePlayer { get; protected set; }

		public bool LastPlayerPassed { get; protected set; }

		internal bool HasPlayerAnyValidMoves(int player)
		{
			return GetPlayerValidMoves(player).Any();
		}

		internal List<Color> GetPlayerValidMoves(int player)
		{
			List<Color> availableColors = availableColorFinder.GetAvailableColors();
			return availableColors.Where(color => IsValidMove(player, color)).ToList();
		}

		private bool IsValidMove(int player, Color color)
		{
			Board clone = Board.Clone();
			clone.SetColor(homeSquares[player], color);
			return clone.GetConnectedColorsCount(homeSquares[player]) >
				Board.GetConnectedColorsCount(homeSquares[player]);
		}

		public int GetPlayerTurnsTaken(int player)
		{
			return turns[player];
		}
	}
}