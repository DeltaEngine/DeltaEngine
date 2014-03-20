using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Takes a string of text and returns an array of graphical glyph data for rendering.
	/// </summary>
	public class TextConverter
	{
		public TextConverter(Dictionary<char, Glyph> glyphDictionary, int pixelLineHeight)
		{
			this.glyphDictionary = glyphDictionary;
			wrapper = new TextWrapper(glyphDictionary, FallbackCharForUnsupportedCharacters,
				pixelLineHeight);
			MaxTextPixelSize = Size.Zero;
			glyphs = new List<GlyphDrawData>();
		}

		private readonly Dictionary<char, Glyph> glyphDictionary;
		private readonly TextWrapper wrapper;
		private const char FallbackCharForUnsupportedCharacters = '?';
		public Size MaxTextPixelSize { get; private set; }
		private readonly List<GlyphDrawData> glyphs;

		public GlyphDrawData[] GetRenderableGlyphs(string text, HorizontalAlignment alignment)
		{
			SetInitialValues();
			var allLines = wrapper.SplitTextIntoLines(text, MaxTextPixelSize, false);
			for (int lineIndex = 0; lineIndex < allLines.Count; lineIndex++)
			{
				var singleLine = allLines[lineIndex];
				if (!IsTextLineEmpty(singleLine))
					CreateLineAlignmentAndGlyphs(singleLine, lineIndex, alignment);
				AdvanceLineVertically();
			}
			UpdateMaximumLinePixels();
			return glyphs.ToArray();
		}

		private void SetInitialValues()
		{
			glyphs.Clear();
			lastGlyph = null;
			lastDrawData = CreateFirstGlyphDrawData(wrapper.GetFontHeight());
		}

		private Glyph lastGlyph;
		private GlyphDrawData lastDrawData;

		private void CreateLineAlignmentAndGlyphs(List<char> singleLine, int lineIndex,
			HorizontalAlignment alignment)
		{
			AlignTextLineHorizontally(singleLine, lineIndex, alignment);
			float totalGlyphWidth = 0.0f;
			float lineStartX = lastDrawData.DrawArea.Left;
			foreach (char lineCharacter in singleLine)
			{
				CreateCharacterGlyph(lineCharacter, ref totalGlyphWidth, lineStartX);
			}
		}

		private void CreateCharacterGlyph(char lineCharacter, ref float totalGlyphWidth,
			float lineStartX)
		{
			Glyph characterGlyph = GetGlyphFromDictionary(lineCharacter);
			totalGlyphWidth += GetKerningFromDictionary(lineCharacter, lastGlyph);
			var newDrawInfo = PlaceGlyphInLine(characterGlyph, lineStartX, totalGlyphWidth);
			glyphs.Add(newDrawInfo);
			lastDrawData = newDrawInfo;
			totalGlyphWidth += (float)Math.Round(characterGlyph.AdvanceWidth);
			lastGlyph = characterGlyph;
		}

		private void AdvanceLineVertically()
		{
			lastDrawData.DrawArea.Top += wrapper.GetFontHeight();
		}

		private void UpdateMaximumLinePixels()
		{
			MaxTextPixelSize = new Size(wrapper.MaxTextLineWidth, lastDrawData.DrawArea.Top);
		}

		private static GlyphDrawData CreateFirstGlyphDrawData(float totalLineHeight)
		{
			return new GlyphDrawData
			{
				DrawArea = new Rectangle(Vector2D.Zero, new Size(0, totalLineHeight)),
				UV = new Rectangle(Vector2D.Zero, new Size(0, totalLineHeight))
			};
		}

		private static bool IsTextLineEmpty(List<char> textLine)
		{
			return textLine.Count <= 0;
		}

		private void AlignTextLineHorizontally(List<char> textLine, int lineIndex,
			HorizontalAlignment alignment)
		{
			if (alignment == HorizontalAlignment.Center)
				CenterTextLine(textLine, lineIndex);
			else if (alignment == HorizontalAlignment.Left)
				LeftAlignTextLine(textLine);
			else
				RightAlignTextLine(textLine, lineIndex);
		}

		private void CenterTextLine(List<char> textLine, int lineIndex)
		{
			char firstChar = textLine[0];
			lastDrawData.DrawArea.Left =
				((wrapper.MaxTextLineWidth - wrapper.TextLineWidths[lineIndex]) * 0.5f -
					glyphDictionary[firstChar].LeftSideBearing).Round();
		}

		private void LeftAlignTextLine(List<char> textLine)
		{
			char firstChar = textLine[0];
			lastDrawData.DrawArea.Left = - glyphDictionary[firstChar].LeftSideBearing.Round();
		}

		private void RightAlignTextLine(List<char> textLine, int lineIndex)
		{
			char lastChar = textLine[textLine.Count - 1];
			lastDrawData.DrawArea.Left =
				((wrapper.MaxTextLineWidth - wrapper.TextLineWidths[lineIndex]) -
					glyphDictionary[lastChar].RightSideBearing).Round();
		}

		private Glyph GetGlyphFromDictionary(char textChar)
		{
			Glyph characterGlyph;
			return glyphDictionary.TryGetValue(textChar, out characterGlyph)
				? characterGlyph : glyphDictionary[FallbackCharForUnsupportedCharacters];
		}

		private static int GetKerningFromDictionary(char textChar, Glyph lastGlyph)
		{
			int characterKerning;
			return lastGlyph != null && lastGlyph.Kernings != null &&
				lastGlyph.Kernings.TryGetValue(textChar, out characterKerning) ? characterKerning : 0;
		}

		private GlyphDrawData PlaceGlyphInLine(Glyph characterGlyph, float lineStartX,
			float totalGlyphWidth)
		{
			var glyph = new GlyphDrawData();
			var position =
				new Vector2D((lineStartX + totalGlyphWidth + characterGlyph.LeftSideBearing).Round(),
					lastDrawData.DrawArea.Top);
			glyph.DrawArea = new Rectangle(position, characterGlyph.UV.Size);
			glyph.UV = characterGlyph.PrecomputedFontMapUV;
			return glyph;
		}
	}
}