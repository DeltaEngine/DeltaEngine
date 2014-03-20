using DeltaEngine.Commands;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Shapes;

namespace DeltaEngine.Tutorials.Basic02DrawLine
{
	public class Program : App
	{
		public Program()
		{
			var line = new Line2D(Vector2D.Zero, Vector2D.One, Color.Red);
			new Command("Click", () => line.Color = Color.Yellow);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}