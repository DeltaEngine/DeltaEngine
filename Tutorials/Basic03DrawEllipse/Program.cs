using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Tutorials.Basic03DrawEllipse
{
	public class Program : App
	{
		public Program()
		{
			new Ellipse(Vector2D.Half, 0.4f, 0.2f, Color.Red);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}