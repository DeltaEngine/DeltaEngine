using System;
using System.Collections.Generic;
using CreepyTowers.Content;
using CreepyTowers.Levels;
using CreepyTowers.Nightmares;
using DeltaEngine.Content;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class Nightmare1 : Menu
	{
		public Nightmare1()
		{
			unlockedChapters = new List<Content.Nightmare1>(10);
			CreateScene();
		}

		private readonly List<Content.Nightmare1> unlockedChapters;

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneNightmare1.ToString());
			Hide();
			AttachButtonEvents();
			unlockedChapters.Add(Content.Nightmare1.ButtonChapter1);
		}

		private void AttachButtonEvents()
		{
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter1);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter2);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter3);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter4);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter5);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter6);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter7);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter8);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter9);
			AttachChapterButtonEvent(Content.Nightmare1.ButtonChapter10);
			AddBackButtonEvent();
		}

		private void AttachChapterButtonEvent(Content.Nightmare1 button)
		{
			var chapterButton = (InteractiveButton)GetSceneControl(button.ToString());
			chapterButton.Clicked += () => StartChapter(button);
		}

		private void StartChapter(Content.Nightmare1 button)
		{
			if (!IsChapterUnlocked(button))
				return;
			PlayClickedSound();
			Nightmare1ChapterSelector.SelectChapter(button);
		}

		private bool IsChapterUnlocked(Content.Nightmare1 chapterButton)
		{
			return unlockedChapters.Contains(chapterButton);
		}

		private static void PlayClickedSound()
		{
			var sound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			sound.Play();
		}

		private void AddBackButtonEvent()
		{
			var backButton =
				(InteractiveButton)GetSceneControl(Content.Nightmare1.ButtonBack.ToString());
			backButton.Clicked += ShowAvatarMenuAndHideCurrentMenu;
		}

		private static void ShowAvatarMenuAndHideCurrentMenu()
		{
			PlayClickedSound();
			MenuController.Current.HideMenu(GameMenus.SceneNightmare1);
			MenuController.Current.ShowMenu(GameMenus.SceneAvatarSelection);
		}

		public static int ChapterToUnlock { get; set; }

		public override void Reset()
		{
			var level = (GameLevel)Level.Current;
			if (level != null && level.IsCompleted)
				UnlockChapter();
		}

		private void UnlockChapter()
		{
			if (ChapterToUnlock == 0 || ChapterToUnlock == 1)
				return;

			var unlockedButtonName = "ButtonChapter" + ChapterToUnlock;
			unlockedChapters.Add(
				(Content.Nightmare1)Enum.Parse(typeof(Content.Nightmare1), unlockedButtonName));
			var button = (InteractiveButton)GetSceneControl(unlockedButtonName);
			button.Theme = new Theme
			{
				Button = ContentLoader.Load<Material>("Chapter" + ChapterToUnlock + "Mat"),
				ButtonDisabled = ContentLoader.Load<Material>("Chapter" + ChapterToUnlock + "Mat"),
				ButtonPressed = ContentLoader.Load<Material>("Chapter" + ChapterToUnlock + "Mat"),
				ButtonMouseover = ContentLoader.Load<Material>("Chapter" + ChapterToUnlock + "Mat"),
			};
		}
	}
}