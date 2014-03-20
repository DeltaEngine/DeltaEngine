using System;
using System.Collections.Generic;
using System.Linq;
using CreepyTowers.Content;
using CreepyTowers.Towers;
using DeltaEngine.Datatypes;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;

namespace $safeprojectname$.GUI
{
	public class DefaultTowerSelectionPanelForTesting
	{
		public DefaultTowerSelectionPanelForTesting(Vector2D position, Game game)
		{
			towerPanel = new Scene();
			clickedPosition = position;
			this.game = game;
			DisplayTowerSelectionPanel();
		}

		private readonly Scene towerPanel;
		private Vector2D clickedPosition;
		private readonly Game game;

		private void DisplayTowerSelectionPanel()
		{
			DrawAcidTowerButton();
			DrawFireTowerButton();
			DrawIceTowerButton();
			DrawImpactTowerButton();
			DrawSliceTowerButton();
			DrawWaterTowerButton();
		}

		private void DrawAcidTowerButton()
		{
			var button = CreateInteractiveButton(0.0f,  Color.PaleGreen, "Acid");
			AddClickEvent(button, TowerType.Acid, TowerModels.TowerAcidConeJanitorHigh.ToString());
			towerPanel.Add(button);
		}

		private InteractiveButton CreateInteractiveButton(float angle, Color color, string towerName)
		{
			var drawArea = CalculateDrawArea(angle);
			var button = new InteractiveButton(drawArea, towerName);
			button.Color = color;
			button.AddTag(towerName);
			return button;
		}

		private Rectangle CalculateDrawArea(float angle)
		{
			var drawAreaCenterX = (float)(clickedPosition.X + 0.07f * Math.Cos(angle * Math.PI / 180));
			var drawAreaCenterY = (float)(clickedPosition.Y + 0.07f * Math.Sin(angle * Math.PI / 180));
			var drawArea = Rectangle.FromCenter(new Vector2D(drawAreaCenterX, drawAreaCenterY),
				new Size(0.5f));
			return drawArea;
		}

		private void AddClickEvent(InteractiveButton button, TowerType type, string towerName)
		{
			button.Clicked += () =>
			{
				//ContentLoader.Load<Sound>(GameSounds.PressButton.ToString()).Play();
				//game.Get<InGameCommands>().HideTowerPanel();
				//game.CreateTower(game.Get<InGameCommands>().PositionInGrid, type, towerName);
			};
		}

		private void DrawFireTowerButton()
		{
			var button = CreateInteractiveButton(60.0f, Color.Red, "Fire");
			AddClickEvent(button, TowerType.Fire, TowerModels.TowerFireCandlehulaHigh.ToString());
			towerPanel.Add(button);
		}

		private void DrawIceTowerButton()
		{
			var button = CreateInteractiveButton(120.0f, Color.LightBlue, "Ice");
			AddClickEvent(button, TowerType.Ice, TowerModels.TowerIceConeIceladyHigh.ToString());
			towerPanel.Add(button);
		}

		private void DrawImpactTowerButton()
		{
			var button = CreateInteractiveButton(180.0f, Color.Brown, "Impact");
			AddClickEvent(button, TowerType.Impact,
				TowerModels.TowerImpactRangedKnightscalesHigh.ToString());
			towerPanel.Add(button);
		}

		private void DrawSliceTowerButton()
		{
			var button = CreateInteractiveButton(240.0f, Color.White, "Slice");
			AddClickEvent(button, TowerType.Slice, TowerModels.TowerSliceConeKnifeblockHigh.ToString());
			towerPanel.Add(button);
		}

		private void DrawWaterTowerButton()
		{
			var button = CreateInteractiveButton(300.0f, Color.Blue, "Water");
			AddClickEvent(button, TowerType.Water, TowerModels.TowerWaterRangedWatersprayHigh.ToString());
			towerPanel.Add(button);
		}

		public void InactiveButtonsInPanel(List<string> inactiveButtonsTagList)
		{
			if (inactiveButtonsTagList == null)
				return;
			foreach (string tag in inactiveButtonsTagList)
				foreach (InteractiveButton button in 
					towerPanel.Controls.Where(control => control.ContainsTag(tag)))
					button.IsEnabled = false;
		}

		public void Hide()
		{
			towerPanel.Hide();
		}
	}
}