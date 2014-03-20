using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FindTheWord.Tests
{
	internal class GameScreenTests : TestWithMocksOrVisually
	{
		[Test, Ignore]
		public void ShowScreen()
		{
			Resolve<Window>().ViewportPixelSize = new Size(1280, 800);
			var screen = new GameScreen();
			screen.FadeIn();
			screen.StartNextLevel();
		}
	}
}