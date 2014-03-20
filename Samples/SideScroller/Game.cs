using System.Collections.Generic;
using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering3D.Particles;
using DeltaEngine.ScreenSpaces;

namespace SideScroller
{
	/// <summary>
	/// Initialization of the SideScrollerGame, creation of all further instances used during the game.
	/// </summary>
	public class Game : Entity2D
	{
		public Game(Window window)
			: base(Rectangle.Zero)
		{
			Settings.Current.LimitFramerate = 60;
			window.ViewportPixelSize = Settings.Current.Resolution;
			mainMenu = new Menu();
			mainMenu.InitGame += StartGame;
			mainMenu.QuitGame += window.CloseAfterFrame;
			window.ViewportSizeChanged += size => { Settings.Current.Resolution = size; };
		}

		public readonly Menu mainMenu;

		public void StartGame()
		{
			IsActive = true;
			mainMenu.Hide();
			if (backToMenuCommand != null && backToMenuCommand.IsActive)
				backToMenuCommand.Dispose(); //ncrunch: no coverage
			if (gameOverMessage != null)
				gameOverMessage.Dispose(); //ncrunch: no coverage
			interact = new InteractionLogics();
			enemyTexture = new Material(ShaderFlags.Position2DColoredTextured, "EnemyPlane");
			player = new PlayerPlane(new Vector2D(ScreenSpace.Current.Viewport.Left + 0.08f, 0.5f));
			controls = new PlayerControls(player);
			background = new ParallaxBackground(4, layerImageNames, layerScrollFactors);
			background.BaseSpeed = 0.2f;
			player.Destroyed += DisplayGameOverMessage;
			Start<EnemySpawner>();
		}

		public PlayerPlane player;
		internal PlayerControls controls;
		public InteractionLogics interact;
		//internal Material playerTexture;
		internal Material enemyTexture;
		private ParallaxBackground background;

		public void CreateEnemyAtPosition(Vector2D position)
		{
			new EnemyPlane(position);
		}

		private void DisplayGameOverMessage()
		{
			Stop<EnemySpawner>();
			controls.Dispose();
			gameOverMessage = new FontText(Font.Default,
				GetRandomGameOverMessage() + "\n\n [Q] - " + "return to Main Menu.", Rectangle.One);
			backToMenuCommand = new Command(BackToMainMenu).Add(new KeyTrigger(Key.Q));
		}

		private static string GetRandomGameOverMessage()
		{
			var randomSelection = Randomizer.Current.Get(0, 3);
			switch (randomSelection)
			{
			case 0:
				return "Crashed... ~";
			case 1:
				return "Halt! Did you forget the parachute again?";
			case 2:
				return "I guess that's it, right?";
			}
			return "Crashed... ~";
		}

		private FontText gameOverMessage;
		private Command backToMenuCommand;
		private readonly string[] layerImageNames = new[]
		{ "SkyBackground", "Mountains_Back", "Mountains_Middle", "Mountains_Front" };
		private readonly float[] layerScrollFactors = new[] { 0.4f, 0.6f, 1.0f, 1.4f };

		private class EnemySpawner : UpdateBehavior
		{
			public override void Update(IEnumerable<Entity> entities)
			{
				foreach (var entity in entities)
				{
					if (!(GlobalTime.Current.Milliseconds - timeLastOneSpawned > 2000))
						continue;
					var game = entity as Game;
					game.CreateEnemyAtPosition(new Vector2D(ScreenSpace.Current.Viewport.Right,
						ScreenSpace.Current.Viewport.Center.Y + alternating * 0.1f));
					timeLastOneSpawned = GlobalTime.Current.Milliseconds;
					alternating *= -1;
				}
			}

			private float timeLastOneSpawned;
			private int alternating = 1;
		}

		private void BackToMainMenu()
		{
			controls.Dispose();
			Dispose();
			if (gameOverMessage != null)
				gameOverMessage.Dispose();
			if (backToMenuCommand != null)
				backToMenuCommand.Dispose();
			mainMenu.Show();
		}

		public override void Dispose()
		{
			player.Dispose();
			background.Dispose();
			var enemies = EntitiesRunner.Current.GetEntitiesOfType<EnemyPlane>();
			for (int i = 0; i < enemies.Count; i++)
				enemies[i].Dispose();
			var emitters = EntitiesRunner.Current.GetEntitiesOfType<ParticleEmitter>();
			for (int i = 0; i < emitters.Count; i++)
				emitters[i].Dispose();
			base.Dispose();
		}
	}
}