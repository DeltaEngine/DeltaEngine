using DeltaEngine.GameLogic;

namespace CreepyTowers.Triggers
{
	public class SubtractLife : GameTrigger
	{
		protected override void EnemyReachGoalPoint()
		{
			var player = Player.Current;
			if (player == null)
				return;
			player.LivesLeft--;
			if (player.LivesLeft >= 0)
				return;
			player.LivesLeft = 0;
			OnGameOver();
		}
	}
}