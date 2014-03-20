using DeltaEngine.Entities;
using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Triggers
{
	public class StartTimer : GameTrigger
	{
		public StartTimer(string startingTime)
		{
			StartingTime = startingTime.Convert<int>();
		}

		public int StartingTime { get; private set; }

		protected override void StartingLevel()
		{
			Player.Current.Time = StartingTime;
		}

		protected override void UpdateAfterOneSecond()
		{
			deltaTime += Time.Delta;
			if (deltaTime < 1.0f)
				return;
			var player = Player.Current;
			player.Time--;
			if (player.Time <= 0)
				OnGameOver();
			deltaTime -= 1.0f; //ncrunch: no coverage end
		}

		private float deltaTime;
	}
}