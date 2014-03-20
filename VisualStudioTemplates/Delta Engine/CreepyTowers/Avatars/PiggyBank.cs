using CreepyTowers.Collectables;
using CreepyTowers.Enemy.Creeps;
using CreepyTowers.Stats;
using CreepyTowers.Towers;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;

namespace $safeprojectname$.Avatars
{
	/// <summary>
	/// Piggy Bank is all about the gold. Special abilities are: 
	/// Greed: Amount of gold earned for killing a creep is increased
	/// Payday: Creeps drop extra gold
	/// Coins Rain: Collectable coins fall on the grid over a short period, doing damage
	/// </summary>
	public class PiggyBank : Avatar
	{
		public PiggyBank()
		{
			IsLocked = false;
			AddTag(Content.Avatars.PiggyBank.ToString());
		}

		public override void PerformAttack(AvatarAttack attack, Vector2D position)
		{
			if (attack == AvatarAttack.PiggyBankCoinMinefield)
				SpawnCoinMinefield();
			else if (attack == AvatarAttack.PiggyBankPayDay)
				BuffAllCreepsToDropMoreGold();
		}

		private static void SpawnCoinMinefield()
		{
			var width = (int)Level.Current.Size.Width;
			var height = (int)Level.Current.Size.Height;
			var coinsCount = (int)(width * height * CoinPercentage);
			for (int i = 0; i < coinsCount; i++)
				SpawnCoin(Randomizer.Current.Get(0, width), Randomizer.Current.Get(0, height));
		}

		private const float CoinPercentage = 0.2f;

		private static void SpawnCoin(int x, int y)
		{
			new Coin(new Vector3D(x + 0.5f, y + 0.5f, 0), CoinGoldValue)
			{
				DamageType = CoinDamageType,
				DamageRange = CoinDamageRange,
				Damage = CoinDamage
			};
		}

		private const int CoinGoldValue = 1;
		private const TowerType CoinDamageType = TowerType.Fire;
		private const float CoinDamage = 10;
		private const float CoinDamageRange = 0.5f;

		private static void BuffAllCreepsToDropMoreGold()
		{
			var creeps = EntitiesRunner.Current.GetEntitiesOfType<Creep>();
			var payday = new BuffEffect("PiggyBankPayDayGoldMultiplier");
			foreach (var creep in creeps)
				creep.ApplyBuff(payday);
		}
	}
}