using System;
using System.Collections.Generic;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.Scenes.Controls;
using DeltaEngine.ScreenSpaces;
using Drench.Logics;

namespace Drench.Games
{
	public abstract class Game : Entity
	{
		protected Game(Logic logic)
		{
			this.logic = logic;
			buttonShader =
				ContentLoader.Create<Shader>(new ShaderCreationData(ShaderFlags.Position2DColoredTextured));
			buttonImage = new Theme().Button.DiffuseMap;
			buttons = new InteractiveButton[logic.Board.Width, logic.Board.Height];
			ArrangeScene();
			ScreenSpace.Current.ViewportSizeChanged += ArrangeScene;
		}

		protected internal readonly Logic logic;
		private readonly Shader buttonShader;
		private readonly Image buttonImage;
		private readonly InteractiveButton[,] buttons;

		protected void ArrangeScene()
		{
			upperText.DrawArea = new Rectangle(0.0f, ScreenSpace.Current.Top, 1.0f, Border);
			lowerText.DrawArea = new Rectangle(0.0f, ScreenSpace.Current.Bottom - Border, 1.0f, Border);
			buttonsLeft = ScreenSpace.Current.Left + Border;
			buttonsTop = ScreenSpace.Current.Top + Border;
			float width = ScreenSpace.Current.Right - ScreenSpace.Current.Left - 2 * Border;
			float height = ScreenSpace.Current.Bottom - ScreenSpace.Current.Top - 2 * Border;
			buttonWidth = width / logic.Board.Width;
			buttonHeight = height / logic.Board.Height;
			CreateButtons();
		}

		internal const float Border = 0.1f;
		internal readonly FontText upperText = new FontText(Font.Default, "", Rectangle.Zero);
		internal readonly FontText lowerText = new FontText(Font.Default, "", Rectangle.Zero);
		private float buttonsLeft;
		private float buttonsTop;
		private float buttonWidth;
		private float buttonHeight;

		private void CreateButtons()
		{
			for (int x = 0; x < logic.Board.Width; x++)
				for (int y = 0; y < logic.Board.Height; y++)
					CreateButton(x, y);
		}

		private void CreateButton(int x, int y)
		{
			if (buttons[x, y] != null)
				buttons[x, y].Dispose();
			var theme = GetCachedButtonThemeOrCreateNewButtonTheme(x, y);
			var drawArea = new Rectangle(buttonsLeft + x * buttonWidth, buttonsTop + y * buttonHeight,
				buttonWidth, buttonHeight);
			buttons[x, y] = new InteractiveButton(theme, drawArea);
			buttons[x, y].Clicked = () => ButtonClicked(x, y);
		}

		private Theme GetCachedButtonThemeOrCreateNewButtonTheme(int x, int y)
		{
			var color = logic.Board.GetColor(x, y);
			return cachedButtonThemes.ContainsKey(color)
				? cachedButtonThemes[color] : CreateButtonTheme(color);
		}

		private readonly Dictionary<Color, Theme> cachedButtonThemes = new Dictionary<Color, Theme>();

		protected virtual Theme CreateButtonTheme(Color color)
		{
			var darkColor = new Color(color.RedValue * 0.7f, color.GreenValue * 0.7f,
				color.BlueValue * 0.7f);
			var lightColor = new Color(color.RedValue * 0.85f, color.GreenValue * 0.85f,
				color.BlueValue * 0.85f);
			var buttonTheme = new Theme
			{
				Button = new Material(buttonShader, buttonImage) { DefaultColor = darkColor },
				ButtonMouseover = new Material(buttonShader, buttonImage) { DefaultColor = lightColor },
				ButtonPressed = new Material(buttonShader, buttonImage) { DefaultColor = color },
				ButtonDisabled = new Material(buttonShader, buttonImage) { DefaultColor = Color.Gray }
			};
			cachedButtonThemes.Add(color, buttonTheme);
			return buttonTheme;
		}

		protected virtual void ButtonClicked(int x, int y)
		{
			if (logic.IsGameOver)
			{
				ExitGame();
				return;
			}
			if (!ProcessDesiredMove(x, y))
				return;
			ArrangeScene();
			if (logic.IsGameOver)
				GameOver();
		}

		private void ExitGame()
		{
			for (int x = 0; x < logic.Board.Width; x++)
				for (int y = 0; y < logic.Board.Height; y++)
					if (buttons[x, y] != null)
						buttons[x, y].Dispose();
			upperText.Dispose();
			lowerText.Dispose();
			if (Exited != null)
				Exited();
		}

		public event Action Exited;
		protected abstract bool ProcessDesiredMove(int x, int y);
		protected abstract void GameOver();
	}
}