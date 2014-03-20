using DeltaEngine.Datatypes;

namespace DeltaEngine.Commands
{
	/// <summary>
	/// Allows a position based trigger to be invoked along with any associated Command or Entity.
	/// </summary>
	public interface PositionTrigger
	{
		Vector2D Position { get; set; }
	}
}