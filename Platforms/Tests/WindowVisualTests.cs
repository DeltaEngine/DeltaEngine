using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Shapes;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class WindowVisualTests : TestWithMocksOrVisually
	{
		[SetUp]
		public void CreateWindow()
		{
			window = Resolve<Window>();
		}

		private Window window;

		[Test]
		public void ShowCursorAndToggleHideWhenClicking()
		{
			bool showCursor = true;
			new Command(() => window.ShowCursor = showCursor = !showCursor).Add(new MouseButtonTrigger());
		}

		//ncrunch: no coverage start
		[Test, Ignore]
		public void ChangeCursorIcon()
		{
			window.SetCursorIcon("TestCursor.cur");
		}

		[Test, Category("Slow")]
		public void ShowColoredEllipse()
		{
			new Ellipse(new Rectangle(Vector2D.Half, Size.One / 4), Color.Red);
		}
	}
}