using System;
using System.Collections.Generic;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	///   Wraps a string of text introducing line breaks between words where possible.
	/// </summary>
	public class TextWrapper
	{
		public TextWrapper(Dictionary<char, Glyph> glyphDictionary,
			char fallbackCharForUnsupportedCharacters, int pixelLineHeight)
		{
			this.glyphDictionary = glyphDictionary;
			parser = new TextParser(glyphDictionary, fallbackCharForUnsupportedCharacters);
			this.pixelLineHeight = pixelLineHeight;
		}

		private readonly Dictionary<char, Glyph> glyphDictionary;
		private readonly TextParser parser;
		private readonly int pixelLineHeight;

		public List<List<char>> SplitTextIntoLines(string text, Size maxTextPixelSize,
			bool isClipping)
		{
			characterLines = new List<List<char>>();
			if (String.IsNullOrEmpty(text) ||
					isClipping && IsFontFittingIntoHeight(maxTextPixelSize.Height))
				return characterLines;
			MaxTextLineWidth = 0.0f;
			TextLineWidths = new List<float>();
			parsedLines = parser.GetLines(text);
			for (lineIndex = 0; lineIndex < parsedLines.Count; lineIndex++)
				if (ParseLine(maxTextPixelSize, isClipping))
					break;
			return characterLines;
		}

		private bool ParseLine(Size maxTextPixelSize, bool isClipping)
		{
			parsedLine = parsedLines[lineIndex];
			var currentTextLine = new List<char>();
			currentTextLineWidth = 0.0f;
			currentWord = new List<char>();
			currentWordWidth = 0.0f;
			wordNumber = 0;
			Glyph lastGlyph = null;
			for (characterIndex = 0; characterIndex < parsedLine.Count; characterIndex++)
				lastGlyph = ParseCharacter(maxTextPixelSize, isClipping, lastGlyph, currentTextLine);
			characterLines.Add(currentTextLine);
			TextLineWidths.Add(currentTextLineWidth);
			UpdateMaxTextLineWidth();
			if (!IsEnoughSpaceForNextLineAvailable(isClipping, maxTextPixelSize))
				return true;
			return false;
		}

		private Glyph ParseCharacter(Size maxTextPixelSize, bool isClipping, Glyph lastGlyph,
			List<char> currentTextLine)
		{
			char character = parsedLine[characterIndex];
			glyph = glyphDictionary[character];
			float glyphWidth = glyph.AdvanceWidth;
			glyphWidth += GetGlyphWidth(character, lastGlyph);
			glyphWidth = GetNextFullPixel(glyphWidth);
			glyphWidth += GetGlyphWidthOfNextCharacterInLine();
			if (isClipping)
				HandleClipping(maxTextPixelSize, currentTextLine, character, glyphWidth);
			else
				HandleNoClipping(currentTextLine, character, glyphWidth);
			lastGlyph = glyph;
			return lastGlyph;
		}

		private void HandleClipping(Size maxTextPixelSize, List<char> currentTextLine, char character,
			float glyphWidth)
		{
			currentWord.Add(character);
			if (!IsSpaceOrTab(character))
				currentWordWidth += glyphWidth;
			if (!IsSpaceOrTab(character) && !IsLastCharacterInLine())
				return;
			if (IsEnoughSpaceForWordInCurrentLineAvailable(maxTextPixelSize) || wordNumber == 0)
				AddWord(currentTextLine, character, glyphWidth);
			else
				MoveRestOfLineToTheNextLine();
			currentWord = new List<char>();
			currentWordWidth = 0.0f;
		}

		private void AddWord(List<char> currentTextLine, char character, float glyphWidth)
		{
			currentTextLine.AddRange(currentWord);
			currentTextLineWidth += currentWordWidth;
			if (IsSpaceOrTab(character))
				currentTextLineWidth += glyphWidth;
			wordNumber++;
		}

		private void HandleNoClipping(List<char> currentTextLine, char character, float glyphWidth)
		{
			currentTextLine.Add(character);
			currentTextLineWidth += glyphWidth;
		}

		private List<List<char>> characterLines;

		private bool IsFontFittingIntoHeight(float height)
		{
			return height < GetFontHeight();
		}

		public float MaxTextLineWidth { get; private set; }
		public List<float> TextLineWidths { get; private set; }
		private List<List<char>> parsedLines;
		private int lineIndex;
		private List<char> parsedLine;
		private float currentTextLineWidth;
		private List<char> currentWord;
		private float currentWordWidth;
		private int wordNumber;
		private int characterIndex;
		private Glyph glyph;

		private static int GetGlyphWidth(char character, Glyph lastGlyph)
		{
			int charKerning;
			if (lastGlyph != null && lastGlyph.Kernings != null &&
					lastGlyph.Kernings.TryGetValue(character, out charKerning))
				return charKerning;

			return 0;
		}

		private static float GetNextFullPixel(float glyphWidth)
		{
			return (float)Math.Round(glyphWidth);
		}

		private float GetGlyphWidthOfNextCharacterInLine()
		{
			char nextCharacter = GetNextCharacterInLine();
			if (nextCharacter != NoChar && IsLastCharacterInLine())
				return glyphDictionary[nextCharacter].RightSideBearing; //ncrunch: no coverage

			return 0.0f;
		}

		private char GetNextCharacterInLine()
		{
			return characterIndex + 1 < parsedLine.Count ? parsedLine[characterIndex + 1] : NoChar;
		}

		private const char NoChar = '\0';

		private static bool IsSpaceOrTab(char character)
		{
			return character == ' ' || character == '\t';
		}

		private bool IsLastCharacterInLine()
		{
			return characterIndex + 1 == parsedLine.Count;
		}

		private bool IsEnoughSpaceForWordInCurrentLineAvailable(Size maxTextPixelSize)
		{
			return currentTextLineWidth + currentWordWidth < maxTextPixelSize.Width;
		}

		private void MoveRestOfLineToTheNextLine()
		{
			while (++characterIndex < parsedLine.Count)
				currentWord.Add(parsedLine[characterIndex]);

			parsedLines.Insert(lineIndex + 1, currentWord);
		}

		private void UpdateMaxTextLineWidth()
		{
			if (MaxTextLineWidth < currentTextLineWidth)
				MaxTextLineWidth = currentTextLineWidth;
		}

		private bool IsEnoughSpaceForNextLineAvailable(bool isClipping, Size maxTextPixelSize)
		{
			if (!isClipping)
				return true;

			return GetRequiredHeightForNextLine() <= maxTextPixelSize.Height;
		}

		private float GetRequiredHeightForNextLine()
		{
			return (characterLines.Count + 1) * GetFontHeight();
		}

		public float GetFontHeight()
		{
			return pixelLineHeight;
		}
	}
}