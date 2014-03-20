using System.Globalization;
using CreepyTowers.Content;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.GUI
{
	public class SuccessScene : Menu
	{
		public SuccessScene()
		{
			CreateScene();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneChapterSuccess.ToString());
			Hide();
			AttachButtonEvents();
		}

		private void AttachButtonEvents()
		{
			AttachHomeButtonEvent();
			AttachReplayButtonEvent();
			AttachContinueButtonEvent();
		}

		private void AttachHomeButtonEvent()
		{
			var button = (InteractiveButton)GetSceneControl(Content.SuccessScene.ButtonHome.ToString());
			button.Clicked += () =>
			{
				PlayClickedSound();
				DisposeAllActors();
				MenuController.Current.HideMenu(GameMenus.SceneChapterSuccess);
				MenuController.Current.HideMenu(GameMenus.SceneGameHud);
				MenuController.Current.ShowMenu(GameMenus.SceneMainMenu);
			};
		}

		private static void PlayClickedSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (clickSound != null)
				clickSound.Play();
		}

		private static void DisposeAllActors()
		{
			var allActors = EntitiesRunner.Current.GetEntitiesOfType<Actor3D>();
			foreach (Actor3D actor in allActors)
				actor.Dispose();
		}

		private void AttachReplayButtonEvent()
		{
			var button =
				(InteractiveButton)GetSceneControl(Content.SuccessScene.ButtonReplay.ToString());
			button.Clicked += () =>
			{
				PlayClickedSound();
				MenuController.Current.HideMenu(GameMenus.SceneChapterSuccess);
				MenuController.Current.ShowMenu(GameMenus.SceneGameHud);
				var hud = (Hud)MenuController.Current.GetMenu(GameMenus.SceneGameHud);
				hud.UpdateLevelValuesHud();
				var chapter = Chapter.Current;
				if (chapter != null)
					chapter.Restart();
			};
		}

		private void AttachContinueButtonEvent()
		{
			var button =
				(InteractiveButton)GetSceneControl(Content.SuccessScene.ButtonContinue.ToString());
			button.Clicked += () =>
			{
				MenuController.Current.HideMenu(GameMenus.SceneGameHud);
				MenuController.Current.HideMenu(GameMenus.SceneChapterSuccess);
				MenuController.Current.ShowMenu(GameMenus.SceneNightmare1);
				Level.Current.Dispose();
			};
		}

		public override void Reset()
		{
			var goldCount = (Label)GetSceneControl(Content.SuccessScene.GoldCount.ToString());
			var gemCount = (Label)GetSceneControl(Content.SuccessScene.GemCount.ToString());
			goldCount.Text = Player.Current.Gold.ToString(CultureInfo.InvariantCulture);
			gemCount.Text = Player.Current.Gems.ToString(CultureInfo.InvariantCulture);
		}
	}
}