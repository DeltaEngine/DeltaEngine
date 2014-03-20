using System.Collections.Generic;
using System.IO;
using DeltaEngine.Content.Xml;
using DeltaEngine.Datatypes;

namespace DeltaEngine.Rendering2D.Fonts
{
	/// <summary>
	/// Holds the individual glyphs and kerning data for rendering a smoothly drawn font.
	/// </summary>
	internal class FontDescription
	{
		public FontDescription(XmlData data)
		{
			this.data = data;
			GlyphDictionary = new Dictionary<char, Glyph>();
			LoadFromXmlData();
			converter = new TextConverter(GlyphDictionary, PixelLineHeight);
		}

		private readonly XmlData data;
		public Dictionary<char, Glyph> GlyphDictionary { get; private set; }
		private readonly TextConverter converter;

		private void LoadFromXmlData()
		{
			var fontImagePixelSize = FontMapPixelSize;
			foreach (var child in data.Children)
				if (child.Name == "Glyphs")
					foreach (var glyphData in child.Children)
						LoadGlyph(glyphData, fontImagePixelSize);
				else if (child.Name == "Kernings")
					foreach (var kerningData in child.Children)
						LoadKerning(kerningData);
		}

		private void LoadGlyph(XmlData glyphData, Size fontMapSize)
		{
			char character = glyphData.GetAttributeValue("Character", ' ');
			var glyph = new Glyph
			{
				UV = new Rectangle(glyphData.GetAttributeValue("UV")),
				LeftSideBearing = glyphData.GetAttributeValue("LeftBearing", 0.0f),
				RightSideBearing = glyphData.GetAttributeValue("RightBearing", 0.0f)
			};
			glyph.AdvanceWidth = glyphData.GetAttributeValue("AdvanceWidth", glyph.UV.Width - 2.0f);
			glyph.PrecomputedFontMapUV = Rectangle.BuildUVRectangle(glyph.UV, fontMapSize);
			GlyphDictionary.Add(character, glyph);
		}

		//ncrunch: no coverage start
		public void LoadKerning(XmlData kerningData)
		{
			char firstChar = kerningData.GetAttributeValue("First", '\0');
			char secondChar = kerningData.GetAttributeValue("Second", '\0');
			int kerningDistance = kerningData.GetAttributeValue("Distance", 0);
			if (firstChar == '\0' || secondChar == '\0' || kerningDistance == 0)
				throw new InvalidDataException("Unable to add kerning " + firstChar + " and " + secondChar +
					" with distance=" + kerningDistance);
			Glyph glyph;
			if (GlyphDictionary.TryGetValue(firstChar, out glyph))
				glyph.Kernings.Add(secondChar, kerningDistance);
		} //ncrunch: no coverage end

		public string FontFamilyName
		{
			get { return data.GetAttributeValue("Family", "Verdana"); }
		}

		public int SizeInPoints
		{
			get { return data.GetAttributeValue("Size", 12); }
		}

		public string Style
		{
			get { return data.GetAttributeValue("Style"); }
		}

		public int PixelLineHeight
		{
			get { return data.GetAttributeValue("LineHeight", 20); }
		}

		public string FontMapName
		{
			get
			{
				var bitmap = data.GetChild("Bitmap");
				return bitmap == null ? "Verdana12Font" : bitmap.GetAttributeValue("Name");
			}
		}

		public Size FontMapPixelSize
		{
			get
			{
				var bitmap = data.GetChild("Bitmap");
				return bitmap == null ? Size.One : new Size(
					bitmap.GetAttributeValue("Width", 256), bitmap.GetAttributeValue("Height", 256));
			}
		}

		public void Generate(string text, HorizontalAlignment alignment)
		{
			Glyphs = converter.GetRenderableGlyphs(text, alignment);
			DrawSize = converter.MaxTextPixelSize;
		}

		public GlyphDrawData[] Glyphs { get; private set; }
		public Size DrawSize { get; private set; }

		public override string ToString()
		{
			return base.ToString() + ", Font Family=" + FontFamilyName + ", Font Size=" + SizeInPoints;
		}
	}
}