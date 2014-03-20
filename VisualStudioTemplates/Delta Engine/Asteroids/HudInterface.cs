using System.Globalization;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Rendering2D.Fonts;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$
{
	public class HudInterface
	{
		public HudInterface()
		{
			ScoreDisplay = new FontText(ContentLoader.Load<Font>("Tahoma30"), "0",
				new Rectangle(ScreenSpace.Current.Viewport.Left, ScreenSpace.Current.Viewport.Top + 0.01f,
					0.1f, 0.05f));
			ScoreDisplay.RenderLayer = (int)AsteroidsRenderLayer.UserInterface;
			GameOverText = new FontText(ContentLoader.Load<Font>("Verdana12"), "", Rectangle.FromCenter(0.5f, 0.5f, 0.8f, 0.4f));
			GameOverText.RenderLayer = (int)AsteroidsRenderLayer.UserInterface;
		}

		public FontText ScoreDisplay { get; private set; }

		public void SetScoreText(int score)
		{
			ScoreDisplay.Text = score.ToString(CultureInfo.InvariantCulture);
		}

		public void SetGameOverText()
		{
			GameOverText.Text = "Game Over!\n\n[Space] / Controller (A) - Restart\n[Esc] / Controller (B)- Back to Menu";
			GameOverText.IsVisible = true;
		}

		public FontText GameOverText { get; private set; }

		public void SetInGameMode()
		{
			GameOverText.IsVisible = false;
		}

		public void Dispose()
		{
			GameOverText.Dispose();
			ScoreDisplay.Dispose();
		}
	}
}