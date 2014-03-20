using System;
using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Multimedia;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace GhostWars
{
	public class MainMenu : Entity
	{
		public MainMenu(Window window)
		{
			//menuScene = new Scene();
			CreateMainMenu();
			new Command(Command.Exit, window.CloseAfterFrame);
		}

		//private readonly Scene menuScene;

		//ncrunch: no coverage start, user interface
		private void CreateMainMenu()
		{
			Clear();
			State = GameState.Menu;
			if (trees != null)
			{
				trees.RemoveTrees();
				trees.Dispose();
			}
			AddMenuBackground();
			AddMenuOption(OnHowToPlay, "HowToPlay", new Vector2D(0.5f, 0.50f));
			AddMenuOption(OnSingleplayer, "Singleplayer", new Vector2D(0.5f, 0.57f));
			AddMenuOption(OnCredits, "Credits", new Vector2D(0.5f, 0.64f));
		}

		public void Clear()
		{
			if (entities.Count > 0)
				PlayClickSound();
			foreach (var entity in entities)
				entity.Dispose();
			entities.Clear();
			if (background != null)
				background.IsActive = false;
			//menuScene.Clear();
		}

		private readonly List<Entity> entities = new List<Entity>();

		private static void PlayClickSound()
		{
			ContentLoader.Load<Sound>("MalletHit").Play();
		}

		private void AddMenuBackground()
		{
			background =
				new Sprite(new Material(ShaderFlags.Position2DColoredTextured, "MenuBackground"),
					ScreenSpace.Scene.Viewport) { RenderLayer = int.MinValue };
			ScreenSpace.Scene.ViewportSizeChanged +=
				() => background.SetWithoutInterpolation(ScreenSpace.Scene.Viewport);
			//menuScene.SetViewportBackground("MenuBackground");
		}

		private void AddEntity(Entity entity)
		{
			entities.Add(entity);
		}

		private void AddMenuOption(Action clickAction, string buttonName, Vector2D position)
		{
			var buttonImage = ContentLoader.Load<Image>(buttonName + "Default");
			var buttonRect = Rectangle.FromCenter(position,
				new Size(0.05f * buttonImage.PixelSize.AspectRatio, 0.05f));
			var button = new Sprite(buttonName + "Default", buttonRect);
			button.RenderLayer = 10;
			AddEntity(button);
			AddEntity(new Command(Command.Click, point =>
			{
				if (buttonRect.Contains(point))
					clickAction();
			}));
			AddEntity(
				new Command(pos => UpdateSpriteImage(button, buttonName, pos)).Add(
					new MouseMovementTrigger()));
		}

		private void AddSubMenuBackButton()
		{
			var buttonRect = new Rectangle(ScreenSpace.Current.Viewport.Left + 0.025f,
				ScreenSpace.Current.Viewport.Bottom - 0.075f, 0.125f, 0.05f);
			var button = new Sprite("Back" + "Default", buttonRect);
			button.RenderLayer = 10;
			AddEntity(button);
			AddEntity(new Command(Command.Click, point =>
			{
				if (buttonRect.Contains(point))
					CreateMainMenu();
			}));
			AddEntity(
				new Command(pos => UpdateSpriteImage(button, "Back", pos)).Add(new MouseMovementTrigger()));
		}

		private void UpdateSpriteImage(Sprite button, string name, Vector2D position)
		{
			if (button.DrawArea.Contains(position) && button.Material.DiffuseMap.Name != name + "Hover")
				button.Material = new Material(ShaderFlags.Position2DColoredTextured, name + "Hover");
			else if (!button.DrawArea.Contains(position) &&
				button.Material.DiffuseMap.Name != name + "Default")
				button.Material = new Material(ShaderFlags.Position2DColoredTextured, name + "Default");
			else
				return;
			if (Time.Total - lastSwingSound < MinimumDelayBetweenMenuSwingSounds)
				return;
			lastSwingSound = Time.Total;
			ContentLoader.Load<Sound>("MalletSwing").Play(DurationOfMenuSwingSound);
		}

		private float lastSwingSound;
		private const float MinimumDelayBetweenMenuSwingSounds = 0.075f;
		private const float DurationOfMenuSwingSound = 0.24f;

		private void OnHowToPlay()
		{
			Clear();
			AddEntity(new Sprite("GhostWarsHowToPlay", ScreenSpace.Current.Viewport));
			AddSubMenuBackButton();
		}

		private void OnSingleplayer()
		{
			Clear();
			AddEntity(new Sprite("LevelSelectionBackground", ScreenSpace.Current.Viewport));
			var clickAreas = new[]
			{
				Rectangle.FromCenter(0.25f, 0.66f, 0.19f, 0.19f),
				Rectangle.FromCenter(0.5f, 0.66f, 0.19f, 0.19f),
				Rectangle.FromCenter(0.75f, 0.66f, 0.19f, 0.19f)
			};
			AddLevelSelection(1, clickAreas[0]);
			AddLevelSelection(2, clickAreas[1]);
			AddLevelSelection(3, clickAreas[2]);
			AddEntity(new Command(Command.Click, position => SinglePlayerMenuClick(position, clickAreas)));
			AddSubMenuBackButton();
		}

		private void AddLevelSelection(int levelNumber, Rectangle mapDrawArea)
		{
			var levelText = new FontText(Font, levelNumber + "",
				Rectangle.FromCenter(mapDrawArea.Center - new Vector2D(0.0f, 0.115f), new Size(0.2f, 0.1f)));
			AddEntity(levelText);
			var map = new Sprite("GhostWarsLevel" + levelNumber, mapDrawArea);
			AddEntity(map);
		}

		private void SinglePlayerMenuClick(Vector2D position, Rectangle[] clickAreas)
		{
			for (int i = 0; i < clickAreas.Length; i++)
				if (clickAreas[i].Contains(position))
					StartGame(i + 1);
		}

		private void StartGame(int level)
		{
			Clear();
			State = GameState.CountDown;
			background = new Sprite(new Material(ShaderFlags.Position2DColoredTextured, "Background"),
				ScreenSpace.Scene.Viewport) { RenderLayer = int.MinValue };
			ScreenSpace.Scene.ViewportSizeChanged +=
				() => background.SetWithoutInterpolation(ScreenSpace.Scene.Viewport);
			//menuScene.SetViewportBackground("Background");
			trees = new TreeManager(Team.HumanYellow);
			if (level == 1)
				SetupLevel1Trees();
			else if (level == 2)
				SetupLevel2Trees();
			else
				SetupLevel3Trees();
			trees.GameFinished += SetGameOverState;
			trees.GameLost += SetGameOverState;
		}

		private TreeManager trees;
		public int CurrentLevel { get; set; }

		private Sprite background;

		private void SetupLevel1Trees()
		{
			CurrentLevel = 1;
			trees.AddTree(new Vector2D(0.11f, 0.5f), Team.HumanYellow);
			trees.AddTree(new Vector2D(0.2f, 0.4f), Team.None);
			trees.AddTree(new Vector2D(0.12f, 0.675f), Team.None);
			trees.AddTree(new Vector2D(0.365f, 0.45f), Team.None);
			trees.AddTree(new Vector2D(0.265f, 0.6f), Team.None);
			trees.AddTree(new Vector2D(0.9f, 0.675f), Team.ComputerPurple);
			trees.AddTree(new Vector2D(0.89f, 0.5f), Team.None);
			trees.AddTree(new Vector2D(0.91f, 0.325f), Team.ComputerTeal);
			trees.AddTree(new Vector2D(0.74f, 0.325f), Team.None);
			trees.AddTree(new Vector2D(0.73f, 0.675f), Team.None);
		}

		private void SetupLevel2Trees()
		{
			CurrentLevel = 2;
			trees.AddTree(new Vector2D(0.325f, 0.55f), Team.HumanYellow);
			trees.AddTree(new Vector2D(0.225f, 0.4f), Team.None);
			trees.AddTree(new Vector2D(0.225f, 0.7f), Team.None);
			trees.AddTree(new Vector2D(0.575f, 0.41f), Team.None);
			trees.AddTree(new Vector2D(0.575f, 0.68f), Team.None);
			trees.AddTree(new Vector2D(0.725f, 0.42f), Team.ComputerPurple);
			trees.AddTree(new Vector2D(0.85f, 0.56f), Team.None);
			trees.AddTree(new Vector2D(0.685f, 0.53f), Team.None);
			trees.AddTree(new Vector2D(0.725f, 0.67f), Team.ComputerTeal);
		}

		private void SetupLevel3Trees()
		{
			CurrentLevel = 3;
			trees.AddTree(new Vector2D(0.14f, 0.45f), Team.HumanYellow);
			trees.AddTree(new Vector2D(0.5f, 0.4f), Team.None);
			trees.AddTree(new Vector2D(0.8f, 0.45f), Team.ComputerPurple);
			trees.AddTree(new Vector2D(0.85f, 0.62f), Team.None);
			trees.AddTree(new Vector2D(0.715f, 0.549f), Team.None);
			trees.AddTree(new Vector2D(0.46f, 0.67f), Team.ComputerTeal);
			trees.AddTree(new Vector2D(0.25f, 0.56f), Team.None);
		}

		private void OnCredits()
		{
			Clear();
			AddEntity(new Sprite("CreditsBackground", ScreenSpace.Current.Viewport));
			AddSubMenuBackButton();
		}

		public static GameState State { get; set; }
		public static Team PlayerTeam { get; set; }
		public static Font Font
		{
			get { return ContentLoader.Load<Font>("Tahoma30"); }
		}

		public void SetGameOverState()
		{
			AddMenuOption(CreateMainMenu, "Back", new Vector2D(0.5f, 0.6f));
		}

		public void RestartGame()
		{
			StartGame(CurrentLevel);
		}
	}
}