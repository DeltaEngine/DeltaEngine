using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Basic05RotatingSprite
{
	public class Program : App
	{
		public Program()
		{
			new Sprite(ContentLoader.Load<Material>("Logo"), Rectangle.HalfCentered).StartRotating(
				Randomizer.Current.Get(-50, 50));
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}