using System.Diagnostics;
using DeltaEngine.Core;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;
using DeltaEngine.Graphics.Mocks;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Graphics.Tests
{
	internal class ScreenshotCapturerTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void MakeScreenshotOfYellowBackground()
		{
			Resolve<Window>().BackgroundColor = Color.Yellow;
			new DrawingTests.Line(Vector2D.Zero, new Vector2D(1280, 720), Color.Red);
			RunAfterFirstFrame(() =>
			{
				var capturer = Resolve<ScreenshotCapturer>();
				capturer.MakeScreenshot(ScreenshotFileName);
				if (!StackTraceExtensions.StartedFromNCrunchOrNunitConsole)
					Process.Start(ScreenshotFileName); //ncrunch: no coverage
				else if (capturer is MockScreenshotCapturer)
					Assert.AreEqual(ScreenshotFileName, (capturer as MockScreenshotCapturer).LastFilename);
			});
		}

		private const string ScreenshotFileName = "Test.png";
	}
}