using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	public class Program : App
	{
		public Program()
		{
			Settings.Current.Resolution = new Size(1280, 720);
			var startupScreen = new StartupScreen();
			var gameScreen = new GameScreen();
			new Game(startupScreen, gameScreen);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}