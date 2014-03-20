using DeltaEngine.Physics2D;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;

namespace Physics2DDemo
{
	public class Program : App
	{
		public Program()
		{
			new FpsDisplay();
			new Game(Resolve<Physics>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}