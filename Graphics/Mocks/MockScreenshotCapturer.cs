namespace DeltaEngine.Graphics.Mocks
{
	/// <summary>
	/// Mock taker of screenshots used in unit tests.
	/// </summary>
	public class MockScreenshotCapturer : ScreenshotCapturer
	{
		public void MakeScreenshot(string fileName)
		{
			LastFilename = fileName;
		}

		public string LastFilename { get; private set; }
	}
}