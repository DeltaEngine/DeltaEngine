using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Tutorials.Basic07Fonts
{
	public class Program : App
	{
		public Program()
		{
			new FontText(Font.Default, "Hi there", Rectangle.HalfCentered);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}