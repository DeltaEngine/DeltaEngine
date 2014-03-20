using DeltaEngine.Commands;
using DeltaEngine.Content;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.Rendering2D;

namespace DeltaEngine.Tutorials.Entities10LogoRacer
{
	public class Program : App
	{
		public Program()
		{
			new Sprite(ContentLoader.Load<Material>("Road"), Rectangle.One).StartMovingUV(new Vector2D(
				0, -0.8f));
			new ScoreDisplay(new Player(), new EnemySpawner());
			new Command(Command.Exit, Resolve<Window>().CloseAfterFrame);
		}

		public static void Main()
		{
			new Program().Run();
		}
	}
}