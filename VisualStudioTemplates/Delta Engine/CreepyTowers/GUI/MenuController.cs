using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CreepyTowers.Content;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class MenuController : IDisposable
	{
		public MenuController()
		{
			menuList = new Menu[Enum.GetNames(typeof(GameMenus)).Length];
			Current = this;
			CreateAllGameScenes();
		}

		public static MenuController Current { get; private set; }

		private readonly Menu[] menuList;

		private void CreateAllGameScenes()
		{
			menuList[(int)GameMenus.SceneMainMenu] = new MainMenu(Game.AppWindow);
			menuList[(int)GameMenus.SceneSettingsMenu] = new Settings();
			menuList[(int)GameMenus.SceneCredits] = new Credits();
			menuList[(int)GameMenus.SceneIntro] = new IntroScene();
			menuList[(int)GameMenus.SceneNightmare1] = new Nightmare1();
			menuList[(int)GameMenus.SceneGameHud] = new Hud();
			menuList[(int)GameMenus.SceneGamePaused] = new PauseScene();
			menuList[(int)GameMenus.SceneChapterSuccess] = new SuccessScene();
			menuList[(int)GameMenus.SceneAvatarSelection] = new AvatarSelectionMenu();
			menuList[(int)GameMenus.TowerSelectionPanel] = new TowerSelectionPanel();
			menuList[(int)GameMenus.SceneTutorial] = new TutorialCinematics();
			menuList[(int)GameMenus.SceneAvatarSuperVillain] = new AvatarVillianCinematics();
		}

		public void MoveMenuToBackground(GameMenus menuToBackground)
		{
			var menuToBePushedToBackground = menuList[(int)menuToBackground];
			if (menuToBePushedToBackground != null)
				menuToBePushedToBackground.Disable();
		}

		public void MoveMenuToForeground(GameMenus menuToForeground)
		{
			var menuToBePushedToForeground = menuList[(int)menuToForeground];
			if (menuToBePushedToForeground != null)
				menuToBePushedToForeground.Enable();
		}

		public void ShowMenu(GameMenus menuToBeShown)
		{
			var menuToShow = menuList[(int)menuToBeShown];
			if (menuToShow == null)
				return;

			menuToShow.Show();
			menuToShow.Reset();
		}

		public void HideMenu(GameMenus menuToBeHid)
		{
			var menuToHide = menuList[(int)menuToBeHid];
			if (menuToHide == null)
				return;

			menuToHide.Hide();
		}

		public Menu GetMenu(GameMenus menu)
		{
			return menuList[(int)menu];
		}

		public void UpdateStatsWhenChapterOver()
		{
			var chapterSuccess = GetMenu(GameMenus.SceneChapterSuccess);
			var gold = (Label)chapterSuccess.GetSceneControl(Content.SuccessScene.GoldCount.ToString());
			var gems = (Label)chapterSuccess.GetSceneControl(Content.SuccessScene.GemCount.ToString());
			var playerLevel =
				(Label)chapterSuccess.GetSceneControl(Content.SuccessScene.PlayerLevel.ToString());
			var avatarLevel =
				(Label)chapterSuccess.GetSceneControl(Content.SuccessScene.AvatarLevel.ToString());
			var player = Player.Current;
			gold.Text = player.Gold.ToString(CultureInfo.InvariantCulture);
			gems.Text = player.Gems.ToString(CultureInfo.InvariantCulture);
			playerLevel.Text = player.ProgressLevel.ToString(CultureInfo.InvariantCulture);
			avatarLevel.Text = player.Avatar.ProgressLevel.ToString(CultureInfo.InvariantCulture);
		}

		public void HideAllMenus()
		{
			foreach (Menu menu in menuList.Where(menu => menu != null))
				menu.Hide();
		}

		public void HideAllVisibleMenus()
		{
			foreach (Menu menu in menuList.Where(menu => menu != null && menu.IsShown))
				menu.Hide();
		}

		public List<SceneCinematics> GetAllCinematicMenus()
		{
			var cinematics = new List<SceneCinematics>();
			foreach (Menu menu in menuList)
				if (menu is SceneCinematics)
					cinematics.Add((SceneCinematics)menu);
			return cinematics;
		}

		public void Dispose() {}
	}
}