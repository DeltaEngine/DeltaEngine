using System;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.ScreenSpaces
{
	public class QuadraticScreenSpaceTests
	{
		[Test]
		public void SquareWindowWithQuadraticSpace()
		{
			ScreenSpace.Current = new QuadraticScreenSpace(window);
			window.ViewportPixelSize = new Size(100, 100);
			Assert.AreEqual(Vector2D.Zero, ScreenSpace.Current.TopLeft);
			Assert.AreEqual(Vector2D.One, ScreenSpace.Current.BottomRight);
			Assert.AreEqual(Rectangle.One, ScreenSpace.Current.Viewport);
			Assert.AreEqual(Vector2D.One, ScreenSpace.Current.FromPixelSpace(new Vector2D(100, 100)));
			Assert.AreEqual(Vector2D.Half, ScreenSpace.Current.FromPixelSpace(new Vector2D(50, 50)));
			Assert.IsTrue(
				ScreenSpace.Current.FromPixelSpace(new Rectangle(10, 10, 80, 80)).IsNearlyEqual(
					new Rectangle(0.1f, 0.1f, 0.8f, 0.8f)));
		}

		private readonly Window window = new MockWindow();

		[Test]
		public void ToQuadraticWithUnevenSize()
		{
			window.ViewportPixelSize = new Size(99, 199);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(0.2512563f, 0), screen.TopLeft);
			Assert.AreEqual(new Vector2D(0.7487437f, 1), screen.BottomRight);
			Assert.AreEqual(screen.BottomRight, screen.FromPixelSpace(new Vector2D(99, 199)));
		}

		[Test]
		public void ToQuadraticWithNonSquareWindow()
		{
			window.ViewportPixelSize = new Size(100, 75);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(0, screen.Left);
			Assert.AreEqual(0.125f, screen.Top);
			Assert.AreEqual(1, screen.Right);
			Assert.AreEqual(0.875f, screen.Bottom);
			Assert.AreEqual(new Rectangle(0, 0.125f, 1, 0.75f), screen.Viewport);
			Assert.AreEqual(new Vector2D(1f, 0.875f), screen.FromPixelSpace(new Vector2D(100, 75)));
			Assert.AreEqual(Vector2D.Half, screen.FromPixelSpace(new Vector2D(50, 37.5f)));
			Assert.IsTrue(screen.FromPixelSpace(new Size(10, 10)).IsNearlyEqual(new Size(0.1f, 0.1f)));
		}

		[Test]
		public void ToQuadraticWithPortraitWindow()
		{
			window.ViewportPixelSize = new Size(75, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(0.125f, 0), screen.TopLeft);
			Assert.AreEqual(new Vector2D(0.875f, 1), screen.BottomRight);
			Assert.AreEqual(new Rectangle(0.125f, 0, 0.75f, 1), screen.Viewport);
			Assert.AreEqual(new Vector2D(0.875f, 1f), screen.FromPixelSpace(new Vector2D(75, 100)));
			Assert.AreEqual(Vector2D.Half, screen.FromPixelSpace(new Vector2D(37.5f, 50)));
			Assert.IsTrue(screen.FromPixelSpace(new Size(10, 10)).IsNearlyEqual(new Size(0.1f, 0.1f)));
		}

		[Test]
		public void ToPixelWithSquareWindow()
		{
			window.ViewportPixelSize = new Size(100, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(100, 100), screen.ToPixelSpace(Vector2D.One));
			Assert.AreEqual(Vector2D.Zero, screen.ToPixelSpace(Vector2D.Zero));
			Assert.AreEqual(new Vector2D(50, 50), screen.ToPixelSpace(Vector2D.Half));
		}

		[Test]
		public void ToPixelWithUnevenSizeFromQuadraticSpace()
		{
			window.ViewportPixelSize = new Size(99, 199);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(149, 199), screen.ToPixelSpace(Vector2D.One));
			Assert.AreEqual(new Vector2D(-50, 0), screen.ToPixelSpace(Vector2D.Zero));
			Assert.AreEqual(new Vector2D(49.5f, 99.5f), screen.ToPixelSpace(Vector2D.Half));
			Assert.AreEqual(new Vector2D(50, 100), screen.ToPixelSpaceRounded(Vector2D.Half));
			Assert.AreEqual(new Vector2D(199, 199),
				screen.ToPixelSpaceRounded(Vector2D.One) - screen.ToPixelSpaceRounded(Vector2D.Zero));
		}

		[Test]
		public void ToPixelWithNonSquareWindow()
		{
			window.ViewportPixelSize = new Size(100, 75);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(100, 75), screen.ToPixelSpace(new Vector2D(1f, 0.875f)));
			Assert.AreEqual(Vector2D.Zero, screen.ToPixelSpace(new Vector2D(0, 0.125f)));
			Assert.AreEqual(new Vector2D(50, 37.5f), screen.ToPixelSpace(Vector2D.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
			Assert.IsTrue(
				screen.ToPixelSpace(new Rectangle(0.2f, 0.2f, 0.6f, 0.6f)).IsNearlyEqual(new Rectangle(20,
					7.5f, 60, 60)));
		}

		[Test]
		public void ToPixelWithPortraitWindow()
		{
			window.ViewportPixelSize = new Size(75, 100);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(75, 100), screen.ToPixelSpace(new Vector2D(0.875f, 1f)));
			Assert.AreEqual(Vector2D.Zero, screen.ToPixelSpace(new Vector2D(0.125f, 0)));
			Assert.AreEqual(new Vector2D(37.5f, 50), screen.ToPixelSpace(Vector2D.Half));
			Assert.AreEqual(new Size(10, 20), screen.ToPixelSpace(new Size(0.1f, 0.2f)));
		}

		[Test]
		public void ToPixelInFullHdResolution()
		{
			window.ViewportPixelSize = new Size(1920, 1080);
			var screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(new Vector2D(1680, 1500), screen.ToPixelSpace(new Vector2D(0.875f, 1f)));
			var somePoint = screen.FromPixelSpace(new Vector2D(324, 483));
			var somePointPlusOne = screen.FromPixelSpace(new Vector2D(325, 483));
			Assert.IsFalse(somePoint.X.IsNearlyEqual(somePointPlusOne.X),
				somePoint + " should not be nearly equal to " + somePointPlusOne);
			Assert.AreEqual(new Vector2D(324, 483), screen.ToPixelSpaceRounded(somePoint));
			Assert.AreEqual(new Vector2D(325, 483), screen.ToPixelSpaceRounded(somePointPlusOne));
		}

		[Test]
		public void GetInnerPoint()
		{
			window.ViewportPixelSize = new Size(800, 600);
			ScreenSpace screen = new QuadraticScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPosition(Vector2D.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPosition(Vector2D.One));
		}

		[Test]
		public void TestViewportSizeChanged()
		{
			window.ViewportPixelSize = new Size(800, 800);
			var screen = new QuadraticScreenSpace(window);
			Action checkSize = delegate { Assert.AreEqual(Rectangle.One, screen.Viewport); };
			screen.ViewportSizeChanged += checkSize;
			window.ViewportPixelSize = new Size(800, 800);
			screen.ViewportSizeChanged -= checkSize;
		}

		[Test]
		public void TestAspectRatio()
		{
			var screen = new QuadraticScreenSpace(window);
			window.ViewportPixelSize = new Size(800, 800);
			Assert.AreEqual(1f, screen.AspectRatio);
			window.ViewportPixelSize = new Size(1920, 1080);
			Assert.AreEqual(0.5625f, screen.AspectRatio);
		}
	}
}