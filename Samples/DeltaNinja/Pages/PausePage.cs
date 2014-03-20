using DeltaEngine.ScreenSpaces;

namespace DeltaNinja.Pages
{
	class PausePage : BasePage
	{
		public PausePage(ScreenSpace screen)
			: base(screen)
		{
			SetViewportBackground("PauseBackground");
			SetTitle("Pause", 0.25f, 4f, 0.05f);
			AddButton(MenuButton.Resume, 0.2f, 4f);
			AddButton(MenuButton.NewGame, 0.2f, 4f);
			AddButton(MenuButton.Abort, 0.2f, 4f);						
		} 
	}
}