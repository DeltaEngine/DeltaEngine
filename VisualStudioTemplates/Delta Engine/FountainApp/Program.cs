using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace $safeprojectname$
{
	public class Program : App
	{
		public Program()
		{
			new ParticleFountain(new Vector2D(0.5f, 0.6f));
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}