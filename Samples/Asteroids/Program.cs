using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace Asteroids
{
	//ncrunch: no coverage start, this is equal to the default DE app initialization
	internal class Program : App
	{
		public Program()
		{
			new Game(Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
	//ncrunch: no coverage end
}