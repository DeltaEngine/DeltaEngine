using DeltaEngine.Core;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	/// <summary>
	/// Just starts the ColorChanger class. For more complex examples see the other sample games.
	/// </summary>
	public class Program : App
	{
		public Program()
		{
			new ColorChanger(Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}