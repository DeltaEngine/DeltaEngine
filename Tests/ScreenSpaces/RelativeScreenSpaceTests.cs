using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.ScreenSpaces
{
	public class RelativeScreenSpaceTests
	{
		[Test]
		public void SquareWindowWithRelativeSpace()
		{
			SquareWindowShouldAlwaysReturnRelativeValues(new RelativeScreenSpace(window));
		}

		private readonly Window window = new MockWindow();

		private void SquareWindowShouldAlwaysReturnRelativeValues(ScreenSpace screen)
		{
			window.ViewportPixelSize = new Size(100, 100);
			Assert.AreEqual(Vector2D.Zero, screen.TopLeft);
			Assert.AreEqual(Vector2D.One, screen.BottomRight);
			Assert.AreEqual(Rectangle.One, screen.Viewport);
			Assert.AreEqual(Vector2D.One, screen.FromPixelSpace(new Vector2D(100, 100)));
			Assert.AreEqual(Vector2D.Half, screen.FromPixelSpace(new Vector2D(50, 50)));
			Assert.IsTrue(
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)).IsNearlyEqual(new Rectangle(0.1f, 0.1f,
					0.8f, 0.8f)));
		}

		[Test]
		public void GetInnerPoint()
		{
			window.ViewportPixelSize = new Size(800, 600);
			ScreenSpace screen = new RelativeScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPosition(Vector2D.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPosition(Vector2D.One));
		}

		[Test]
		public void ToRelativeWithUnevenSize()
		{
			window.ViewportPixelSize = new Size(99, 199);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(Vector2D.Zero, screen.TopLeft);
			Assert.AreEqual(Vector2D.One, screen.BottomRight);
			Assert.AreEqual(screen.BottomRight, screen.FromPixelSpace(new Vector2D(99, 199)));
		}

		[Test]
		public void ToPixelSpaceFromRelativeSpace()
		{
			window.ViewportPixelSize = new Size(30, 50);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(new Vector2D(30, 50), screen.ToPixelSpace(Vector2D.One));
			Assert.AreEqual(Size.Zero, screen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(new Vector2D(10, 20), screen.ToPixelSpace(new Vector2D(10 / 30.0f, 20 / 50.0f)));
			Assert.AreEqual(new Size(7.5f, 12.5f), screen.ToPixelSpace(new Size(0.25f)));
		}

		[Test]
		public void NonSquareWindowWithRelativeSpace()
		{
			window.ViewportPixelSize = new Size(100, 75);
			var screen = new RelativeScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(0.0f, screen.Top);
			Assert.AreEqual(1.0f, screen.Right);
			Assert.AreEqual(1.0f, screen.Bottom);
		}
	}
}