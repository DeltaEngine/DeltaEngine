using System;

namespace DeltaEngine.Datatypes
{
	/// <summary>
	/// Sprites can be rendered flipped horizontally or vertically.
	/// </summary>
	[Flags]
	public enum FlipMode
	{
		None,
		Horizontal,
		Vertical,
		HorizontalAndVertical
	}
}