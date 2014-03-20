using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Mocks;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Tests.ScreenSpaces
{
	public class InvertedScreenSpaceTests
	{
		[Test]
		public void SquareWindowWithInvertedSpace()
		{
			window.ViewportPixelSize = new Size(100, 100);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(Vector2D.UnitY, screen.TopLeft);
			Assert.AreEqual(Vector2D.UnitX, screen.BottomRight);
			Assert.AreEqual(new Rectangle(Vector2D.UnitY, new Size(1, -1)), screen.Viewport);
			Assert.IsTrue(screen.FromPixelSpace(new Vector2D(100, 100)).IsNearlyEqual(Vector2D.UnitX));
			Assert.IsTrue(
				screen.FromPixelSpace(new Rectangle(10, 10, 80, 80)).IsNearlyEqual(new Rectangle(0.1f, 0.9f,
					0.8f, 0.8f)));
		}

		private readonly Window window = new MockWindow();

		[Test]
		public void GetInnerPoint()
		{
			window.ViewportPixelSize = new Size(800, 600);
			ScreenSpace screen = new InvertedScreenSpace(window);
			Assert.AreEqual(screen.TopLeft, screen.GetInnerPosition(Vector2D.Zero));
			Assert.AreEqual(screen.BottomRight, screen.GetInnerPosition(Vector2D.One));
		}

		[Test]
		public void ToPixelSpaceAndFromInvertedSpace()
		{
			window.ViewportPixelSize = new Size(75, 100);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(Vector2D.Zero, screen.ToPixelSpace(screen.TopLeft));
			Assert.AreEqual(new Vector2D(75, 100), screen.ToPixelSpace(screen.BottomRight));
			Assert.AreEqual(Size.Zero, screen.ToPixelSpace(Size.Zero));
			Assert.AreEqual(new Size(75, 100), screen.ToPixelSpace(Size.One));
		}

		[Test]
		public void NonSquareWindowWithInvertedSpace()
		{
			window.ViewportPixelSize = new Size(100, 75);
			var screen = new InvertedScreenSpace(window);
			Assert.AreEqual(0.0f, screen.Left);
			Assert.AreEqual(1.0f, screen.Top);
			Assert.AreEqual(1.0f, screen.Right);
			Assert.AreEqual(0.0f, screen.Bottom);
		}
	}
}