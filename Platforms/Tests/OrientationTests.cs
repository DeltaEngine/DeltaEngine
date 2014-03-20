using DeltaEngine.Commands;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Input;
using DeltaEngine.Rendering2D.Shapes;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Platforms.Tests
{
	public class OrientationTests : TestWithMocksOrVisually
	{
		//ncrunch: no coverage start
		[Test, Category("Slow")]
		public void TestDrawAreaWhenChangingOrientationToPortrait()
		{
			Resolve<Window>().ViewportPixelSize = new Size(480, 800);
			new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red);
			RunAfterFirstFrame(() =>
			{
				var screen = ScreenSpace.Current;
				Assert.AreEqual(typeof(QuadraticScreenSpace), screen.GetType());
				var quadSize = screen.FromPixelSpace(new Vector2D(0, 0));
				ArePointsNearlyEqual(new Vector2D(0.2f, 0f), quadSize);
				quadSize = screen.FromPixelSpace(new Vector2D(480, 800));
				ArePointsNearlyEqual(new Vector2D(0.8f, 1), quadSize);
				var pixelSize = screen.ToPixelSpace(new Vector2D(0.2f, 0));
				ArePointsNearlyEqual(Vector2D.Zero, pixelSize);
				pixelSize = screen.ToPixelSpace(new Vector2D(0.8f, 1));
				ArePointsNearlyEqual(new Vector2D(480, 800), pixelSize);
			});
		}

		private static void ArePointsNearlyEqual(Vector2D expected, Vector2D actual)
		{
			Assert.IsTrue(actual.X.IsNearlyEqual(expected.X),
				"Actual: " + actual + ", Expected: " + expected);
			Assert.IsTrue(actual.Y.IsNearlyEqual(expected.Y),
				"Actual: " + actual + ", Expected: " + expected);
		}

		[Test, Category("Slow"), CloseAfterFirstFrame]
		public void TestDrawAreaWhenChangingOrientationToLandscape()
		{
			new Ellipse(new Rectangle(0.7f, 0.7f, 0.1f, 0.1f), Color.Red);
			RunAfterFirstFrame(() =>
			{
				Resolve<Window>().ViewportPixelSize = new Size(800, 480);
				var screen = ScreenSpace.Current;
				var quadSize = screen.FromPixelSpace(new Vector2D(0, 0));
				ArePointsNearlyEqual(new Vector2D(0f, 0.2f), quadSize);
				quadSize = screen.FromPixelSpace(new Vector2D(800, 480));
				ArePointsNearlyEqual(new Vector2D(1, 0.8f), quadSize);
				var pixelSize = screen.ToPixelSpace(new Vector2D(0f, 0.2f));
				ArePointsNearlyEqual(Vector2D.Zero, pixelSize);
				pixelSize = screen.ToPixelSpace(new Vector2D(1, 0.8f));
				ArePointsNearlyEqual(new Vector2D(800, 480), pixelSize);
			});
		}

		[Test, Category("Slow")]
		public void ChangeOrientation()
		{
			var window = Resolve<Window>();
			var line = new Line2D(Vector2D.Zero, Vector2D.One, Color.Green);
			window.BackgroundColor = Color.Blue;
			new Command(() => window.ViewportPixelSize = new Size(800, 480)).Add(new KeyTrigger(Key.A));
			new Command(() => window.ViewportPixelSize = new Size(480, 800)).Add(new KeyTrigger(Key.B));
			RunAfterFirstFrame(() =>
			{
				var screen = ScreenSpace.Current;
				var startPosition = screen.Viewport.TopLeft;
				var endPosition = screen.Viewport.BottomRight;
				window.Title = "Size: " + window.ViewportPixelSize + " Start: " + startPosition +
					" End: " + endPosition;
				line.StartPoint = startPosition;
				line.EndPoint = endPosition;
			});
		}
	}
}