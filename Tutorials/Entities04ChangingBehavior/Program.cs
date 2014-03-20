using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities04ChangingBehavior
{
	public class Program : App
	{
		public Program()
		{
			new Earth(Vector2D.Half);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}