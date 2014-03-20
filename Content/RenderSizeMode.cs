namespace DeltaEngine.Content
{
	/// <summary>
	/// Normally image sizes for sprites are rendered in the size they were saved, e.g. a 200x100
	/// image is rendered in that size. For easier lay-outing rescaling is possible, e.g. a 256x256
	/// image in 800x480 is rendered as 256x256 in 800x480 resolution or 614x614 pixels in 1920x1080.
	/// </summary>
	public enum RenderSizeMode
	{
		PixelBased,
		SizeFor800X480,
		SizeFor1024X768,
		SizeFor1280X720,
		SizeFor1920X1080,
		SizeForSettingsResolution
	}
}