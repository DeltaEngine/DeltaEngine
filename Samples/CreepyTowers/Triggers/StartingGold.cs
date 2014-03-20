using DeltaEngine.Extensions;
using DeltaEngine.GameLogic;

namespace CreepyTowers.Triggers
{
	/// <summary>
	/// Gold that initializes the amount of money in the game. 
	/// </summary>
	public class StartingGold : GameTrigger
	{
		public StartingGold(string startingGoldAmount)
		{
			Gold = startingGoldAmount.Convert<int>();
		}

		public int Gold { get; private set; }

		protected override void StartingLevel()
		{
			Player.Current.Gold = Gold;
		}
	}
}