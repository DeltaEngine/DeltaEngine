using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Multimedia;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D.Fonts;

namespace DeltaEngine.Tutorials.Basic08PlaySound
{
	public class Program : App
	{
		public Program()
		{
			var sound = ContentLoader.Load<Sound>("DefaultSound");
			new FontText(Font.Default, "Click to play sound", Rectangle.HalfCentered);
			new Command(Command.Click, () => sound.Play());
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}