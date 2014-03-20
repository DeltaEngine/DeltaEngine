using DeltaEngine.Content;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Scenes.Controls
{
	/// <summary>
	/// Used within a tilemap.
	/// </summary>
	public interface Tile
	{
		Material Material { get; set; }
		Color Color { get; set; }
	}
}