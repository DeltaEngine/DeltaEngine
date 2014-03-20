using DeltaEngine.Core;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;

namespace CreepyTowers
{
	/// <summary>
	/// CreepyTowers Tower Defense game
	/// </summary>
	internal class Program : App
	{
		public Program()
		{
			new Game(Resolve<Window>(), Resolve<SoundDevice>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}