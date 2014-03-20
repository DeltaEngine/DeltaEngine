using System.Collections.Generic;
using CreepyTowers.Avatars;
using CreepyTowers.Content;
using CreepyTowers.Levels;
using CreepyTowers.Towers;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.GameLogic;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace CreepyTowers.GUI
{
	public class TowerSelectionPanel : Menu
	{
		public TowerSelectionPanel()
		{
			towerButtonPanel = new List<InteractiveButton>();
			CreateScene();
			new Command(GameCommands.MouseRightButtonClick.ToString(), (pos) =>
			{
				if (IsShown)
					Hide();
				if (!IsShown)
					SellTower(pos);
			});
		}

		private readonly List<InteractiveButton> towerButtonPanel;

		private static void SellTower(Vector2D point)
		{
			var level = (GameLevel)Level.Current;
			if (level == null)
				return;
			level.SellTower(GameLevelExtensions.GetGridPosition(point));
		}

		public List<string> InactiveButtonsList
		{
			get { return inactiveButtonsList; }
			set
			{
				inactiveButtonsList = value;
				UpdateButtonEnabledStates();
			}
		}

		private List<string> inactiveButtonsList;

		protected override sealed void CreateScene()
		{
			Scene = ContentLoader.Load<Scene>(GameMenus.TowerSelectionPanel.ToString());
			Hide();
			SetupButtonsAndAttachEvents();
		}

		private void SetupButtonsAndAttachEvents()
		{
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton1, TowerType.Acid);
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton2, TowerType.Fire);
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton3, TowerType.Ice);
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton4, TowerType.Impact);
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton5, TowerType.Slice);
			AttachButtonEvent(Content.TowerSelectionPanel.PanelButton6, TowerType.Water);
		}

		private void AttachButtonEvent(Content.TowerSelectionPanel buttonName, TowerType type)
		{
			var button = (InteractiveButton)GetSceneControl(buttonName.ToString());
			button.AddTag(type.ToString());
			button.Clicked += () => BuildTower(type);
			towerButtonPanel.Add(button);
		}

		private void UpdateButtonEnabledStates()
		{
			if (InactiveButtonsList == null || InactiveButtonsList.Count == 0)
				return;

			foreach (InteractiveButton button in towerButtonPanel)
			{
				if (!InactiveButtonsList.Contains(button.GetTags()[0]))
					continue;
				button.Material.DefaultColor = Color.DarkGray;
				button.IsEnabled = false;
			}
		}

		private void BuildTower(TowerType type)
		{
			level = (GameLevel)Level.Current;
			Hide();
			if (IsSpecialAttackPossible())
				SpecialAttackSelector.SelectAttack(level.GetRealPosition(clickedPosition));
			else
				level.SpawnTower(type, clickedPosition, 180.0f);
		}

		private GameLevel level;
		private Vector2D clickedPosition;

		private bool IsSpecialAttackPossible()
		{
			level = (GameLevel)Level.Current;
			return level.IsInsideLevelGrid(clickedPosition) &&
				(Player.Current.Avatar.SpecialAttackAIsActivated ||
					Player.Current.Avatar.SpecialAttackBIsActivated);
		}

		public void Display(Vector2D screenPos)
		{
			var currentGameLevel = (GameLevel)Level.Current;
			if (currentGameLevel == null || currentGameLevel.IsCompleted)
				return;
			var gridPos = GameLevelExtensions.GetGridPosition(screenPos);
			if (!currentGameLevel.IsInsideLevelGrid(gridPos))
				return;
			if (!currentGameLevel.IsTileInteractable(gridPos))
			{
				DisplayCrossBillboard();
				return;
			}

			clickedPosition = gridPos;
			MoveSceneToClickedPosition(screenPos);
			Show();
		}

		private void DisplayCrossBillboard() {}

		private void MoveSceneToClickedPosition(Vector2D pos)
		{
			var panelButton6 = GetSceneControl(Content.TowerSelectionPanel.PanelButton6.ToString());
			var panelButton3 = GetSceneControl(Content.TowerSelectionPanel.PanelButton3.ToString());
			var halfDistBetweenButtons = (panelButton6.Center - panelButton3.Center).Length * 0.5f;
			panelButton6.Center = new Vector2D(pos.X - halfDistBetweenButtons, pos.Y);
		}

		public override void Reset() {}
	}
}