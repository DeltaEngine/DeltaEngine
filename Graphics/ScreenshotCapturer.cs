namespace DeltaEngine.Graphics
{
	/// <summary>
	/// For taking screenshots.
	/// </summary>
	public interface ScreenshotCapturer
	{
		void MakeScreenshot(string fileName);
	}
}