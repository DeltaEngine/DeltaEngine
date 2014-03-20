using CreepyTowers.Content;
using CreepyTowers.Levels;
using DeltaEngine.Content;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class PauseScene : Menu
	{
		public PauseScene()
		{
			CreateScene();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneGamePaused.ToString());
			Hide();
			AttachButtonEvents();
		}

		private void AttachButtonEvents()
		{
			AttachHomeButtonEvent();
			AttachRestartButtonEvent();
			AttachResumeButtonEvent();
		}

		private void AttachHomeButtonEvent()
		{
			var button = (InteractiveButton)GetSceneControl(Content.PauseScene.ButtonHome.ToString());
			button.Clicked += () =>
			{
				PauseTimeAndMoveHudToBackground();
				DisposeActorsLevelGridAndCommands();
			};
		}

		private static void PauseTimeAndMoveHudToBackground()
		{
			PlayClickedSound();
			Time.IsPaused = false;
			MenuController.Current.HideAllMenus();
			MenuController.Current.ShowMenu(GameMenus.SceneMainMenu);
			Player.Current.Avatar.Reset();
		}

		private static void PlayClickedSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (clickSound != null)
				clickSound.Play();
		}

		private static void DisposeActorsLevelGridAndCommands()
		{
			Level.Current.Dispose();
		}

		private void AttachRestartButtonEvent()
		{
			var button = (InteractiveButton)GetSceneControl(Content.PauseScene.ButtonRestart.ToString());
			button.Clicked += () =>
			{
				UnPauseTimeAndMoveHudToForeground();
				MenuController.Current.HideAllVisibleMenus();
				var hud = (Hud)MenuController.Current.GetMenu(GameMenus.SceneGameHud);
				hud.Show();
				hud.UpdateLevelValuesHud();
				Chapter.Current.Restart();
			};
		}

		private static void UnPauseTimeAndMoveHudToForeground()
		{
			PlayClickedSound();
			Time.IsPaused = false;
			MenuController.Current.HideMenu(GameMenus.SceneGamePaused);
			MenuController.Current.MoveMenuToForeground(GameMenus.SceneGameHud);
			var towerPanel = MenuController.Current.GetMenu(GameMenus.TowerSelectionPanel);
			if (towerPanel.IsShown)
				towerPanel.Enable();
			Player.Current.Avatar.Reset();
		}

		private void AttachResumeButtonEvent()
		{
			var button = (InteractiveButton)GetSceneControl(Content.PauseScene.ButtonResume.ToString());
			button.Clicked += UnPauseTimeAndMoveHudToForeground;
		}

		public override void Reset() {}
	}
}