using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;

namespace DeltaEngine.Tutorials.Entities07DeactivatingEntities
{
	public class Program : App
	{
		public Program()
		{
			var earth = new Earth(Vector2D.Half);
			new Command(Command.Click, () => earth.IsActive = !earth.IsActive);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}