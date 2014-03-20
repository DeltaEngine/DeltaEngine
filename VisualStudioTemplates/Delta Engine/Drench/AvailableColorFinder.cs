using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;

namespace $safeprojectname$
{
	internal class AvailableColorFinder
	{
		public AvailableColorFinder(Board board, Vector2D[] homeSquares)
		{
			this.board = board;
			this.homeSquares = homeSquares;
			homeColors = new Color[homeSquares.Length];
		}

		private readonly Board board;
		private readonly Vector2D[] homeSquares;
		private readonly Color[] homeColors;

		public List<Color> GetAvailableColors()
		{
			availableColors = new List<Color>();
			GetHomeColors();
			for (int x = 0; x < board.Width; x++)
				for (int y = 0; y < board.Height; y++)
					AddColorIfAvailable(x, y);
			return availableColors;
		}

		private List<Color> availableColors;

		private void GetHomeColors()
		{
			for (int i = 0; i < homeSquares.Length; i++)
				homeColors[i] = board.GetColor(homeSquares[i]);
		}

		private void AddColorIfAvailable(int x, int y)
		{
			Color color = board.GetColor(x, y);
			if (!availableColors.Contains(color) && !homeColors.Any(c => color == c))
				availableColors.Insert(Randomizer.Current.Get(0, availableColors.Count + 1), color);
		}
	}
}