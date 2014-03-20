using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.Avatars
{
	public class AvatarSpecialAttackSoundSelector
	{
		public static void PlaySpecialAttackSound(InteractiveButton buttonSpecialAttack)
		{
			if (buttonSpecialAttack.Material.Name == GameHud.DragonAttackBreathMat.ToString())
				PlayAttackSound(GameSounds.DragonBreathOfFire);
			else if (buttonSpecialAttack.Material.Name == GameHud.PenguinAttackBigFireworkMat.ToString())
				PlayAttackSound(GameSounds.PenguinFireworks);
			else if (buttonSpecialAttack.Material.Name == GameHud.PiggyAttackCoinRainMat.ToString())
				PlayAttackSound(GameSounds.PiggyCoinsRain);
			else if (buttonSpecialAttack.Material.Name == GameHud.DragonAttackCannonMat.ToString())
				PlayAttackSound(GameSounds.DragonAuraCannon);
			else if (buttonSpecialAttack.Material.Name == GameHud.PenguinAttackCarpetBombingMat.ToString())
				PlayAttackSound(GameSounds.PenguinSnowball);
			else if (buttonSpecialAttack.Material.Name == GameHud.PiggyAttackPaydayMat.ToString())
				PlayAttackSound(GameSounds.PiggyPayDay);
		}

		private static void PlayAttackSound(GameSounds attackSoundName)
		{
			var attackSound = ContentLoader.Load<Sound>(attackSoundName.ToString());
			if (attackSound != null)
				attackSound.Play();
		}
	}
}