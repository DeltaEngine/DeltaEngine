using CreepyTowers.Content;
using DeltaEngine.Content;
using DeltaEngine.Multimedia;

namespace CreepyTowers.Enemy.Creeps
{
	public class CreepDyingSoundSelector
	{
		public static void PlayDestroyedSound(CreepType creepType)
		{
			if (creepType == CreepType.Cloth)
				PlayDestroyedSound(GameSounds.CreepDestroyedCloth);
			if (creepType == CreepType.Glass)
				PlayDestroyedSound(GameSounds.CreepDestroyedGlass);
			if (creepType == CreepType.Iron)
				PlayDestroyedSound(GameSounds.CreepDestroyedIron);
			if (creepType == CreepType.Paper)
				PlayDestroyedSound(GameSounds.CreepDestroyedPaper);
			if (creepType == CreepType.Plastic)
				PlayDestroyedSound(GameSounds.CreepDestroyedPlastic);
			if (creepType == CreepType.Sand)
				PlayDestroyedSound(GameSounds.CreepDestroyedSand);
			if (creepType == CreepType.Wood)
				PlayDestroyedSound(GameSounds.CreepDestroyedWood);
		}

		private static void PlayDestroyedSound(GameSounds soundName)
		{
			var sound = ContentLoader.Load<Sound>(soundName.ToString());
			if (sound != null)
				sound.Play();
		}
	}
}