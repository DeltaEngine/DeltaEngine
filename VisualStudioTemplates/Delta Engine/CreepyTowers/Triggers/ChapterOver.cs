using CreepyTowers.Content;
using CreepyTowers.GUI;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;

namespace $safeprojectname$.Triggers
{
	public class ChapterOver
	{
		public static void ChapterCompleted()
		{
			PlaySound(GameSounds.ChapterComplete);
			((GameLevel)Level.Current).IsCompleted = true;
			ToggleMenus(); //ncrunch: no coverage
		}

		private static void ToggleMenus()
		{
			if (MenuController.Current == null)
				return;
			MenuController.Current.HideMenu(GameMenus.SceneGameHud);
			MenuController.Current.ShowMenu(GameMenus.SceneChapterSuccess);
			MenuController.Current.UpdateStatsWhenChapterOver();
		}

		private static void PlaySound(GameSounds soundName)
		{
			var sound = ContentLoader.Load<Sound>(soundName.ToString());
			if (sound == null)
				return;
			sound.Play();
			sound.OnStop += instance => PlayXpCollectedSound();
		}

		private static void PlayXpCollectedSound()
		{
			var xpSound = ContentLoader.Load<Sound>(GameSounds.ChapterXPCountingUP.ToString());
			if (xpSound == null)
				return;
			xpSound.Play();
		}

		public static void ChaptedFailed()
		{
			PlaySound(GameSounds.ChapterFailed);
			ToggleMenus();
		}
	}
}