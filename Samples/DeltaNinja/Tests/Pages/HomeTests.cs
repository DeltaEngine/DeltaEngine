using DeltaEngine.Core;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using DeltaNinja.Pages;
using NUnit.Framework;

namespace DeltaNinja.Tests.Pages
{
	internal class HomeTests : TestWithMocksOrVisually
	{
		[Test]
		public void CreateHome()
		{
			var screen = Resolve<ScreenSpace>();
			var home = new HomePage(screen);
			home.Show();
			home.ButtonClicked += (x) => { Resolve<Window>().Title = "Clicked: " + x.ToString(); };
		}
	}
}