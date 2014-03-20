using System.Collections.Generic;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Breaks a string into a list of lines each of which is a list of characters.
	/// Also converts tabs to spaces and converts unsupported characters to question marks. 
	/// </summary>
	public class TextParser
	{
		public TextParser(Dictionary<char, Glyph> glyphDictionary,
			char fallbackCharForUnsupportedCharacters)
		{
			this.glyphDictionary = glyphDictionary;
			this.fallbackCharForUnsupportedCharacters = fallbackCharForUnsupportedCharacters;
		}

		private readonly Dictionary<char, Glyph> glyphDictionary;
		private readonly char fallbackCharForUnsupportedCharacters = '?';

		public List<List<char>> GetLines(string text)
		{
			var lines = new List<List<char>>();
			var currentLine = new List<char>();
			characters = text.ToCharArray();
			for (index = 0; index < characters.Length; index++)
			{
				if (IsWindowsNewLine())
					index++;
				else if (IsTab())
				{
					currentLine.Add(' ');
					currentLine.Add(' ');
				}
				else if (!IsSpecialCharacter())
					if (IsSupportedByFont())
						currentLine.Add(characters[index]);
					else if (IsFallbackCharacterSupportedByFont())
						currentLine.Add(fallbackCharForUnsupportedCharacters);

				if (IsNewLine() || IsLastCharacterOfText())
				{
					lines.Add(currentLine);
					currentLine = new List<char>();
				}
			}
			return lines;
		}

		private char[] characters;
		private int index;

		private bool IsWindowsNewLine()
		{
			return characters[index] == '\r' && index + 1 < characters.Length &&
				characters[index + 1] == '\n';
		}

		private bool IsTab()
		{
			return characters[index] == '\t';
		}

		private bool IsSpecialCharacter()
		{
			return characters[index] < ' ';
		}

		private bool IsSupportedByFont()
		{
			return glyphDictionary.ContainsKey(characters[index]);
		}

		private bool IsFallbackCharacterSupportedByFont()
		{
			return glyphDictionary.ContainsKey(fallbackCharForUnsupportedCharacters);
		}

		private bool IsNewLine()
		{
			return IsUnixNewLine() || IsMacNewLine() || IsWindowsNewLine();
		}

		private bool IsUnixNewLine()
		{
			return characters[index] == '\n';
		}

		private bool IsMacNewLine()
		{
			return characters[index] == '\r';
		}

		private bool IsLastCharacterOfText()
		{
			return index + 1 == characters.Length;
		}
	}
}