using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	public class TextConverterTests : TestWithMocksOrVisually
	{
		[Test, CloseAfterFirstFrame]
		public void GetGlyphs()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			var textConverter = new TextConverter(fontData.GlyphDictionary, fontData.PixelLineHeight);
			var glyphs = textConverter.GetRenderableGlyphs("    ", HorizontalAlignment.Center);
			Assert.AreEqual(4, glyphs.Length);
		}

		[Test, CloseAfterFirstFrame]
		public void GetGlyphsRightAligned()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			var textConverter = new TextConverter(fontData.GlyphDictionary, fontData.PixelLineHeight);
			var glyphs = textConverter.GetRenderableGlyphs("A a a aaa", HorizontalAlignment.Right);
			Assert.AreEqual(9, glyphs.Length);
		}
	}
}