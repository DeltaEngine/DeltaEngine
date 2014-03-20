using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	internal class Program : App
	{
		//ncrunch: no coverage start
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