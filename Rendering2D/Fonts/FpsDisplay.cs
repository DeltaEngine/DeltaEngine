using DeltaEngine.Core;
using DeltaEngine.Entities;
using DeltaEngine.ScreenSpaces;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Small simple helper class to display the fps on the top middle of the screen for tests
	/// </summary>
	public class FpsDisplay : FontText, Updateable
	{
		//ncrunch: no coverage start
		public FpsDisplay()
			: base(Font.Default, "", ScreenSpace.Current.Viewport)
		{
			VerticalAlignment = VerticalAlignment.Top;
		}

		public void Update()
		{
			DrawArea = ScreenSpace.Current.Viewport;
			Text = "Fps = " + GlobalTime.Current.Fps;
		}
	}
}