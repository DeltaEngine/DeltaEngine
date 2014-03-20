using DeltaEngine.Datatypes;

namespace CreepyTowers.Avatars
{
	public class SpecialAttackSelector
	{
		public static void SelectAttack(Vector2D pos)
		{
			var avatar = Player.Current.Avatar;
			if (avatar is Dragon)
				SpecialAttackDragon(avatar, pos);
			else if (avatar is Penguin)
				SpecialAttackPenguin(avatar, pos);
			else if (avatar is PiggyBank)
				SpecialAttackPiggyBank(avatar, pos);
		}

		private static void SpecialAttackDragon(Avatar avatar, Vector2D pos)
		{
			switch (avatar.ActivatedSpecialAttack)
			{
			case AvatarAttack.DragonBreathOfFire:
				avatar.PerformAttack(AvatarAttack.DragonBreathOfFire, pos);
				avatar.SpecialAttackAIsActivated = false;
				break;
			case AvatarAttack.DragonAuraCannon:
				avatar.PerformAttack(AvatarAttack.DragonAuraCannon, pos);
				avatar.SpecialAttackBIsActivated = false;
				break;
			}
		}

		private static void SpecialAttackPenguin(Avatar avatar, Vector2D pos)
		{
			switch (avatar.ActivatedSpecialAttack)
			{
			case AvatarAttack.PenguinBigFirework:
				avatar.PerformAttack(AvatarAttack.PenguinBigFirework, pos);
				avatar.SpecialAttackAIsActivated = false;
				break;
			case AvatarAttack.PenguinCarpetBombing:
				avatar.PerformAttack(AvatarAttack.PenguinCarpetBombing, pos);
				avatar.SpecialAttackBIsActivated = false;
				break;
			}
		}

		private static void SpecialAttackPiggyBank(Avatar avatar, Vector2D pos)
		{
			switch (avatar.ActivatedSpecialAttack)
			{
			case AvatarAttack.PiggyBankCoinMinefield:
				avatar.PerformAttack(AvatarAttack.PiggyBankCoinMinefield, pos);
				avatar.SpecialAttackAIsActivated = false;
				break;
			case AvatarAttack.PiggyBankPayDay:
				avatar.PerformAttack(AvatarAttack.PiggyBankPayDay, pos);
				avatar.SpecialAttackBIsActivated = false;
				break;
			}
		}
	}
}