using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities02AttachingBehavior
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