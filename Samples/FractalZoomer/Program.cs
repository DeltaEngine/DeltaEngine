using DeltaEngine.Core;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;

namespace FractalZoomer
{
	internal class Program : App
	{
		public Program()
		{
			new FpsDisplay();
			new Zoomer(Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}