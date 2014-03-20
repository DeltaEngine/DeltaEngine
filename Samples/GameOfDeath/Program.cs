using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace GameOfDeath
{
	/// <summary>
	/// This game is from our first Game Jam early in 2013. It is a spinoff of the good old
	/// Game of Life, but with the twist to kill multiplying rabbits quickly.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			new GameCoordinator(Resolve<Window>());
		}

		private static void Main()
		{
			new Program().Run();
		}
	}
}