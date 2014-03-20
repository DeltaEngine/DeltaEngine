namespace Blocks
{
	/// <summary>
	/// The various rendering layers. Higher layers overdraw lower ones 
	/// </summary>
	public enum BlocksRenderLayer
	{
		Background,
		Foreground,
		Grid,
		FallingBrick,
		ZoomingBrick
	}
}