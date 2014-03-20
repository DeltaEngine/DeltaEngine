using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Used in FontData to store all glyph character data from the font bitmap. For some help see:
	/// http://blogs.msdn.com/garykac/articles/749188.aspx
	/// </summary>
	public class Glyph
	{
		/// <summary>
		/// UV Rectangle (in Pixels) used for drawing this character.
		/// </summary>
		public Rectangle UV;

		/// <summary>
		/// How many pixels we have to advance to the right for this character (different from UV.Width)
		/// </summary>
		public float AdvanceWidth;

		/// <summary>
		/// Left side bearing (in pixels) is used to offset the first character of a text to the left.
		/// </summary>
		public float LeftSideBearing;

		/// <summary>
		/// Right side bearing (in pixels) is used to offset last character of a text to the right.
		/// </summary>
		public float RightSideBearing;

		/// <summary>
		/// Contains the amount of extra distances offsets between this character to any other one.
		/// </summary>
		public readonly Dictionary<char, int> Kernings = new Dictionary<char, int>();

		/// <summary>
		/// Not stored in the Xml font file because we can easily generate them at load time (UV/size).
		/// </summary>
		public Rectangle PrecomputedFontMapUV;
	}
}