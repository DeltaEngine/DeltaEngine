using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using DeltaEngine.ScreenSpaces;
using NUnit.Framework;

namespace DeltaEngine.Input.Windows.Tests
{
	public class CursorPositionTranslaterTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void GetClientPositionOnScreen()
		{
			var window = Resolve<Window>();
			var translator = new CursorPositionTranslater(window);
			var outsidePosition = Resolve<ScreenSpace>().FromPixelSpace(new Vector2D(-10, -10));
			var screenPos = translator.ToScreenPositionFromScreenSpace(outsidePosition);
			Assert.IsTrue(screenPos.X < window.PixelPosition.X || screenPos.Y < window.PixelPosition.Y);
			Assert.AreEqual(outsidePosition, translator.FromScreenPositionToScreenSpace(screenPos));
		}

		[Test, CloseAfterFirstFrame]
		public void ConvertPixelFromScreenPositionAndBack()
		{
			var positionTranslator = new CursorPositionTranslater(Resolve<Window>());
			var topLeftPixel = Vector2D.Zero;
			var outside = positionTranslator.FromScreenPositionToScreenSpace(topLeftPixel);
			Assert.AreEqual(topLeftPixel, positionTranslator.ToScreenPositionFromScreenSpace(outside));
		}

		// ncrunch: no coverage start
		[Test, CloseAfterFirstFrame, Ignore] // This moves the mouse every time NCrunch runs!
		public void GetReturnsWhatWasSet()
		{
			var positionTranslator = new CursorPositionTranslater(Resolve<Window>());
			var setPoint = new Vector2D(0.1f, 0.2f);
			positionTranslator.SetCursorPosition(setPoint);
			var getPoint = positionTranslator.GetCursorPosition();
			Assert.AreEqual(setPoint.X, getPoint.X, 0.1f);
			Assert.AreEqual(setPoint.Y, getPoint.Y, 0.1f);
		}
	}
}