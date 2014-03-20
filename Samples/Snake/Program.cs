using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace Snake
{
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
}