using System;
using DeltaEngine.Content;
using DeltaEngine.Platforms;
using NUnit.Framework;

namespace DeltaEngine.Rendering2D.Fonts.Tests
{
	public class TextParserTests : TestWithMocksOrVisually
	{
		[SetUp, CloseAfterFirstFrame]
		public void GetTextParser()
		{
			var fontData = new FontDescription(ContentLoader.Load<Font>("Verdana12").Data);
			parser = new TextParser(fontData.GlyphDictionary, ' ');
		}

		private TextParser parser;

		[Test]
		public void ParseEmptyText()
		{
			var lines = parser.GetLines(GetSpaces(0));
			Assert.AreEqual(0, lines.Count);
		}

		private static string GetSpaces(int numberOfSpaces)
		{
			string spaces = "";
			for (int i = 0; i < numberOfSpaces; i++)
				spaces += " ";
			return spaces;
		}

		[Test]
		public void ParseSingleTextLine()
		{
			var lines = parser.GetLines(GetSpaces(3));
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(3), new string(lines[0].ToArray()));
		}

		[Test]
		public void ParseMultipleTextLines()
		{
			var lines = parser.GetLines(GetSpaces(1) + Environment.NewLine + GetSpaces(2));
			Assert.AreEqual(2, lines.Count);
			Assert.AreEqual(GetSpaces(1), new string(lines[0].ToArray()));
			Assert.AreEqual(GetSpaces(2), new string(lines[1].ToArray()));
		}

		[Test]
		public void ConvertTabsIntoTwoSpaces()
		{
			var lines = parser.GetLines("\t \t");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(5), new string(lines[0].ToArray()));
		}

		[Test]
		public void ParseWithUnsupportedCharacters()
		{
			var lines = parser.GetLines("äöüÄÖÜ");
			Assert.AreEqual(1, lines.Count);
			Assert.AreEqual(GetSpaces(6), new string(lines[0].ToArray()));
		}
	}
}