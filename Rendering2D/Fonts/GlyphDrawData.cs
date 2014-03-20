using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Glyph draw info is used by FontData and for rendering glyphs on the screen.
	/// </summary>
	public struct GlyphDrawData
	{
		public Rectangle DrawArea;
		public Rectangle UV;
	}
}