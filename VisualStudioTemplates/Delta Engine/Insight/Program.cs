using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	public class Program : App
	{
		public Program()
		{
			new StatisticsApp(Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}