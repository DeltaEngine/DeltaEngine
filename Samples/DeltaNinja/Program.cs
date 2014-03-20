using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;

namespace DeltaNinja
{
	internal class Program : App
	{
		public Program()
		{
			Settings.Current.Resolution = new Size(1280, 720);
			new Game(Resolve<ScreenSpace>(), Resolve<Window>());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}