using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Basic01CreateWindow
{
	public class Program : App
	{
		public Program()
		{
			Resolve<Window>().BackgroundColor = Color.CornflowerBlue;
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}