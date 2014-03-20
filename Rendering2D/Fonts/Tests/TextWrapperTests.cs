using DeltaEngine.Content;
using DeltaEngine.Datatypes;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	public class TextWrapperTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void Init()
		{
			fontDescription = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			textWrapper = new TextWrapper(fontDescription.GlyphDictionary, ' ', fontDescription.PixelLineHeight);
		}

		private FontDescription fontDescription;
		private TextWrapper textWrapper;

		[Test]
		public void EmptyText()
		{
			var lines = textWrapper.SplitTextIntoLines("", new Size(100, fontDescription.PixelLineHeight), true);
			Assert.AreEqual(0, lines.Count);
		}

		[Test]
		public void FontDoesNotFitInTooSmallArea()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontDescription.PixelLineHeight / 2.0f), true);
			Assert.AreEqual(0, lines.Count);
		}

		private const string ThreeLineText = Spaces + "\n" + Spaces + "\n" + Spaces;
		private const string Spaces = "   ";

		[Test]
		public void GetLines()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(70, fontDescription.PixelLineHeight * 3), true);
			Assert.AreEqual(3, lines.Count);
			Assert.AreEqual(Spaces, new string(lines[0].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[1].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[2].ToArray()));
		}

		[Test]
		public void GetLinesWithNoSpaceLetters()
		{
			var lines = textWrapper.SplitTextIntoLines("aaaa aaaaa aaa!",
				new Size(70, fontDescription.PixelLineHeight * 3), true);
			Assert.AreEqual(2, lines.Count);
		}

		[Test]
		public void ClipTextHeight()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(60, fontDescription.PixelLineHeight * 2), true);
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual(Spaces, new string(lines[0].ToArray()));
			Assert.AreEqual(Spaces, new string(lines[1].ToArray()));
		}

		[Test]
		public void ClipTextWidth()
		{
			var lines = textWrapper.SplitTextIntoLines(ThreeLineText,
				new Size(10, fontDescription.PixelLineHeight * 6), true);
			Assert.AreEqual(6, lines.Count);
			Assert.AreEqual("  ", new string(lines[0].ToArray()));
			Assert.AreEqual(" ", new string(lines[1].ToArray()));
			Assert.AreEqual("  ", new string(lines[2].ToArray()));
			Assert.AreEqual(" ", new string(lines[3].ToArray()));
			Assert.AreEqual("  ", new string(lines[4].ToArray()));
			Assert.AreEqual(" ", new string(lines[5].ToArray()));
		}
	}
}