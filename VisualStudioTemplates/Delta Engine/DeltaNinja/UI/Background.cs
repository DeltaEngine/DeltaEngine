using DeltaEngine.Rendering2D;
using DeltaEngine.ScreenSpaces;

namespace $safeprojectname$.UI
{
	internal class Background : Sprite
	{
		public Background(ScreenSpace screen)
			: base("Background", screen.Viewport)
		{
			RenderLayer = (int)GameRenderLayer.Background;
			screen.ViewportSizeChanged += () => DrawArea = screen.Viewport;
		}
	}
}