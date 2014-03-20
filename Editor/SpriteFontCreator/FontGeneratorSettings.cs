using System;
using System.Collections.Generic;
using System.Linq;
using DeltaEngine.Datatypes;
using DeltaEngine.Extensions;

namespace DeltaEngine.Editor.SpriteFontCreator
{
	/// <summary>
	/// Helper class to manage the settings passing to the
	/// FontGenerator.GenerateFont method (everything except the font itself
	/// and the font size to generate).
	/// </summary>
	public class FontGeneratorSettings
	{
		public FontGeneratorSettings()
		{
			OutlineColor = DefaultOutlineColor;
			ShadowColor = DefaultShadowColor;
			FontColor = DefaultFontColor;
			UseKerning = true;
		}

		public float FontDpi = DefaultFontDpi;
		public Color FontColor;
		public Color ShadowColor;
		public Color OutlineColor;
		public float ShadowDistancePercent = DefaultShadowDistancePercent;
		public float OutlineThicknessPercent = DefaultOutlineThicknessPercent;
		public int CharacterMarginLeft = 0;
		public int CharacterMarginTop = 0;
		public int CharacterMarginRight = 0;
		public int CharacterMarginBottom = 0;
		public bool UseKerning = true;
		public bool DebugMode;
		public FontStyle Style = FontStyle.Normal;
		public List<char> CharactersToGenerate = new List<char>();
		public float LineHeight = 1;
		public float Tracking = 1;

		public const float DefaultFontDpi = 72;
		public readonly Color DefaultFontColor = Color.White;
		public readonly Color DefaultShadowColor = new Color(0, 0, 0, 128);
		public readonly Color DefaultOutlineColor = Color.Black;
		public const float DefaultShadowDistancePercent = 0.02f;
		public const float DefaultOutlineThicknessPercent = 0.033f;

		public void AddStyle(FontStyle styleToAdd)
		{
			Style |= styleToAdd;
		}

		public void RemoveStyle(FontStyle styleToRemove)
		{
			Style -= styleToRemove;
		}

		public bool IsFontStyleSet(FontStyle styleToCheck)
		{
			return (styleToCheck & Style) != 0;
		}

		[Flags]
		public enum FontStyle
		{
			Normal = 0,
			Italic = 1,
			Bold = 2,
			Underline = 4,
			AddShadow = 8,
			AddOutline = 16,
			HighContrast = 32,
			LowContrast = 64,
			SmoothAntiAliasing = 128,
			NoAntiAliasing = 256,
			ClearTypeAntiAliasing = 512,
		}

		public static List<char> GetCharactersToGenerate(string charactersFormula)
		{
			var outputCharacters = new List<char>();
			outputCharacters.Add(' ');
			string[] formulaParts = charactersFormula.SplitAndTrim(',');
			foreach (string formulaPart in formulaParts)
			{
				string[] startAndEnd = formulaPart.SplitAndTrim('-');
				if (startAndEnd.Length == 2 && startAndEnd[0].Length == 1 && startAndEnd[1].Length == 1)
					for (char character = startAndEnd[0][0]; character <= startAndEnd[1][0]; character++)
						outputCharacters.Add(character);
				else
					foreach (char character in formulaPart)
						outputCharacters.Add(character);
			}
			return outputCharacters.ToList();
		}

		public void ExcludeUnusedCharacters()
		{
			var charactersToExclude = new List<char>();
			for (var letter = (char)127; letter < 191; letter++)
				if (letter != (char)161 && // ¡
					letter != (char)162 && // ¢
					letter != (char)163 && // £
					letter != (char)167 && // §
					letter != (char)176 && // °
					letter != (char)191) // ¿
					charactersToExclude.Add(letter);

			charactersToExclude.Add((char)215); // ×
			charactersToExclude.Add((char)247); // ÷

			foreach (char letter in charactersToExclude)
				CharactersToGenerate.Remove(letter);
		}
	}
}