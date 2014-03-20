using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.ScreenSpaces
{
	public class PixelScreenSpaceTests
	{
		[SetUp]
		public void CreateWindow()
		{
			window = new MockWindow { ViewportPixelSize = new Size(800, 600) };
		}

		private Window window;

		[Test]
		public void SquareWindowWithPixelSpace()
		{
			var screen = new PixelScreenSpace(window);
			Assert.AreEqual(Vector2D.Zero, screen.TopLeft);
			Assert.AreEqual(window.ViewportPixelSize, (Size)screen.BottomRight);
			Assert.AreEqual(new Rectangle(Vector2D.Zero, window.TotalPixelSize), screen.Viewport);
			Assert.AreEqual(new Vector2D(100, 100), screen.FromPixelSpace(new Vector2D(100, 100)));
			Assert.AreEqual(new Rectangle(10, 10, 80, 80),
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)));
			window.CloseAfterFrame();
		}

		[Test]
		public void GetInnerPoint()
		{
			ScreenSpace screen = new PixelScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPosition(Vector2D.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPosition(Vector2D.One));
			window.CloseAfterFrame();
		}

		[Test]
		public void ToPixelSpaceAndFromPixelSpace()
		{
			var pixelScreen = new PixelScreenSpace(window);
			Assert.AreEqual(pixelScreen.TopLeft, pixelScreen.ToPixelSpace(pixelScreen.TopLeft));
			Assert.AreEqual(pixelScreen.BottomRight, pixelScreen.ToPixelSpace(pixelScreen.BottomRight));
			Assert.AreEqual(Size.Zero, pixelScreen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(Size.One, pixelScreen.ToPixelSpace(Size.One));
			window.CloseAfterFrame();
		}

		[Test]
		public void NonSquareWindowWithPixelSpace()
		{
			var screen = new PixelScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(0.0f, screen.Top);
			Assert.AreEqual(800.0f, screen.Right);
			Assert.AreEqual(600.0f, screen.Bottom);
			window.CloseAfterFrame();
		}

		[Test]
		public void ChangingWindowViewportPixelSizeWillAlsoAffectTotalPixelSize()
		{
			Size border = window.TotalPixelSize - window.ViewportPixelSize;
			Size newViewportSize = new Size(400, 300);
			window.ViewportPixelSize = newViewportSize;
			Assert.AreEqual(newViewportSize, window.ViewportPixelSize);
			Assert.AreEqual(newViewportSize + border, window.TotalPixelSize);
			window.CloseAfterFrame();
		}

		[Test]
		public void MoveWindow()
		{
			window.PixelPosition = new Vector2D(100, 200);
			Assert.AreEqual(new Vector2D(100, 200), window.PixelPosition);
			window.CloseAfterFrame();
		}

		[Test]
		public void SquareWindow()
		{
			var pixelScreen = new PixelScreenSpace(window);
			window.ViewportPixelSize = new Size(100, 100);
			Assert.AreEqual(Vector2D.Zero, pixelScreen.TopLeft);
			Assert.AreEqual(new Vector2D(100, 100), pixelScreen.BottomRight);
			Assert.AreEqual(new Rectangle(0, 0, 100, 100), pixelScreen.Viewport);
			Assert.AreEqual(new Vector2D(100, 100), pixelScreen.FromPixelSpace(new Vector2D(100, 100)));
			Assert.AreEqual(new Vector2D(50, 50), pixelScreen.FromPixelSpace(new Vector2D(50, 50)));
		}
	}
}