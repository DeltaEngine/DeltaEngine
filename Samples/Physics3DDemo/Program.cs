using DeltaEngine.Core;
using DeltaEngine.Physics3D;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;

namespace Physics3DDemo
{
	public class Program : App
	{
		public Program()
		{
			new FpsDisplay();
			new Game(Resolve<Physics>(), Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}