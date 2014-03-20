using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities01OwnEntity
{
	public class Program : App
	{
		public Program()
		{
			new Earth(new Vector2D(0.4f, 0.5f));
			var secondEarth = new Earth(new Vector2D(0.6f, 0.5f));
			new Command(Command.Click, () => secondEarth.IsPlaying = !secondEarth.IsPlaying);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}