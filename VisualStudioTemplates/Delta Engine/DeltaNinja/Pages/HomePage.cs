using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$.Pages
{
	public class HomePage : BasePage
	{
		public HomePage(ScreenSpace screen)
			: base(screen)
		{
			SetTitle("DeltaNinjaHome", 0.4f, 3f, 0f);
			AddLogoLink("DeltaEngineLink", "http://deltaengine.net/", 0.07f, -2);
			AddLogoLink("CodePlexLink", "http://deltaninja.codeplex.com/", 0.07f, 2);
			AddButton(MenuButton.NewGame, 0.2f, 4f);
			AddButton(MenuButton.About, 0.2f, 4f);
			AddButton(MenuButton.Exit, 0.2f, 4f);
			new Command(CheckAboutBox).Add(new MouseButtonTrigger());
			aboutBox = new Sprite("AboutBox",
				Rectangle.FromCenter(Vector2D.Half, new Size(0.38f, 0.38f * 0.6070f)));
			aboutBox.RenderLayer = 9000;
			aboutBox.IsActive = false;
		}

		private readonly Sprite aboutBox;

		protected override void OnButtonClicked(MenuButton code)
		{
			if (code == MenuButton.About)
			{
				aboutBox.IsActive = true;
				return;
			}
			base.OnButtonClicked(code);
		}

		private void CheckAboutBox(Vector2D position)
		{
			if (aboutBox.IsActive)
				if (aboutBox.DrawArea.Contains(position))
					aboutBox.IsActive = false;
		}
	}
}