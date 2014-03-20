using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Entities;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities05Tags
{
	public class Program : App
	{
		public Program()
		{
			new Earth(new Vector2D(0.3f, 0.5f)).AddTag("EarthToggle");
			new Earth(new Vector2D(0.5f, 0.5f)).AddTag("EarthToggle");
			new Earth(new Vector2D(0.7f, 0.5f)).AddTag("EarthNormal");
			new Command(Command.Click, () =>
			{
				foreach (Earth entity in EntitiesRunner.Current.GetEntitiesWithTag("EarthToggle"))
					entity.IsPlaying = !entity.IsPlaying;
			});
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}