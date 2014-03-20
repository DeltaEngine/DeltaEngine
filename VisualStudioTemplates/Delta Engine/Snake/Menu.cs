using System;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.Scenes;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class Menu : Scene
	{
		public Menu()
		{
			CreateMenuTheme();
			gameColors = new[] { Color.Black, Color.PaleGreen, Color.Green, Color.Gold };
			AddStartButton();
			AddColorsButton();
			AddHowToPlay();
			AddQuitButton();
		}

		private void CreateMenuTheme()
		{
			SetViewportBackground("SnakeMainMenuBackground");
			menuTheme = new Theme
			{
				Button = new Material(ShaderFlags.Position2DTextured, "SnakeButtonDefault"),
				ButtonMouseover = new Material(ShaderFlags.Position2DTextured, "SnakeButtonHover"),
				ButtonPressed = new Material(ShaderFlags.Position2DTextured, "SnakeButtonPressed")
			};
		}

		private Theme menuTheme;

		private void AddStartButton()
		{
			var startButton = new InteractiveButton(menuTheme, new Rectangle(0.3f, 0.1f, 0.4f, 0.15f),
				"Start Game");
			startButton.Clicked += TryInvokeGameStart;
			Add(startButton);
		}

		//ncrunch: no coverage start
		private void TryInvokeGameStart()
		{
			colorOptions.Hide();
			if (InitGame != null)
				InitGame();
		}
		//ncrunch: no coverage end

		public event Action InitGame;

		private void AddQuitButton()
		{
			var quitButton = new InteractiveButton(menuTheme, new Rectangle(0.3f, 0.7f, 0.4f, 0.15f),
				"Quit");
			quitButton.Clicked += TryInvokeQuit;
			Add(quitButton);
		}

		//ncrunch: no coverage start
		private void TryInvokeQuit()
		{
			if (Quit != null)
				Quit();
		}
		//ncrunch: no coverage end

		public event Action Quit;

		//ncrunch: no coverage start
		private void AddColorsButton()
		{
			var colorButton = new InteractiveButton(menuTheme, new Rectangle(0.3f, 0.3f, 0.4f, 0.15f),
				"ChooseColours");
			colorButton.Clicked += () =>
			{
				Hide();
				colorOptions.Show();
			};
			Add(colorButton);
			CreateColorOptions();
		}
		//ncrunch: no coverage end

		public Color[] gameColors;

		private void CreateColorOptions()
		{
			colorOptions = new ColorOptionsMenu(this);
		}

		private ColorOptionsMenu colorOptions;

		private class ColorOptionsMenu : Scene
		{
			public ColorOptionsMenu(Menu parentMenu)
			{
				this.parentMenu = parentMenu;
				SetViewportBackground("SnakeMainMenuBackground");
				AddBackgroundAndColorDisplay();
				AddGameElementSelection();
				AddColorSliders();
				AddDoneButton();
			}

			private readonly Menu parentMenu;

			private void AddBackgroundAndColorDisplay()
			{
				Add(new FilledRect(new Rectangle(0.15f, 0.15f, 0.7f, 0.7f), Color.DarkGray)
				{
					RenderLayer = 4
				});
				currentColorShown = new FilledRect(new Rectangle(0.2f, 0.45f, 0.1f, 0.1f),
					parentMenu.gameColors[currentColorIndex]) { RenderLayer = 5 };
				Add(currentColorShown);
			}

			//ncrunch: no coverage start
			private void AddGameElementSelection()
			{
				var backgroundButton = new InteractiveButton(parentMenu.menuTheme,
					new Rectangle(0.2f, 0.2f, 0.12f, 0.07f), "Background") { RenderLayer = 5 };
				backgroundButton.Clicked += () =>
				{
					currentColorIndex = 0;
					UpdateColorDisplay();
					UpdateSliderValues();
				};
				Add(backgroundButton);

				var borderButton = new InteractiveButton(parentMenu.menuTheme,
					new Rectangle(0.35f, 0.2f, 0.12f, 0.07f), "Border") { RenderLayer = 5 };
				borderButton.Clicked += () =>
				{
					currentColorIndex = 1;
					UpdateColorDisplay();
					UpdateSliderValues();
				};
				Add(borderButton);

				var snakeButton = new InteractiveButton(parentMenu.menuTheme,
					new Rectangle(0.5f, 0.2f, 0.12f, 0.07f), "Snake") { RenderLayer = 5 };
				snakeButton.Clicked += () =>
				{
					currentColorIndex = 2;
					UpdateColorDisplay();
					UpdateSliderValues();
				};
				Add(snakeButton);

				var chunkButton = new InteractiveButton(parentMenu.menuTheme,
					new Rectangle(0.65f, 0.2f, 0.12f, 0.07f), "Chunk") { RenderLayer = 5 };
				chunkButton.Clicked += () =>
				{
					currentColorIndex = 3;
					UpdateColorDisplay();
					UpdateSliderValues();
				};
				Add(chunkButton);
			}

			private void AddColorSliders()
			{
				sliderRed = new Slider(parentMenu.menuTheme, new Rectangle(0.4f, 0.3f, 0.4f, 0.1f))
				{
					RenderLayer = 5,
					MaxValue = 255,
					MinValue = 0
				};
				sliderRed.ValueChanged += value =>
				{
					parentMenu.gameColors[currentColorIndex].R = (byte)value;
					UpdateColorDisplay();
				};
				Add(sliderRed);

				sliderGreen = new Slider(parentMenu.menuTheme, new Rectangle(0.4f, 0.45f, 0.4f, 0.1f))
				{
					RenderLayer = 5,
					MaxValue = 255,
					MinValue = 0
				};
				sliderGreen.ValueChanged += value =>
				{
					parentMenu.gameColors[currentColorIndex].G = (byte)value;
					UpdateColorDisplay();
				};
				Add(sliderGreen);

				sliderBlue = new Slider(parentMenu.menuTheme, new Rectangle(0.4f, 0.6f, 0.4f, 0.1f))
				{
					RenderLayer = 5,
					MaxValue = 255,
					MinValue = 0
				};
				sliderBlue.ValueChanged += value =>
				{
					parentMenu.gameColors[currentColorIndex].B = (byte)value;
					UpdateColorDisplay();
				};
				Add(sliderBlue);

				UpdateSliderValues();
			}

			private Slider sliderRed;
			private Slider sliderGreen;
			private Slider sliderBlue;

			private void AddDoneButton()
			{
				var doneButton = new InteractiveButton(parentMenu.menuTheme,
					new Rectangle(0.4f, 0.72f, 0.2f, 0.1f), "Done!") { RenderLayer = 5 };
				doneButton.Clicked += () =>
				{
					Hide();
					parentMenu.Show();
				};
				Add(doneButton);
				Hide();
			}

			private void UpdateColorDisplay()
			{
				currentColorShown.Color = parentMenu.gameColors[currentColorIndex];
			}

			private void UpdateSliderValues()
			{
				sliderRed.Value = parentMenu.gameColors[currentColorIndex].R;
				sliderGreen.Value = parentMenu.gameColors[currentColorIndex].G;
				sliderBlue.Value = parentMenu.gameColors[currentColorIndex].B;
			}

			private int currentColorIndex;
			private FilledRect currentColorShown;
		}
		//ncrunch: no coverage end

		private void AddHowToPlay()
		{
			var howToButton = new InteractiveButton(menuTheme, new Rectangle(0.3f, 0.5f, 0.4f, 0.15f),
				"How To Play");
			howToButton.Clicked += ShowHowToPlaySubMenu;
			Add(howToButton);
		}

		//ncrunch: no coverage start
		private void ShowHowToPlaySubMenu()
		{
			if(howToPlay == null)
				howToPlay = new HowToPlaySubMenu(this, menuTheme);
			howToPlay.Show();
			Hide();
		}

		private HowToPlaySubMenu howToPlay;

		private sealed class HowToPlaySubMenu : Scene
		{
			public HowToPlaySubMenu(Scene parent, Theme menuTheme)
			{
				this.parent = parent;
				this.menuTheme = menuTheme;
				SetViewportBackground("SnakeMainMenuBackground");
				AddControlDescription();
				AddBackButton();
			}

			private readonly Scene parent;
			private readonly Theme menuTheme;

			private void AddControlDescription()
			{
				const string DescriptionText = "Collect the chunks lying around separately\n" +
					"by moving the head of the snake over them\n" + 
					" so they can be attached to the end\n\n" +
					"Now - How to move around...\n\n" +
						"You can either use cursor keys or a gamepad controller,\n" +
						"in case you have one at your disposal,\n\n" +
						"or you may click / tap on a position that,\n" +
						"relatively to the foremost part of the snake,\n" +
						"is towards your desired movement direction";
				var howToDisplayText =
					new FontText(Font.Default, DescriptionText, Rectangle.One) { Color = Color.Black };
				Add(howToDisplayText);
			}

			private void AddBackButton()
			{
				var backButton = new InteractiveButton(menuTheme,
					new Rectangle(0.3f, ScreenSpace.Current.Bottom - 0.15f, 0.4f, 0.1f), "Back");
				backButton.Clicked += () =>
				{
					Hide();
					parent.Show();
				};
				Add(backButton);
			}
		}
		//ncrunch: no coverage end
	}
}