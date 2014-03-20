using System.Globalization;
using System.Linq;
using CreepyTowers.Avatars;
using CreepyTowers.Content;
using CreepyTowers.Levels;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.GameLogic;
using DeltaEngine.Multimedia;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class Hud : Menu
	{
		public Hud()
		{
			CreateScene();
			AddInputCommands();
		}

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.SceneGameHud.ToString());
			UpdateLevelValuesHud();
			Hide();
			AttachButtonEvents();
		}

		private void AttachButtonEvents()
		{
			AttachInGameOptionsButtonEvents();
			AttachSpecialAttackAButtonEvent();
			AttackSpecialAttackBButtonEvent();
		}

		private void AttachInGameOptionsButtonEvents()
		{
			var button = (InteractiveButton)GetSceneControl(GameHud.ButtonInGameOptions.ToString());
			button.Clicked += DisplayGamePausedScene;
		}

		private static void DisplayGamePausedScene()
		{
			PlayClickedSound();
			Time.IsPaused = true;
			MenuController.Current.MoveMenuToBackground(GameMenus.SceneGameHud);
			var towerPanel = MenuController.Current.GetMenu(GameMenus.TowerSelectionPanel);
			if (towerPanel.IsShown)
				MenuController.Current.MoveMenuToBackground(GameMenus.TowerSelectionPanel);
			MenuController.Current.ShowMenu(GameMenus.SceneGamePaused);
		}

		private static void PlayClickedSound()
		{
			var clickSound = ContentLoader.Load<Sound>(GameSounds.MenuButtonClick.ToString());
			if (clickSound != null)
				clickSound.Play();
		}

		private void AttachSpecialAttackAButtonEvent()
		{
			buttonSpecialAttackA =
				(InteractiveButton)GetSceneControl(GameHud.ButtonSpecialAttackA.ToString());
			buttonSpecialAttackA.Clicked += () =>
			{
				PlayClickedSound();
				StartSpecialAttackA();
			};
		}

		private InteractiveButton buttonSpecialAttackA;

		private void StartSpecialAttackA()
		{
			Player.Current.Avatar.SpecialAttackAIsActivated = true;
			ActivateSpecialAttackA(Player.Current.Avatar);
			AvatarSpecialAttackSoundSelector.PlaySpecialAttackSound(buttonSpecialAttackA);
		}

		private static void ActivateSpecialAttackA(Avatar avatar)
		{
			if (avatar is Dragon)
				avatar.ActivatedSpecialAttack = AvatarAttack.DragonBreathOfFire;
			else if (avatar is Penguin)
				avatar.ActivatedSpecialAttack = AvatarAttack.PenguinBigFirework;
			else if (avatar is PiggyBank)
				avatar.ActivatedSpecialAttack = AvatarAttack.PiggyBankCoinMinefield;
		}

		private void AttackSpecialAttackBButtonEvent()
		{
			buttonSpecialAttackB =
				(InteractiveButton)GetSceneControl(GameHud.ButtonSpecialAttackB.ToString());
			buttonSpecialAttackB.Clicked += () =>
			{
				PlayClickedSound();
				StartSpecialAttackB();
			};
		}

		private InteractiveButton buttonSpecialAttackB;

		private void StartSpecialAttackB()
		{
			Player.Current.Avatar.SpecialAttackBIsActivated = true;
			ActivateSpecialAttackB(Player.Current.Avatar);
			AvatarSpecialAttackSoundSelector.PlaySpecialAttackSound(buttonSpecialAttackB);
		}

		private static void ActivateSpecialAttackB(Avatar avatar)
		{
			if (avatar is Dragon)
				avatar.ActivatedSpecialAttack = AvatarAttack.DragonAuraCannon;
			else if (avatar is Penguin)
				avatar.ActivatedSpecialAttack = AvatarAttack.PenguinCarpetBombing;
			else if (avatar is PiggyBank)
				avatar.ActivatedSpecialAttack = AvatarAttack.PiggyBankPayDay;
		}

		private static void AddInputCommands()
		{
			new Command(GameCommands.MouseLeftButtonClick.ToString(), pos =>
			{
				var cinematicMenus = MenuController.Current.GetAllCinematicMenus();
				if (cinematicMenus.Any(cinematic => cinematic.IsShown) || cinematicMenus.Count == 0)
					return;
				if (IsSpecialAttackPossible(pos))
					SpecialAttackSelector.SelectAttack(((GameLevel)Level.Current).GetRealPosition(pos));
				else
					ShowTowerPanel(pos);
			});
		}

		private static bool IsSpecialAttackPossible(Vector2D pos)
		{
			return Player.Current.Avatar.SpecialAttackAIsActivated ||
				Player.Current.Avatar.SpecialAttackBIsActivated &&
					Level.Current.IsInsideLevelGrid(GameLevelExtensions.GetGridPosition(pos));
		}

		private static void ShowTowerPanel(Vector2D pos)
		{
			var towerPanel =
				(TowerSelectionPanel)MenuController.Current.GetMenu(GameMenus.TowerSelectionPanel);
			if (!towerPanel.IsShown)
				towerPanel.Display(pos);
		}

		public void UpdateLevelValuesHud()
		{
			var level = (GameLevel)Level.Current;
			if (level == null)
				return;
			UpdateMoneyCounter();
			UpdateGemsCounter();
			UpdateLifeCounter();
			UpdateWaveCounter();
			level.MoneyUpdated += UpdateMoneyCounter;
			level.LifeUpdated += UpdateLifeCounter;
			level.WaveUpdated += UpdateWaveCounter;
		}

		private void UpdateMoneyCounter()
		{
			var label = (Label)GetSceneControl(GameHud.GoldCount.ToString());
			if (label != null)
				label.Text = Player.Current.Gold.ToString(CultureInfo.InvariantCulture);
		}

		private void UpdateGemsCounter()
		{
			var label = (Label)GetSceneControl(GameHud.GemCount.ToString());
			if (label != null)
				label.Text = Player.Current.Gems.ToString(CultureInfo.InvariantCulture);
		}

		public void UpdateLifeCounter()
		{
			var label = (Label)GetSceneControl(GameHud.PlayerHealthCount.ToString());
			if (label != null)
				label.Text = Player.Current.LivesLeft.ToString(CultureInfo.InvariantCulture);
		}

		private void UpdateWaveCounter()
		{
			var level = (GameLevel)Level.Current;
			if (level == null)
				return;
			var label = (Label)GetSceneControl(GameHud.CreepWaveCount.ToString());
			if (label == null)
				return;
			var spawnedWaves = level.TotalNumberOfWaves - level.WaveGenerator.waveList.Count;
			label.Text = spawnedWaves + " / " +
				level.TotalNumberOfWaves.ToString(CultureInfo.InvariantCulture);
		}

		public override void Reset()
		{
			Enable();
		}
	}
}