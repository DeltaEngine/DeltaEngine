using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	public class FontDataTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void LoadFontData()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			Assert.AreEqual("Verdana", fontData.FontFamilyName);
			Assert.AreEqual(12, fontData.SizeInPoints);
			Assert.AreEqual("AddOutline", fontData.Style);
			Assert.AreEqual(16, fontData.PixelLineHeight);
			Assert.AreEqual("Verdana12Font", fontData.FontMapName);
			Assert.AreEqual(new Size(128, 128), fontData.FontMapPixelSize);
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), fontData.GlyphDictionary[' '].UV);
			Assert.AreEqual(7.34875f, fontData.GlyphDictionary[' '].AdvanceWidth);
		}

		[Test, CloseAfterFirstFrame]
		public void GetGlyphDrawAreaAndUVs()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			fontData.Generate("", HorizontalAlignment.Center);
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate("\n", HorizontalAlignment.Center);
			Assert.AreEqual(0, fontData.Glyphs.Length);
			fontData.Generate(" ", HorizontalAlignment.Center);
			Assert.AreEqual(1, fontData.Glyphs.Length);
			GlyphDrawData glyphA = fontData.Glyphs[0];
			Assert.AreEqual(glyphA.UV,
				Rectangle.BuildUVRectangle(new Rectangle(0, 0, 1, 16), new Size(128, 128)));
			Assert.AreEqual(new Rectangle(0, 0, 1, 16), glyphA.DrawArea);
		}

		[Test, CloseAfterFirstFrame]
		public void GetGlyphsForMultilineText()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			fontData.Generate(" \n \n ", HorizontalAlignment.Center);
			Assert.AreEqual(3, fontData.Glyphs.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void ToStringTest()
		{
			var fontDataType = typeof(FontDescription);
			var expected = fontDataType.Namespace + "." + fontDataType.Name +
				", Font Family=Verdana, Font Size=12";
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			Assert.AreEqual(expected, fontData.ToString());
		}
	}
}