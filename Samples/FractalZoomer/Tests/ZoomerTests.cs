using DeltaEngine.Core;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace FractalZoomer.Tests
{
	public class ZoomerTests : TestWithMocksOrVisually
	{
		[Test]
		public void ShowInitialZoomer()
		{
			new Zoomer(Resolve<Window>());
		}
	}
}