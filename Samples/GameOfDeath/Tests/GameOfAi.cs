using DeltaEngine.Core;

namespace GameOfDeath.Tests
{
	/// <summary>
	/// Testing different Ai learning strategies for the Game Of Life to learn the best surviving
	/// strategy by itself.
	/// </summary>
	internal sealed class GameOfAi : GameOfLife
	{
		public GameOfAi(int width, int height)
			: base(width, height)
		{
			aliveCount = new int[width,height];
			foodAvailablePerRound = width * height / 2;
			aliveStrategy = new int[width,height];
			notAliveStrategy = new int[width,height];
			RandomizeStrategies();
		}

		private readonly int[,] aliveCount;
		private readonly int foodAvailablePerRound;
		private readonly int[,] aliveStrategy;
		private readonly int[,] notAliveStrategy;

		private void RandomizeStrategies()
		{
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					SetStrategy(x, y);
		}

		private void SetStrategy(int x, int y)
		{
			aliveStrategy[x, y] = Randomizer.Current.Get(MinNeighbours, MaxNeighbours);
			notAliveStrategy[x, y] = Randomizer.Current.Get(MinNeighbours, MaxNeighbours);
		}

		private const int MinNeighbours = 0;
		private const int MaxNeighbours = 8;

		public override bool ShouldSurvive(int x, int y)
		{
			if (x == 0 && y == 0)
				foodEatenThisRound = 0;

			bool isAlive = this[x, y];
			if (this[x, y])
				aliveCount[x, y]++;
			else if (aliveCount[x, y] > 0)
				aliveCount[x, y]--; //ncrunch: no coverage

			int neighbours = GetNumberOfNeighbours(x, y);
			bool isStarving = foodEatenThisRound > foodAvailablePerRound;
			bool isSurviving = isStarving == false && aliveCount[x, y] < MaxAge &&
				neighbours > (isAlive ? aliveStrategy[x, y] : notAliveStrategy[x, y]);
			if (isSurviving == false)
				if (isAlive)
					aliveStrategy[x, y] = Randomizer.Current.Get(MinNeighbours, neighbours);
				else
					notAliveStrategy[x, y] = Randomizer.Current.Get(neighbours, MaxNeighbours);
			if (isSurviving)
				foodEatenThisRound++;
			return isSurviving;
		}

		private int foodEatenThisRound;
		private const int MaxAge = 10;
	}
}