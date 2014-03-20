using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities06Triggers
{
	public class Program : App
	{
		public Program()
		{
			var random = Randomizer.Current;
			for (int num = 0; num < 3; num++)
				new Earth(new Vector2D(random.Get(0.2f, 0.8f), random.Get(0.3f, 0.7f)),
					new Vector2D(random.Get(-0.4f, 0.4f), random.Get(-0.3f, 0.3f)));
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}