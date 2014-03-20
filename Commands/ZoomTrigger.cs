namespace DeltaEngine.Commands
{
	/// <summary>
	/// Allows the application to get informed if any input device triggers any zoom gesture.
	/// </summary>
	public interface ZoomTrigger
	{
		float ZoomAmount { get; set; }
	}
}