using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Triggers
{
	public class StartingLives : GameTrigger
	{
		public StartingLives(string startingLivesAmount)
		{
			Lives = startingLivesAmount.Convert<int>();
		}

		public int Lives { get; private set; }

		protected override void StartingLevel()
		{
			Player.Current.LivesLeft = Lives;
		}
	}
}