using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	/// <summary>
	/// Knits the lower level objects together by feeding their events to each other
	/// </summary>
	public class GameCoordinator
	{
		public GameCoordinator(Window window)
		{
			var screenSpace = new Camera2DScreenSpace(window);
			screenSpace.LookAt = Vector2D.Half;
			DisplayIntroLogo();
			rabbitMatrix = new RabbitGrid(20, 12,
				new Rectangle(ScreenSpace.Current.Left + 0.05f, ScreenSpace.Current.Top + 0.1f,
					ScreenSpace.Current.Viewport.Width - 0.1f, ScreenSpace.Current.Viewport.Height - 0.15f));
			userInterface = new UserInterface();
			userInterface.Money = 50;
			rabbitMatrix.MoneyEarned += money => userInterface.Money += money;
			rabbitMatrix.RabbitKilled += () => userInterface.Kills++;
			userInterface.DidDamage += rabbitMatrix.DoDamage;
			window.ViewportSizeChanged += size => rabbitMatrix.RecalculateRabbitPositionsAndSizes(new Rectangle(
				ScreenSpace.Current.Left + 0.05f, ScreenSpace.Current.Top + 0.1f,
				ScreenSpace.Current.Viewport.Width - 0.1f, ScreenSpace.Current.Viewport.Height - 0.15f));
			RespondToInput();
		}

		private readonly UserInterface userInterface;

		private static void DisplayIntroLogo()
		{
			var introLogo = new FadeSprite(ContentLoader.Load<Material>("MaterialGameLogo"),
				Rectangle.FromCenter(Vector2D.Half, new Size(0.33f, 0.24f)), 5.0f);
			introLogo.RenderLayer = (int)RenderLayers.IntroLogo;
		}

		private readonly RabbitGrid rabbitMatrix;

		internal void RespondToInput()
		{
			new Command(userInterface.UpdateDisplayedItemPosition).Add(new MouseMovementTrigger());
			new Command(Command.Click, MouseButtonPress);
		}

		private void MouseButtonPress(Vector2D position)
		{
			if (!rabbitMatrix.IsGameOver())
				userInterface.HandleItemInGame(position);
		}
	}
}